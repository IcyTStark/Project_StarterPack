using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EnhancedSignals
{
    /// <summary>
    /// Base interface for all signals
    /// </summary>
    public interface ISignal
    {
        string Hash { get; }
    }

    /// <summary>
    /// Global access point for signals
    /// </summary>
    public static class Signals
    {
        private static readonly SignalHub _globalHub = new SignalHub();

        /// <summary>
        /// Get a signal of the specified type
        /// </summary>
        public static SType Get<SType>() where SType : ISignal, new()
        {
            return _globalHub.Get<SType>();
        }

        /// <summary>
        /// Get a signal of the specified type bound to a specific object instance
        /// </summary>
        public static SType Get<SType>(object instance) where SType : AInstanceSignal, new()
        {
            return _globalHub.Get<SType>(instance);
        }

        /// <summary>
        /// Remove all listeners associated with a specific object instance
        /// </summary>
        public static void UnbindInstance(object instance)
        {
            _globalHub.UnbindInstance(instance);
        }

        /// <summary>
        /// Clean up any dead instance references
        /// </summary>
        public static void Cleanup()
        {
            _globalHub.Cleanup();
        }

        /// <summary>
        /// Clear all signals and listeners in the global hub
        /// </summary>
        public static void Clear()
        {
            _globalHub.Clear();
        }
    }

    /// <summary>
    /// A hub for signals that can be used locally or globally
    /// </summary>
    public class SignalHub
    {
        private readonly Dictionary<Type, ISignal> _signals = new Dictionary<Type, ISignal>();
        private readonly Dictionary<int, HashSet<Type>> _instanceBindings = new Dictionary<int, HashSet<Type>>();
        private readonly Dictionary<int, WeakReference> _instances = new Dictionary<int, WeakReference>();

        /// <summary>
        /// Get a signal of the specified type
        /// </summary>
        public SType Get<SType>() where SType : ISignal, new()
        {
            Type signalType = typeof(SType);
            
            if (_signals.TryGetValue(signalType, out ISignal signal))
            {
                return (SType)signal;
            }

            return (SType)Bind(signalType);
        }

        /// <summary>
        /// Get a signal of the specified type bound to a specific object instance
        /// </summary>
        public SType Get<SType>(object instance) where SType : AInstanceSignal, new()
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            Type signalType = typeof(SType);
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            // Store the instance binding
            if (!_instanceBindings.TryGetValue(instanceId, out HashSet<Type> bindings))
            {
                bindings = new HashSet<Type>();
                _instanceBindings[instanceId] = bindings;
                _instances[instanceId] = new WeakReference(instance);
            }
            
            bindings.Add(signalType);
            
            if (_signals.TryGetValue(signalType, out ISignal signal))
            {
                AInstanceSignal instanceSignal = (AInstanceSignal)signal;
                instanceSignal.BindInstance(instance);
                return (SType)signal;
            }

            SType newSignal = (SType)Bind(signalType);
            newSignal.BindInstance(instance);
            return newSignal;
        }

        /// <summary>
        /// Create a signal binding for the specified type
        /// </summary>
        private ISignal Bind(Type signalType)
        {
            if (_signals.TryGetValue(signalType, out ISignal signal))
            {
                Debug.LogWarning($"Signal already registered for type {signalType}");
                return signal;
            }

            signal = (ISignal)Activator.CreateInstance(signalType);
            _signals.Add(signalType, signal);
            return signal;
        }

        /// <summary>
        /// Remove all listeners associated with a specific object instance
        /// </summary>
        public void UnbindInstance(object instance)
        {
            if (instance == null) return;
            
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            if (_instanceBindings.TryGetValue(instanceId, out HashSet<Type> bindings))
            {
                foreach (Type signalType in bindings)
                {
                    if (_signals.TryGetValue(signalType, out ISignal signal) && signal is AInstanceSignal instanceSignal)
                    {
                        instanceSignal.UnbindInstance(instance);
                    }
                }
                
                _instanceBindings.Remove(instanceId);
                _instances.Remove(instanceId);
            }
        }

        /// <summary>
        /// Clean up any dead instance references
        /// </summary>
        public void Cleanup()
        {
            List<int> deadIds = new List<int>();
            
            foreach (KeyValuePair<int, WeakReference> pair in _instances)
            {
                if (!pair.Value.IsAlive)
                {
                    deadIds.Add(pair.Key);
                }
            }
            
            foreach (int id in deadIds)
            {
                if (_instanceBindings.TryGetValue(id, out HashSet<Type> bindings))
                {
                    foreach (Type signalType in bindings)
                    {
                        if (_signals.TryGetValue(signalType, out ISignal signal) && signal is AInstanceSignal instanceSignal)
                        {
                            instanceSignal.UnbindInstanceById(id);
                        }
                    }
                    
                    _instanceBindings.Remove(id);
                    _instances.Remove(id);
                }
            }
        }

        /// <summary>
        /// Clear all signals and listeners
        /// </summary>
        public void Clear()
        {
            _signals.Clear();
            _instanceBindings.Clear();
            _instances.Clear();
        }
    }

    /// <summary>
    /// Base class for all signals
    /// </summary>
    public abstract class ABaseSignal : ISignal
    {
        private string _hash;
        
        /// <summary>
        /// Unique identifier for this signal type
        /// </summary>
        public string Hash
        {
            get
            {
                if (string.IsNullOrEmpty(_hash))
                {
                    _hash = GetType().FullName;
                }
                return _hash;
            }
        }
        
        /// <summary>
        /// Check if a delegate is anonymous (lambda) and warn if in editor
        /// </summary>
        protected static void ValidateDelegate(Delegate handler)
        {
            #if UNITY_EDITOR
            if (handler != null && handler.Method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0 
                && handler.Method.Name.Contains("<"))
            {
                Debug.LogWarning($"Adding anonymous delegate as Signal callback: {handler.Method.Name}. " +
                               "This is not recommended as you won't be able to unregister it later.");
            }
            #endif
        }
    }

    /// <summary>
    /// Base class for signals that can be bound to specific instances
    /// </summary>
    public abstract class AInstanceSignal : ABaseSignal
    {
        protected readonly Dictionary<int, object> BoundInstances = new Dictionary<int, object>();
        
        /// <summary>
        /// Bind this signal to a specific instance
        /// </summary>
        public virtual void BindInstance(object instance)
        {
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            if (!BoundInstances.ContainsKey(instanceId))
            {
                BoundInstances[instanceId] = instance;
            }
        }
        
        /// <summary>
        /// Unbind this signal from a specific instance
        /// </summary>
        public virtual void UnbindInstance(object instance)
        {
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            UnbindInstanceById(instanceId);
        }
        
        /// <summary>
        /// Unbind this signal from a specific instance ID
        /// </summary>
        public virtual void UnbindInstanceById(int instanceId)
        {
            BoundInstances.Remove(instanceId);
        }
        
        /// <summary>
        /// Check if this signal is bound to a specific instance
        /// </summary>
        public bool IsBoundTo(object instance)
        {
            if (instance == null) return false;
            return BoundInstances.ContainsKey(RuntimeHelpers.GetHashCode(instance));
        }
    }
}
