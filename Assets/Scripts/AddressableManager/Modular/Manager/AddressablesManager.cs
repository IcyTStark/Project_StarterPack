#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using Cysharp.Threading.Tasks;
using deVoid.Utils;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using VContainer;

using UnityEngine;

namespace FIO.ModularAddressableSystem
{
    public class AddressablesManager
    {
        private IInitializeAddressable addressableInitializer;
        private ICatalogContentUpdater catalogContentUpdater;
        private IAssetLoader assetLoader;
        private IAssetInstantiator assetInstantiator;
        private ICacheManager cacheManager;

        private bool isInitialized = false;
        public bool IsInitialized => isInitialized;

        public AddressablesManager(
            IInitializeAddressable addressableInitializer, 
            ICatalogContentUpdater catalogContentUpdater,
            IAssetLoader assetLoader,
            IAssetInstantiator assetInstantiator,
            ICacheManager cacheManager)
        {
            this.addressableInitializer = addressableInitializer;
            this.catalogContentUpdater = catalogContentUpdater;
            this.assetLoader = assetLoader;
            this.assetInstantiator = assetInstantiator;
            this.cacheManager = cacheManager;
            SmartDebug.Log($"All Addressable Manager Dependency Injected 😁", "ADDRESSABLEMANAGER");
        }

        public async UniTask StartInitialization()
        {
            isInitialized = await addressableInitializer.InitializeAddressableAsync();

            SmartDebug.Log($"Is Addressable Initialized: {isInitialized}", "ADDRESSABLEMANAGER");

            if (isInitialized)
            {
                await catalogContentUpdater.CheckForCatalogUpdatesAsync();

                Signals.Get<OnAddressableInitialized>().Dispatch();
            }
            else
            {
                Signals.Get<OnAddressableInitializationFailed>().Dispatch();
            }
        }

        public async UniTask<T> LoadAssetAsync<T>(string address) where T : Object
        {
            if(isInitialized)
                return await assetLoader.LoadAssetAsync<T>(address);
            else
                return null;
        }

        public async UniTask<GameObject> InstantiateAssetAsync(string address)
        {
            if (isInitialized)
                return await assetInstantiator.InstantiateAsync(address, Vector3.zero, Quaternion.identity);
            else
                return null;
        }

        public async UniTask<GameObject> InstantiateAssetAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (isInitialized)
                return await assetInstantiator.InstantiateAsync(address, position, rotation, parent);
            else
                return null;
        }

        public void ReleaseAsset(string address)
        {
            assetLoader.ReleaseAsset(address);
        }

        public void ReleaseAllAssets()
        {
            assetLoader.ReleaseAll();
        }

        public async UniTask ClearCacheAsync(string address)
        {
            await cacheManager.CleanAddressableCache(address);
        }

        public async UniTask<bool> IsAssetCachedAsync(string address)
        {
            return await cacheManager.IsAssetCached(address);
        }
    }
}