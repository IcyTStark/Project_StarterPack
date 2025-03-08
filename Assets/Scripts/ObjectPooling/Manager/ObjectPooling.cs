using System;
using UnityEngine.Pool;
using UnityEngine;

namespace Utils.ObjectPoolSystem
{
    public abstract class ObjectPoolBase<T> : IObjectPooling<T> where T : MonoBehaviour
    {
        protected IObjectPool<T> objectPool;
        protected T prefab;

        protected IObjectPool<T> ObjectPool => objectPool;

        public ObjectPoolBase(T prefab, bool collectionCheck = true, int defaultCapacity = 20, int maxPoolCapacity = 100)
        {
            this.prefab = prefab;
            objectPool = new ObjectPool<T>(CreateObject, GetObjectFromPool, ReturnObject, DestroyPooledObject, collectionCheck, defaultCapacity, maxPoolCapacity);
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
            return objectPool.Get(); 
        }

        public void Release(T obj)
        {
            objectPool.Release(obj); 
        }
    }
}