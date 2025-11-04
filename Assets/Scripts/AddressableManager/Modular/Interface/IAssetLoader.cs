using Cysharp.Threading.Tasks;

namespace FIO.ModularAddressableSystem
{
    interface IAssetLoader
    {
        public UniTask<T> LoadAssetAsync<T>(string address);
    }
} 