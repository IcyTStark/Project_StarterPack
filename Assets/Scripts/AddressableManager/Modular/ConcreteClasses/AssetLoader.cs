using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FIO.ModularAddressableSystem
{
    public class AssetLoader : IAssetLoader, IDisposable
    {
        private readonly Dictionary<string, AsyncOperationHandle> _loadedAssets = new();

        public async UniTask<T> LoadAssetAsync<T>(string address) where T : UnityEngine.Object
        {
            // Check if we already have a handle for this address
            if (_loadedAssets.TryGetValue(address, out var existingHandle))
            {
                if (existingHandle.IsValid())
                {
                    Debug.Log($"Asset already loaded: {address}");
                    var typedHandle = existingHandle.Convert<T>();
                    if (typedHandle.IsValid())
                    {
                        return typedHandle.Result; // Now safe since it's already complete
                    }
                }
                else
                {
                    // Handle is invalid, remove it
                    _loadedAssets.Remove(address);
                }
            }

            try
            {
                var handle = Addressables.LoadAssetAsync<T>(address);
                T asset = await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _loadedAssets[address] = handle;
                    Debug.Log($"Successfully loaded asset at address: {address}");
                    return asset;
                }
                else
                {
                    Debug.LogError($"Failed to load asset at address: {address}, Error: {handle.OperationException}");
                    Addressables.Release(handle); // Release on failure
                    return null;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception while loading asset at address: {address}, Error: {e.Message}");
                return null;
            }
        }

        public void ReleaseAsset(string address)
        {
            if (_loadedAssets.TryGetValue(address, out var handle))
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                    Debug.Log($"Released: {address}");
                }
                _loadedAssets.Remove(address);
            }
            else
            {
                Debug.LogWarning($"No loaded asset found for: {address}");
            }
        }

        public void ReleaseAll()
        {
            foreach (var kvp in _loadedAssets)
            {
                if (kvp.Value.IsValid())
                {
                    Addressables.Release(kvp.Value);
                }
            }
            _loadedAssets.Clear();
            Debug.Log($"Released all assets");
        }

        public void Dispose()
        {
            ReleaseAll();
        }
    }
}