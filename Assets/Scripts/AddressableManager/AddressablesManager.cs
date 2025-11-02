using Cysharp.Threading.Tasks;
using deVoid.Utils;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FIO.AddressablesSystem
{
    public class AddressablesManager : Singleton<AddressablesManager>
    {
        private const string LOG_NAME = "[AddressableManager]";

        // Track handles for proper cleanup
        private readonly Dictionary<string, AsyncOperationHandle> _loadedAssets = new();

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            InitializeAddressables().Forget();
        }

        #region Addressables Initialization
        /// <summary>
        /// Initialize Addressables system early to avoid delays on first load.
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid InitializeAddressables()
        {
            try
            {
                var initHandle = Addressables.InitializeAsync();
                await initHandle.ToUniTask();

                if (initHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"{LOG_NAME} Addressables initialized successfully");

                    CheckForContentUpdates().Forget();

                    Signals.Get<OnAddressableInitialized>().Dispatch();
                }
                else
                {
                    Debug.LogError($"{LOG_NAME} Addressables initialization failed: {initHandle.OperationException}");

                    Signals.Get<OnAddressableInitializationFailed>().Dispatch();
                }

                Addressables.Release(initHandle);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{LOG_NAME} Addressables initialization failed: {e.Message}");
            }
        }

        private async UniTaskVoid CheckForContentUpdates()
        {
            try
            {
                var checkForContentUpdateHandle = Addressables.CheckForCatalogUpdates(autoReleaseHandle: true);
                var catalogsToUpdate = await checkForContentUpdateHandle.ToUniTask();

                if(checkForContentUpdateHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (catalogsToUpdate.Count > 0)
                    {
                        UpdateCatalogs(catalogsToUpdate).Forget();
                    }
                    else
                    {
                        Debug.Log($"{LOG_NAME} No catalog updates available");
                    }
                }
                else
                {
                    Debug.LogError($"{LOG_NAME} Failed to check for updates: {checkForContentUpdateHandle.OperationException}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{LOG_NAME} Failed at Addressable Content Update: {ex.Message}");
            }
        }

        private async UniTaskVoid UpdateCatalogs(List<string> catalogsToUpdate)
        {
            try
            {
                Debug.Log($"{LOG_NAME} Found {catalogsToUpdate.Count} catalog(s) to update");

                var updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate, autoReleaseHandle: true);
                await updateHandle.ToUniTask();

                if (updateHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"{LOG_NAME} Catalogs updated successfully");
                }
                else
                {
                    Debug.LogError($"{LOG_NAME} Failed to update catalogs: {updateHandle.OperationException}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{LOG_NAME} Failed at Updating catalogs: {ex.Message}");
            }
           
        }
        #endregion

        #region Asset Loading
        /// <summary>
        /// Load an assets asynchronously by its address.
        /// </summary>
        public async UniTask<T> LoadAssetAsync<T>(string address) where T : Object
        {
            // Check if we already have a handle for this address
            if (_loadedAssets.TryGetValue(address, out var existingHandle))
            {
                if (existingHandle.IsValid())
                {
                    Debug.Log($"{LOG_NAME} Asset already loaded: {address}");
                    return existingHandle.Convert<T>().Result;
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
                    Debug.Log($"{LOG_NAME} Successfully loaded asset at address: {address}");
                    return asset;
                }
                else
                {
                    Debug.LogError($"{LOG_NAME} Failed to load asset at address: {address}, Error: {handle.OperationException}");
                    Addressables.Release(handle); // Release on failure
                    return null;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{LOG_NAME} Exception while loading asset at address: {address}, Error: {e.Message}");
                return null;
            }
        }
        #endregion

        #region Asset Instantiation
        public async UniTask<GameObject> InstantiateAsync(string address, Transform parent = null)
        {
            return await InstantiateAsync(address, Vector3.zero, Quaternion.identity, parent);
        }

        public async UniTask<GameObject> InstantiateAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            try
            {
                var handle = Addressables.InstantiateAsync(address, position, rotation, parent);
                GameObject instance = await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"{LOG_NAME} Successfully instantiated: {address}");
                    return instance;
                }
                else
                {
                    Debug.LogError($"{LOG_NAME} Failed to instantiate: {address}, Error: {handle.OperationException}");
                    Addressables.Release(handle);
                    return null;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{LOG_NAME} Exception instantiating: {address}, Error: {e.Message}");
                return null;
            }
        }
        #endregion

        #region Utility Methods
        public async UniTask<bool> IsAssetCached(string address)
        {
            try
            {
                var isAssetCachedHandle = Addressables.GetDownloadSizeAsync(address);
                long downloadSize = await isAssetCachedHandle.ToUniTask();

                Addressables.Release(isAssetCachedHandle);

                bool isCached = downloadSize == 0;
                Debug.Log($"{LOG_NAME} Asset '{address}' cached: {isCached} (Download size: {downloadSize} bytes)");

                return isCached;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{LOG_NAME} Error checking cache for '{address}': {e.Message}");
                return false; // Assume not cached on error
            }
        }
        #endregion

        #region Clear Addressables Cache
#if ODIN_INSPECTOR
        [BoxGroup("Clean Addressable By Name", centerLabel: true)]
#endif
        [SerializeField] private string _address;

#if ODIN_INSPECTOR
        [BoxGroup("Clean Addressable By Name", centerLabel: true)]
        [Button("Clean Cache", Style = ButtonStyle.CompactBox)]
#endif
        public void CleanCacheByAddressEditor()
        {
            CleanAddressableCache(_address).Forget();
        }

        /// <summary>
        /// Cleans Addressable Cache for a specific address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async UniTaskVoid CleanAddressableCache(string address)
        {
            try
            {
                var clearCache = Addressables.ClearDependencyCacheAsync(address, autoReleaseHandle: true);
                Debug.Log($"{LOG_NAME} Cleared {address}");
                await clearCache.ToUniTask();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{LOG_NAME} Exception Clearing Addressable Cache: {address}, Error: {ex.Message}");
            }
        }
        #endregion

        #region Asset Release
        /// <summary>
        /// Release a loaded asset by address.
        /// Decrements the ref count. Asset unloads from memory when ref count reaches 0.
        /// </summary>
        public void ReleaseAsset(string address)
        {
            if (_loadedAssets.TryGetValue(address, out var handle))
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                    Debug.Log($"{LOG_NAME} Released: {address}");
                }
                _loadedAssets.Remove(address);
            }
            else
            {
                Debug.LogWarning($"{LOG_NAME} No loaded asset found for: {address}");
            }
        }

        /// <summary>
        /// Release all tracked assets. Useful for scene cleanup.
        /// </summary>
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
            Debug.Log($"{LOG_NAME} Released all assets");
        }
        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ReleaseAll();
        }
    }
}

public class OnAddressableInitialized : ASignal
{

}

public class OnAddressableInitializationFailed : ASignal
{

}