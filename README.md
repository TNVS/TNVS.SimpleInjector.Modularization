# TNVS.SimpleInjector.Modularization

[![NuGet](https://img.shields.io/nuget/v/TNVS.SimpleInjector.Modularization.svg?label=TNVS.SimpleInjector.Modularization)](https://www.nuget.org/packages/TNVS.SimpleInjector.Modularization/)

[![NuGet](https://img.shields.io/nuget/v/TNVS.SimpleInjector.Modularization.Abstractions.svg?label=TNVS.SimpleInjector.Modularization.Abstractions)](https://www.nuget.org/packages/TNVS.SimpleInjector.Modularization.Abstractions/)

Simple modularization for [SimpleInjector](https://simpleinjector.org/index.html).

While SimpleInjector is an awesome DI/IoC framework, it does not come with out-of-the-box support for modularization, i.e. registering services from different assemblies (potentially loaded at startup, such as plugins).
This library aims to make it easy to handle such cases.

## Get Started

In a dependent assembly, reference [TNVS.SimpleInjector.Modularization.Abstractions](https://www.nuget.org/packages/TNVS.SimpleInjector.Modularization.Abstractions/) and define a simple class that implements the `IModule` interface and that has a public parameterless constructor. Here, register all services from that assembly. There are multiple overloads for different lifestyles, as usual for SimpleInjector (work for scoped lifestyle is ongoing).
This example shows some of the possibilities:

```csharp
using TNVS.SimpleInjector.Modularization.Abstractions;

public class RepositoryModule : IModule
{
    public void RegisterServices(IServiceRegistry registry)
    {
        registry.RegisterSingleton<IWhateverRepository, WhateverRepository>();
        registry.RegisterSingleton(() => new DirectlyCreatedService());
        registry.RegisterSingletonAsMultipleServices<IService1, IService2, IService3, ServiceImplementation>();
        
        registry.RegisterTransient<ISomeScopedService, SomeScopedService>();
    }
}
```

Then, in your host assembly, reference [TNVS.SimpleInjector.Modularization](https://www.nuget.org/packages/TNVS.SimpleInjector.Modularization/) and, while creating the container, use the `AddModulesFromLoadedAssemblies()` extension method to call all `IModule`s in all loaded assemblies and let them register their services, like so:

```csharp
// e.g. in your Startup class
using TNVS.SimpleInjector.Modularization;

private readonly Container _container;

private void InitializeContainer()
{
  _container = new Container();
  _container.AddModulesFromLoadedAssemblies();
  _container.Verify();
}
```

And done, that's it :)

## Things to note

- Since SimpleInjector throws an exception when registering multiple service implementations without using collection registration, the ServiceRegistry in this library collects all registrations first and then registers them to the container. If multiple implementations are found, they will be registered as a collection.
- Due to this grouping, you can not register services to the container directly that may have other implementations in modules. We recommend to only use `IModule` implementations to register services.
- If multiple implementations are registered with different lifestyles (e.g. one as singleton and one as transient), the library will throw an `IncompatibleLifestyleException` describing the conflict.
- There is an overload of `AddModulesFromLoadedAssemblies()` that takes a `ModuleRegistrationOptions` instance. With the options, you can define filter callbacks to not load some modules or services. You can also define service types that should always be registered using SimpleInjector's `RegisterCollection` methods, even if only one implementation is registered (this may be required for plugin architectures, where the number of implementations is not known beforehand).
- The `AssemblyLoadUtilities` class provides methods to make it easier to load plugin assemblies from a folder at runtime:
  ```csharp
  var searchPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
  AssemblyLoadUtilities.LoadAssembliesFromFolder(searchPath);
  // ...
  _container.AddModulesFromLoadedAssemblies(); // will include all assemblies loaded from the folder
  ```
## TODOs

- Support for scoped lifestyle
- Tests
- Documentation
