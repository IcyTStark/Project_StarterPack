using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T service) 
    {
        services[typeof(T)] = service;
    }

    public static T GetService<T>()
    {
        if (services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        return default;
    }

    public static bool HasService<T>()
    {
        return services.ContainsKey(typeof(T));
    }

    public static void Clear()
    {
        services.Clear();
    }
}
