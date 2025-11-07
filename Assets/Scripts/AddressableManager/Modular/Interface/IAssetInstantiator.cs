using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FIO.ModularAddressableSystem
{
    public interface IAssetInstantiator
    {
        public UniTask<GameObject> InstantiateAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null);
    }
}
