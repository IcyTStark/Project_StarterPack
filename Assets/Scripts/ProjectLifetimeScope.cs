using FIO.ModularAddressableSystem;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<IInitializeAddressable, AddressableInitializer>(Lifetime.Singleton);
    }
}
