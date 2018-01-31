using System;

namespace TNVS.SimpleInjector.Modularization.Abstractions
{
    public interface IServiceRegistry
    {
        void RegisterSingleton<T>() where T : class;

        void RegisterSingleton<T>(T instance) where T : class;

        void RegisterSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService;

        void RegisterSingleton<T>(Func<T> instanceProducer) where T : class;

        void RegisterSingletonAsMultipleServices<TService1, TService2, TImplementation>() where TService1 : class where TImplementation : class, TService1, TService2;

        void RegisterSingletonAsMultipleServices<TService1, TService2, TService3, TImplementation>() where TService1 : class where TService2 : class where TService3 : class where TImplementation : class, TService1, TService2, TService3;

        void RegisterTransient<T>() where T : class;

        void RegisterTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService;
    }
}
