using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using TNVS.SimpleInjector.Modularization.Abstractions;

namespace TNVS.SimpleInjector.Modularization
{
    internal class SimpleInjectorServiceRegistry : IServiceRegistry
    {
        private readonly Container _container;
        private readonly IReadOnlyList<Type> _typesToForceCollectionFor;
        private readonly List<(Type ServiceType, LifestyleType LifestyleType, Registration Registration)> _registrations = new List<(Type, LifestyleType, Registration)>();

        public SimpleInjectorServiceRegistry(Container container, IReadOnlyList<Type> typesToForceCollectionFor)
        {
            _container = container;
            _typesToForceCollectionFor = typesToForceCollectionFor;
        }

        public void RegisterSingleton<T>() where T : class
        {
            _registrations.Add((typeof(T), LifestyleType.Singleton, Lifestyle.Singleton.CreateRegistration<T>(_container)));
        }

        public void RegisterSingleton<T>(T instance) where T : class
        {
            _registrations.Add((typeof(T), LifestyleType.Singleton, Lifestyle.Singleton.CreateRegistration(() => instance, _container)));
        }

        public void RegisterSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            _registrations.Add((typeof(TService), LifestyleType.Singleton, Lifestyle.Singleton.CreateRegistration<TImplementation>(_container)));
        }

        public void RegisterSingleton<T>(Func<T> instanceProducer) where T : class
        {
            _registrations.Add((typeof(T), LifestyleType.Singleton, Lifestyle.Singleton.CreateRegistration(instanceProducer, _container)));
        }

        public void RegisterTransient<T>() where T : class
        {
            _registrations.Add((typeof(T), LifestyleType.Transient, Lifestyle.Transient.CreateRegistration<T>(_container)));
        }

        public void RegisterTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            _registrations.Add((typeof(TService), LifestyleType.Transient, Lifestyle.Transient.CreateRegistration<TImplementation>(_container)));
        }

        public void RegisterSingletonAsMultipleServices<TService1, TService2, TImplementation>() where TService1 : class where TImplementation : class, TService1, TService2
        {
            var registration = Lifestyle.Singleton.CreateRegistration<TImplementation>(_container);
            _registrations.Add((typeof(TService1), LifestyleType.Singleton, registration));
            _registrations.Add((typeof(TService2), LifestyleType.Singleton, registration));
        }

        public void RegisterSingletonAsMultipleServices<TService1, TService2, TService3, TImplementation>() where TService1 : class where TService2 : class where TService3 : class where TImplementation : class, TService1, TService2, TService3
        {
            var registration = Lifestyle.Singleton.CreateRegistration<TImplementation>(_container);
            _registrations.Add((typeof(TService1), LifestyleType.Singleton, registration));
            _registrations.Add((typeof(TService2), LifestyleType.Singleton, registration));
            _registrations.Add((typeof(TService3), LifestyleType.Singleton, registration));
        }

        public void AddCollectedRegistrationsToContainer()
        {
            foreach (var group in _registrations.GroupBy(r => r.ServiceType))
            {
                var registrations = group.ToList();
                if (registrations.Count == 1 && !_typesToForceCollectionFor.Contains(registrations.Single().ServiceType))
                {
                    var registration = registrations.Single();
                    _container.AddRegistration(registration.ServiceType, registration.Registration);
                }
                else
                {
                    var usedLifestyleTypes = registrations.GroupBy(r => r.LifestyleType).ToList();
                    if (usedLifestyleTypes.Count > 1)
                    {
                        throw new IncompatibleLifestyleException($"Service registrations for service type '{registrations.First().ServiceType}' use more than one lifestyle: {usedLifestyleTypes.Aggregate("", (agg, lifestyleTypeGroup) => $"{agg}, {lifestyleTypeGroup.First().LifestyleType}: {string.Join(", ", lifestyleTypeGroup.Select(gm => gm.Registration.ImplementationType))}")}");
                    }

                    var usedLifeStyleType = usedLifestyleTypes.Single().Key;
                    var serviceType = registrations[0].ServiceType;
                    _container.RegisterCollection(serviceType, registrations.Select(r => r.Registration));
                }
            }
        }

        private enum LifestyleType
        {
            Singleton,
            Transient,
            Scoped
        }
    }
}
