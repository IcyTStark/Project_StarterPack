using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FIO.ModularAddressableSystem
{
    public interface IAssetLoader
    {
        public UniTask<T> LoadAssetAsync<T>(string address) where T : UnityEngine.Object;

        public void ReleaseAsset(string address);

        public void ReleaseAll();
    }
} 