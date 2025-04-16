using System;
using UnityEngine.Pool;
using UnityEngine;

namespace TMS.ObjectPoolSystem
{
    public abstract class ObjectPoolBase<T> : IObjectPooling<T> where T : MonoBehaviour
    {
        protected IObjectPool<T> _objectPool;
        protected T _prefab;

        protected IObjectPool<T> ObjectPool => _objectPool;

        public ObjectPoolBase(T prefab, bool collectionCheck = true, int defaultCapacity = 20, int maxPoolCapacity = 100)
        {
            this._prefab = prefab;
            _objectPool = new ObjectPool<T>(CreateObject, GetObjectFromPool, ReturnObject, DestroyPooledObject, collectionCheck, defaultCapacity, maxPoolCapacity);
        }

        public abstract T CreateObject();

        public virtual void GetObjectFromPool(T pooledObject)
        {
            pooledObject.gameObject.SetActive(true);
        }

        public virtual void ReturnObject(T pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }

        public virtual void DestroyPooledObject(T obj)
        {
            UnityEngine.Object.Destroy(obj.gameObject);
        }

        public T Get()
        {
            return _objectPool.Get(); 
        }

        public void Release(T obj)
        {
            _objectPool.Release(obj); 
        }
    }
}