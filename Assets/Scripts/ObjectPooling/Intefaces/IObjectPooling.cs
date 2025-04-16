namespace TMS.ObjectPoolSystem
{
    public interface IObjectPooling<T>
    {
        T CreateObject();
        void GetObjectFromPool(T obj);
        void ReturnObject(T obj);
        void DestroyPooledObject(T obj);
    }
}