using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FIO.ModularAddressableSystem
{
    public class CacheManager : ICacheManager
    {
        public async UniTask CleanAddressableCache(string address)
        {
            try
            {
                var clearCache = Addressables.ClearDependencyCacheAsync(address, autoReleaseHandle: true);
                await clearCache.ToUniTask();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Exception Clearing Addressable Cache: {address}, Error: {ex.Message}");
            }
        }

        public async UniTask<bool> IsAssetCached(string address)
        {
            try
            {
                var isAssetCachedHandle = Addressables.GetDownloadSizeAsync(address);
                long downloadSize = await isAssetCachedHandle.ToUniTask();

                Addressables.Release(isAssetCachedHandle);

                bool isCached = downloadSize == 0;
                Debug.Log($"Asset '{address}' cached: {isCached} (Download size: {downloadSize} bytes)");

                return isCached;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error checking cache for '{address}': {e.Message}");
                return false; // Assume not cached on error
            }
        }
    }
}

