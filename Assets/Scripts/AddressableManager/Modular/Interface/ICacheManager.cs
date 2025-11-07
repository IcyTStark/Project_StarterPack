using Cysharp.Threading.Tasks;

namespace FIO.ModularAddressableSystem
{
    public interface ICacheManager
    {
        public UniTask CleanAddressableCache(string address);

        public UniTask<bool> IsAssetCached(string address);
    }
}
