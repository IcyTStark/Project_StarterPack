using UnityEngine;

using Cysharp.Threading.Tasks;

using UnityEngine.AddressableAssets;
using VContainer;

namespace FIO.ModularAddressableSystem
{
    public class AddressableInitializer : IInitializeAddressable
    {
        public async UniTask InitializeAddressableAsync()
        {
            try
            {
                Debug.Log("Initializing Addressables...");

                // This initializes Unity's Addressables system
                var handle = Addressables.InitializeAsync();
                await handle.ToUniTask(); // Convert to UniTask

                Debug.Log("Addressables initialized successfully!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to initialize Addressables: {ex.Message}");
                throw;
            }
        }
    }
}
