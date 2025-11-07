using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FIO.ModularAddressableSystem
{
    public class CatalogContentUpdater : ICatalogContentUpdater
    {
        public async UniTask CheckForCatalogUpdatesAsync()
        {
            try
            {
                var checkForContentUpdateHandle = Addressables.CheckForCatalogUpdates(autoReleaseHandle: true);
                var catalogsToUpdate = await checkForContentUpdateHandle.ToUniTask();

                if (checkForContentUpdateHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (catalogsToUpdate.Count > 0)
                    {
                        await UpdateCatalogs(catalogsToUpdate);
                    }
                    else
                    {

                    }
                }
                else
                {
                    Debug.LogError($"Failed to check for updates: {checkForContentUpdateHandle.OperationException}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed at Addressable Content Update: {ex.Message}");
            }
        }

        private async UniTask UpdateCatalogs(List<string> catalogsToUpdate)
        {
            try
            {
                Debug.Log($"Found {catalogsToUpdate.Count} catalog(s) to update");

                var updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate, autoReleaseHandle: true);
                await updateHandle.ToUniTask();

                if (updateHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Catalogs updated successfully");
                }
                else
                {
                    Debug.LogError($"Failed to update catalogs: {updateHandle.OperationException}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed at Updating catalogs: {ex.Message}");
            }

        }
    }
}