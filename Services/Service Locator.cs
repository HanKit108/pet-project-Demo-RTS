using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    public static Dictionary<Type, object> services = new();

    public static void AddService(Type type, object service)
    {
        services[type] = service;
    }

    public static T GetService<T>() where T : class
    {
        return services[typeof(T)] as T;
    }
}
