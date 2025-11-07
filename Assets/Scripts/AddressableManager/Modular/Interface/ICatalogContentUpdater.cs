using Cysharp.Threading.Tasks;

namespace FIO.ModularAddressableSystem
{
    public interface ICatalogContentUpdater
    {
        public UniTask CheckForCatalogUpdatesAsync();
    }
}