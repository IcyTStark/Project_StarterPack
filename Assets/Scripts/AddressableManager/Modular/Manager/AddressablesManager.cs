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
    public class AddressablesManager : MonoBehaviour
    {
        private IInitializeAddressable addressableInitializer;

        [Inject]
        public void Construct(IInitializeAddressable initializer)
        {
            addressableInitializer = initializer;
            Debug.Log("✅ AddressablesManager dependencies injected");
        }
    }
}