using FIO.ModularAddressableSystem;
using System;
using VContainer;
using VContainer.Unity;

public class GamesceneLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        ResolveAddressableDependency(builder);

        RegisterUICameraDependency(builder);

        ResolveFeedbackDependency(builder);

        //ResolveUICameraDependency(builder);
    }

    private void RegisterUICameraDependency(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<UICamera>();
    }

    private void ResolveUICameraDependency(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<AttachUICamera>();
    }

    private void ResolveFeedbackDependency(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<ButtonOnClickSFX>();
    }

    private void ResolveAddressableDependency(IContainerBuilder builder)
    {
        //Resolve Addressable Dependencies
        builder.RegisterComponentInHierarchy<TestScript>();
        //builder.RegisterComponentInHierarchy<TestScriptOne>();
    }
}
