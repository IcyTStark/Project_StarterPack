using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedSignals
{
    /// <summary>
    /// Extension methods for working with signals in MonoBehaviours
    /// </summary>
    public static class SignalExtensions
    {
        /// <summary>
        /// Add a listener to a signal with no parameters
        /// </summary>
        public static void AddSignalListener<T>(this MonoBehaviour behaviour, Action handler) 
            where T : ASignal, new()
        {
            Signals.Get<T>().AddListener(handler);
        }
        
        /// <summary>
        /// Add a listener to a signal with one parameter
        /// </summary>
        public static void AddSignalListener<T, TParam>(this MonoBehaviour behaviour, Action<TParam> handler) 
            where T : ASignal<TParam>, new()
        {
            Signals.Get<T>().AddListener(handler);
        }
        
        /// <summary>
        /// Add a listener to a signal with two parameters
        /// </summary>
        public static void AddSignalListener<T, TParam1, TParam2>(this MonoBehaviour behaviour, Action<TParam1, TParam2> handler) 
            where T : ASignal<TParam1, TParam2>, new()
        {
            Signals.Get<T>().AddListener(handler);
        }
        
        /// <summary>
        /// Add a listener to a signal with three parameters
        /// </summary>
        public static void AddSignalListener<T, TParam1, TParam2, TParam3>(this MonoBehaviour behaviour, Action<TParam1, TParam2, TParam3> handler) 
            where T : ASignal<TParam1, TParam2, TParam3>, new()
        {
            Signals.Get<T>().AddListener(handler);
        }
        
        /// <summary>
        /// Remove a listener from a signal with no parameters
        /// </summary>
        public static void RemoveSignalListener<T>(this MonoBehaviour behaviour, Action handler) 
            where T : ASignal, new()
        {
            Signals.Get<T>().RemoveListener(handler);
        }
        
        /// <summary>
        /// Remove a listener from a signal with one parameter
        /// </summary>
        public static void RemoveSignalListener<T, TParam>(this MonoBehaviour behaviour, Action<TParam> handler) 
            where T : ASignal<TParam>, new()
        {
            Signals.Get<T>().RemoveListener(handler);
        }
        
        /// <summary>
        /// Remove a listener from a signal with two parameters
        /// </summary>
        public static void RemoveSignalListener<T, TParam1, TParam2>(this MonoBehaviour behaviour, Action<TParam1, TParam2> handler) 
            where T : ASignal<TParam1, TParam2>, new()
        {
            Signals.Get<T>().RemoveListener(handler);
        }
        
        /// <summary>
        /// Remove a listener from a signal with three parameters
        /// </summary>
        public static void RemoveSignalListener<T, TParam1, TParam2, TParam3>(this MonoBehaviour behaviour, Action<TParam1, TParam2, TParam3> handler) 
            where T : ASignal<TParam1, TParam2, TParam3>, new()
        {
            Signals.Get<T>().RemoveListener(handler);
        }
        
        /// <summary>
        /// Register a signal listener and automatically clean it up when the GameObject is destroyed
        /// </summary>
        public static void RegisterSignal<T>(this MonoBehaviour behaviour, Action handler) 
            where T : ASignal, new()
        {
            // Add listener now
            Signals.Get<T>().AddListener(handler);
            
            // Ensure we unregister when the object is destroyed
            var tracker = behaviour.gameObject.GetComponent<SignalTracker>();
            if (tracker == null)
            {
                tracker = behaviour.gameObject.AddComponent<SignalTracker>();
            }
            
            tracker.RegisterCleanupAction(() => Signals.Get<T>().RemoveListener(handler));
        }
        
        /// <summary>
        /// Register a signal listener with one parameter and automatically clean it up when the GameObject is destroyed
        /// </summary>
        public static void RegisterSignal<T, TParam>(this MonoBehaviour behaviour, Action<TParam> handler) 
            where T : ASignal<TParam>, new()
        {
            // Add listener now
            Signals.Get<T>().AddListener(handler);
            
            // Ensure we unregister when the object is destroyed
            var tracker = behaviour.gameObject.GetComponent<SignalTracker>();
            if (tracker == null)
            {
                tracker = behaviour.gameObject.AddComponent<SignalTracker>();
            }
            
            tracker.RegisterCleanupAction(() => Signals.Get<T>().RemoveListener(handler));
        }
        
        /// <summary>
        /// Unregister all signals associated with a GameObject
        /// </summary>
        public static void UnregisterAllSignals(this MonoBehaviour behaviour)
        {
            Signals.UnbindInstance(behaviour);
        }
    }

    /// <summary>
    /// Component that automatically cleans up signal listeners when a GameObject is destroyed
    /// </summary>
    public class SignalTracker : MonoBehaviour
    {
        private readonly List<Action> _cleanupActions = new List<Action>();
        
        /// <summary>
        /// Register an action to be called when this GameObject is destroyed
        /// </summary>
        public void RegisterCleanupAction(Action cleanupAction)
        {
            _cleanupActions.Add(cleanupAction);
        }
        
        /// <summary>
        /// Execute all cleanup actions
        /// </summary>
        private void OnDestroy()
        {
            foreach (var action in _cleanupActions)
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            
            // Also make sure to unbind any instance signals
            Signals.UnbindInstance(this.gameObject);
        }
    }
}
