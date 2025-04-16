using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EnhancedSignals
{
    #region No Parameters Signals

    /// <summary>
    /// Signal with no parameters
    /// </summary>
    public abstract class ASignal : ABaseSignal
    {
        private event Action Callback;
        
        /// <summary>
        /// Add a listener to this signal
        /// </summary>
        public void AddListener(Action handler)
        {
            ValidateDelegate(handler);
            Callback += handler;
        }
        
        /// <summary>
        /// Remove a listener from this signal
        /// </summary>
        public void RemoveListener(Action handler)
        {
            Callback -= handler;
        }
        
        /// <summary>
        /// Dispatch this signal
        /// </summary>
        public void Dispatch()
        {
            try
            {
                Callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
    
    /// <summary>
    /// Instance-aware signal with no parameters
    /// </summary>
    public abstract class AInstanceSignal<TSignal> : AInstanceSignal
        where TSignal : AInstanceSignal<TSignal>, new()
    {
        private readonly Dictionary<int, Action> _instanceListeners = new Dictionary<int, Action>();
        private event Action GlobalCallback;
        
        /// <summary>
        /// Add a global listener to this signal
        /// </summary>
        public void AddListener(Action handler)
        {
            ValidateDelegate(handler);
            GlobalCallback += handler;
        }
        
        /// <summary>
        /// Add a listener for a specific instance
        /// </summary>
        public void AddListener(object instance, Action handler)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            ValidateDelegate(handler);
            
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            if (!_instanceListeners.TryGetValue(instanceId, out Action existing))
            {
                _instanceListeners[instanceId] = handler;
            }
            else
            {
                _instanceListeners[instanceId] = existing + handler;
            }
        }
        
        /// <summary>
        /// Remove a global listener from this signal
        /// </summary>
        public void RemoveListener(Action handler)
        {
            GlobalCallback -= handler;
        }
        
        /// <summary>
        /// Remove a listener for a specific instance
        /// </summary>
        public void RemoveListener(object instance, Action handler)
        {
            if (instance == null) return;
            
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            if (_instanceListeners.TryGetValue(instanceId, out Action existing))
            {
                _instanceListeners[instanceId] = existing - handler;
                
                if (_instanceListeners[instanceId] == null)
                {
                    _instanceListeners.Remove(instanceId);
                }
            }
        }
        
        /// <summary>
        /// Dispatch this signal globally and to the instance
        /// </summary>
        public void Dispatch(object sender)
        {
            try
            {
                // Execute global listeners
                GlobalCallback?.Invoke();
                
                // Execute instance-specific listeners
                if (sender != null)
                {
                    int instanceId = RuntimeHelpers.GetHashCode(sender);
                    if (_instanceListeners.TryGetValue(instanceId, out Action callback))
                    {
                        callback.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        /// <summary>
        /// Unbind listeners for a specific instance
        /// </summary>
        public override void UnbindInstanceById(int instanceId)
        {
            base.UnbindInstanceById(instanceId);
            _instanceListeners.Remove(instanceId);
        }
    }

    #endregion

    #region One Parameter Signals

    /// <summary>
    /// Signal with one parameter
    /// </summary>
    public abstract class ASignal<T> : ABaseSignal
    {
        private event Action<T> Callback;
        
        /// <summary>
        /// Add a listener to this signal
        /// </summary>
        public void AddListener(Action<T> handler)
        {
            ValidateDelegate(handler);
            Callback += handler;
        }
        
        /// <summary>
        /// Remove a listener from this signal
        /// </summary>
        public void RemoveListener(Action<T> handler)
        {
            Callback -= handler;
        }
        
        /// <summary>
        /// Dispatch this signal with one parameter
        /// </summary>
        public void Dispatch(T arg1)
        {
            try
            {
                Callback?.Invoke(arg1);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
    
    /// <summary>
    /// Instance-aware signal with one parameter
    /// </summary>
    public abstract class AInstanceSignal<TSignal, T> : AInstanceSignal
        where TSignal : AInstanceSignal<TSignal, T>, new()
    {
        private readonly Dictionary<int, Action<T>> _instanceListeners = new Dictionary<int, Action<T>>();
        private event Action<T> GlobalCallback;
        
        /// <summary>
        /// Add a global listener to this signal
        /// </summary>
        public void AddListener(Action<T> handler)
        {
            ValidateDelegate(handler);
            GlobalCallback += handler;
        }
        
        /// <summary>
        /// Add a listener for a specific instance
        /// </summary>
        public void AddListener(object instance, Action<T> handler)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            ValidateDelegate(handler);
            
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            if (!_instanceListeners.TryGetValue(instanceId, out Action<T> existing))
            {
                _instanceListeners[instanceId] = handler;
            }
            else
            {
                _instanceListeners[instanceId] = existing + handler;
            }
        }
        
        /// <summary>
        /// Remove a global listener from this signal
        /// </summary>
        public void RemoveListener(Action<T> handler)
        {
            GlobalCallback -= handler;
        }
        
        /// <summary>
        /// Remove a listener for a specific instance
        /// </summary>
        public void RemoveListener(object instance, Action<T> handler)
        {
            if (instance == null) return;
            
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            if (_instanceListeners.TryGetValue(instanceId, out Action<T> existing))
            {
                _instanceListeners[instanceId] = existing - handler;
                
                if (_instanceListeners[instanceId] == null)
                {
                    _instanceListeners.Remove(instanceId);
                }
            }
        }
        
        /// <summary>
        /// Dispatch this signal globally and to the instance
        /// </summary>
        public void Dispatch(object sender, T arg1)
        {
            try
            {
                // Execute global listeners
                GlobalCallback?.Invoke(arg1);
                
                // Execute instance-specific listeners
                if (sender != null)
                {
                    int instanceId = RuntimeHelpers.GetHashCode(sender);
                    if (_instanceListeners.TryGetValue(instanceId, out Action<T> callback))
                    {
                        callback.Invoke(arg1);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        /// <summary>
        /// Unbind listeners for a specific instance
        /// </summary>
        public override void UnbindInstanceById(int instanceId)
        {
            base.UnbindInstanceById(instanceId);
            _instanceListeners.Remove(instanceId);
        }
    }

    #endregion

    #region Two Parameter Signals

    /// <summary>
    /// Signal with two parameters
    /// </summary>
    public abstract class ASignal<T, U> : ABaseSignal
    {
        private event Action<T, U> Callback;
        
        /// <summary>
        /// Add a listener to this signal
        /// </summary>
        public void AddListener(Action<T, U> handler)
        {
            ValidateDelegate(handler);
            Callback += handler;
        }
        
        /// <summary>
        /// Remove a listener from this signal
        /// </summary>
        public void RemoveListener(Action<T, U> handler)
        {
            Callback -= handler;
        }
        
        /// <summary>
        /// Dispatch this signal with two parameters
        /// </summary>
        public void Dispatch(T arg1, U arg2)
        {
            try
            {
                Callback?.Invoke(arg1, arg2);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
    
    /// <summary>
    /// Instance-aware signal with two parameters
    /// </summary>
    public abstract class AInstanceSignal<TSignal, T, U> : AInstanceSignal
        where TSignal : AInstanceSignal<TSignal, T, U>, new()
    {
        private readonly Dictionary<int, Action<T, U>> _instanceListeners = new Dictionary<int, Action<T, U>>();
        private event Action<T, U> GlobalCallback;
        
        /// <summary>
        /// Add a global listener to this signal
        /// </summary>
        public void AddListener(Action<T, U> handler)
        {
            ValidateDelegate(handler);
            GlobalCallback += handler;
        }
        
        /// <summary>
        /// Add a listener for a specific instance
        /// </summary>
        public void AddListener(object instance, Action<T, U> handler)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            ValidateDelegate(handler);
            
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            if (!_instanceListeners.TryGetValue(instanceId, out Action<T, U> existing))
            {
                _instanceListeners[instanceId] = handler;
            }
            else
            {
                _instanceListeners[instanceId] = existing + handler;
            }
        }
        
        /// <summary>
        /// Remove a global listener from this signal
        /// </summary>
        public void RemoveListener(Action<T, U> handler)
        {
            GlobalCallback -= handler;
        }
        
        /// <summary>
        /// Remove a listener for a specific instance
        /// </summary>
        public void RemoveListener(object instance, Action<T, U> handler)
        {
            if (instance == null) return;
            
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            if (_instanceListeners.TryGetValue(instanceId, out Action<T, U> existing))
            {
                _instanceListeners[instanceId] = existing - handler;
                
                if (_instanceListeners[instanceId] == null)
                {
                    _instanceListeners.Remove(instanceId);
                }
            }
        }
        
        /// <summary>
        /// Dispatch this signal globally and to the instance
        /// </summary>
        public void Dispatch(object sender, T arg1, U arg2)
        {
            try
            {
                // Execute global listeners
                GlobalCallback?.Invoke(arg1, arg2);
                
                // Execute instance-specific listeners
                if (sender != null)
                {
                    int instanceId = RuntimeHelpers.GetHashCode(sender);
                    if (_instanceListeners.TryGetValue(instanceId, out Action<T, U> callback))
                    {
                        callback.Invoke(arg1, arg2);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        /// <summary>
        /// Unbind listeners for a specific instance
        /// </summary>
        public override void UnbindInstanceById(int instanceId)
        {
            base.UnbindInstanceById(instanceId);
            _instanceListeners.Remove(instanceId);
        }
    }

    #endregion

    #region Three Parameter Signals

    /// <summary>
    /// Signal with three parameters
    /// </summary>
    public abstract class ASignal<T, U, V> : ABaseSignal
    {
        private event Action<T, U, V> Callback;
        
        /// <summary>
        /// Add a listener to this signal
        /// </summary>
        public void AddListener(Action<T, U, V> handler)
        {
            ValidateDelegate(handler);
            Callback += handler;
        }
        
        /// <summary>
        /// Remove a listener from this signal
        /// </summary>
        public void RemoveListener(Action<T, U, V> handler)
        {
            Callback -= handler;
        }
        
        /// <summary>
        /// Dispatch this signal with three parameters
        /// </summary>
        public void Dispatch(T arg1, U arg2, V arg3)
        {
            try
            {
                Callback?.Invoke(arg1, arg2, arg3);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
    
    /// <summary>
    /// Instance-aware signal with three parameters
    /// </summary>
    public abstract class AInstanceSignal<TSignal, T, U, V> : AInstanceSignal
        where TSignal : AInstanceSignal<TSignal, T, U, V>, new()
    {
        private readonly Dictionary<int, Action<T, U, V>> _instanceListeners = new Dictionary<int, Action<T, U, V>>();
        private event Action<T, U, V> GlobalCallback;
        
        /// <summary>
        /// Add a global listener to this signal
        /// </summary>
        public void AddListener(Action<T, U, V> handler)
        {
            ValidateDelegate(handler);
            GlobalCallback += handler;
        }
        
        /// <summary>
        /// Add a listener for a specific instance
        /// </summary>
        public void AddListener(object instance, Action<T, U, V> handler)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            ValidateDelegate(handler);
            
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            if (!_instanceListeners.TryGetValue(instanceId, out Action<T, U, V> existing))
            {
                _instanceListeners[instanceId] = handler;
            }
            else
            {
                _instanceListeners[instanceId] = existing + handler;
            }
        }
        
        /// <summary>
        /// Remove a global listener from this signal
        /// </summary>
        public void RemoveListener(Action<T, U, V> handler)
        {
            GlobalCallback -= handler;
        }
        
        /// <summary>
        /// Remove a listener for a specific instance
        /// </summary>
        public void RemoveListener(object instance, Action<T, U, V> handler)
        {
            if (instance == null) return;
            
            int instanceId = RuntimeHelpers.GetHashCode(instance);
            
            if (_instanceListeners.TryGetValue(instanceId, out Action<T, U, V> existing))
            {
                _instanceListeners[instanceId] = existing - handler;
                
                if (_instanceListeners[instanceId] == null)
                {
                    _instanceListeners.Remove(instanceId);
                }
            }
        }
        
        /// <summary>
        /// Dispatch this signal globally and to the instance
        /// </summary>
        public void Dispatch(object sender, T arg1, U arg2, V arg3)
        {
            try
            {
                // Execute global listeners
                GlobalCallback?.Invoke(arg1, arg2, arg3);
                
                // Execute instance-specific listeners
                if (sender != null)
                {
                    int instanceId = RuntimeHelpers.GetHashCode(sender);
                    if (_instanceListeners.TryGetValue(instanceId, out Action<T, U, V> callback))
                    {
                        callback.Invoke(arg1, arg2, arg3);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        /// <summary>
        /// Unbind listeners for a specific instance
        /// </summary>
        public override void UnbindInstanceById(int instanceId)
        {
            base.UnbindInstanceById(instanceId);
            _instanceListeners.Remove(instanceId);
        }
    }

    #endregion
}
