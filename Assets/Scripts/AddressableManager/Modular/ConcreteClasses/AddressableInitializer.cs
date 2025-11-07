using Cysharp.Threading.Tasks;
using deVoid.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace FIO.ModularAddressableSystem
{
    public class AddressableInitializer : IInitializeAddressable
    {
        public async UniTask<bool> InitializeAddressableAsync()
        {
            try
            {
                var initHandle = Addressables.InitializeAsync();
                await initHandle.ToUniTask();

                if (initHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    return true;
                }

                Addressables.Release(initHandle);
                return false;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
    }
}
