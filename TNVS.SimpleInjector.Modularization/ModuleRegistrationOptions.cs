using System;
using System.Collections.Generic;
using System.Reflection;

namespace TNVS.SimpleInjector.Modularization
{
    /// <summary>
    /// Provides options for module registration.
    /// </summary>
    public sealed class ModuleRegistrationOptions
    {
        /// <summary>
        /// Service types added here will have their registrations registered as a collection, even if only one registration is available.
        /// </summary>
        public IList<Type> ServiceTypesWithForcedCollectionRegistration { get; } = new List<Type>();

        /// <summary>
        /// This filter predicate can be used to select the modules to register. If the predicate returns false, the module will not be registered.
        /// </summary>
        public Func<Type, bool> ModuleTypeFilter { get; set; }

        /// <summary>
        /// This filter predicate can be used to select the assemblies to load modules from. If the predicate returns false, the assembly will not be searched for modules.
        /// </summary>
        public Func<Assembly, bool> AssemblyFilter { get; set; }
    }
}
