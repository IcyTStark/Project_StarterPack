using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FIO.ModularAddressableSystem
{
    public class AssetInstantiator : IAssetInstantiator
    {
        public async UniTask<GameObject> InstantiateAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            try
            {
                var handle = Addressables.InstantiateAsync(address, position, rotation, parent, trackHandle: true);
                GameObject instance = await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Successfully instantiated: {address}");
                    return instance;
                }
                else
                {
                    Debug.LogError($"Failed to instantiate: {address}, Error: {handle.OperationException}");
                    Addressables.Release(handle);
                    return null;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception instantiating: {address}, Error: {e.Message}");
                return null;
            }
        }
    }
}

