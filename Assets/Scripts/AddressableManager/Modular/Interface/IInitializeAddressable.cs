using Cysharp.Threading.Tasks;

namespace FIO.ModularAddressableSystem
{
    public interface IInitializeAddressable
    {
        public UniTask InitializeAddressableAsync();
    }
}