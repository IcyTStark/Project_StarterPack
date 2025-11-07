using FIO.ModularAddressableSystem;
using VContainer;
using VContainer.Unity;

public class GamesceneLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        RegisterAddressableDependency(builder);
    }

    private void RegisterAddressableDependency(IContainerBuilder builder)
    {
        //Resolve Addressable Dependencies
        builder.RegisterComponentInHierarchy<TestScript>();
        builder.RegisterComponentInHierarchy<TestScriptOne>();
    }
}
