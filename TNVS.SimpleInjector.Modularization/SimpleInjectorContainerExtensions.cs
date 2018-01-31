using SimpleInjector;
using System;
using System.Linq;
using TNVS.SimpleInjector.Modularization.Abstractions;

namespace TNVS.SimpleInjector.Modularization
{
    public static class SimpleInjectorContainerExtensions
    {
        /// <summary>
        /// Registers all modules from all currently loaded assemblies to this container.
        /// </summary>
        /// <param name="container">The container to register the modules with.</param>
        public static void AddModulesFromLoadedAssemblies(this Container container)
        {
            AddModulesFromLoadedAssemblies(container, new ModuleRegistrationOptions());
        }

        /// <summary>
        /// Registers all modules from all currently loaded assemblies to this container.
        /// </summary>
        /// <param name="container">The container to register the modules with.</param>
        /// <param name="options"></param>
        public static void AddModulesFromLoadedAssemblies(this Container container, ModuleRegistrationOptions options)
        {
            var assembliesToSearch = AppDomain.CurrentDomain.GetAssemblies().Where(a => options.AssemblyFilter == null || options.AssemblyFilter(a));
            var registry = new SimpleInjectorServiceRegistry(container, options.ServiceTypesWithForcedCollectionRegistration.ToList());
            var moduleTypes = container.GetTypesToRegister(typeof(IModule), assembliesToSearch).Where(mt => options.ModuleTypeFilter == null || options.ModuleTypeFilter(mt));
            foreach (var moduleType in moduleTypes)
            {
                var moduleInstance = (IModule)Activator.CreateInstance(moduleType);
                moduleInstance.RegisterServices(registry);
            }
            registry.AddCollectedRegistrationsToContainer();
        }
    }
}
