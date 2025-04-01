using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T service) where T : class
    {
        services[typeof(T)] = service;
    }

    public static T GetService<T>() where T : class
    {
        if (services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        return null;
    }

    public static bool HasService<T>() where T : class
    {
        return services.ContainsKey(typeof(T));
    }

    public static void Clear()
    {
        services.Clear();
    }
}