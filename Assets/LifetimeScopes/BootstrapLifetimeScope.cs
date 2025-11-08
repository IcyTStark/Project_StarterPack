using VContainer;
using VContainer.Unity;

using FIO.ModularAddressableSystem;
using TMS.Feedback;

public class BootstrapLifetimeScope : LifetimeScope
{
    private static BootstrapLifetimeScope Instance;

    protected override void Awake()
    {
        base.Awake();

        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        
        Destroy(this.gameObject);
    }

    protected override void Configure(IContainerBuilder builder)
    {
        RegisterAddressableDependency(builder);
        RegisterFeedbackDependency(builder);
    }

    private void RegisterAddressableDependency(IContainerBuilder builder)
    {
        // Register the AddressableDependencyInitializer as an entry point
        builder.RegisterEntryPoint<AddressableDependencyInitializer>();

        //Resgister Addressable Dependencies
        builder.Register<IInitializeAddressable, AddressableInitializer>(Lifetime.Singleton);
        builder.Register<ICatalogContentUpdater, CatalogContentUpdater>(Lifetime.Singleton);
        builder.Register<IAssetLoader, AssetLoader>(Lifetime.Singleton);    
        builder.Register<IAssetInstantiator, AssetInstantiator>(Lifetime.Singleton);
        builder.Register<ICacheManager, CacheManager>(Lifetime.Singleton);  

        //Register AddressableManager
        builder.Register<AddressablesManager>(Lifetime.Singleton);
    }

    private void RegisterFeedbackDependency(IContainerBuilder builder)
    {
        builder.Register<IFeedbackManager, FeedbackManager>(Lifetime.Singleton);
    }
}
