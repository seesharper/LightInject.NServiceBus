using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightInject.NServiceBus
{
    using System.CodeDom;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using global::NServiceBus;
    using global::NServiceBus.ObjectBuilder.Common;
    public class LightInjectObjectBuilder : IContainer
    {
        private IServiceContainer serviceContainer = new ServiceContainer(new ContainerOptions() {EnableVariance = false});
        
        private Scope scope;

        private bool isRootScope;


        public LightInjectObjectBuilder()
        {
            serviceContainer.ScopeManagerProvider = new PerThreadScopeManagerProvider();
            scope = serviceContainer.BeginScope();
            isRootScope = true;
        }

        public LightInjectObjectBuilder(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer;
            scope = serviceContainer.BeginScope();
            isRootScope = false;
        }     

        public void Dispose()
        {
            scope.Dispose();
            if (isRootScope)
            {
                serviceContainer.Dispose();
            }
        }

        public object Build(Type typeToBuild)
        {
            return serviceContainer.GetInstance(typeToBuild);
        }

        public IContainer BuildChildContainer()
        {
            return new LightInjectObjectBuilder(serviceContainer);
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return serviceContainer.GetAllInstances(typeToBuild);
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {           
            serviceContainer.Register(component, GetLifeTime(dependencyLifecycle));
            var interfaces = component.GetInterfaces();
            foreach (var serviceType in interfaces)
            {
                serviceContainer.Register(serviceType, () => serviceContainer.GetInstance(component), GetLifeTime(dependencyLifecycle), component.FullName);
            }
        }
       
        public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
        {
            serviceContainer.Register<T>(factory => component(), GetLifeTime(dependencyLifecycle));
            var servicesTypes = typeof(T).GetInterfaces();
            
            foreach (var servicesType in servicesTypes)
            {
               serviceContainer.Register(servicesType, () => serviceContainer.GetInstance<T>(), GetLifeTime(dependencyLifecycle), typeof(T).FullName);
            }
            
        }


        /// <summary>
        /// Sets the value to be configured for the given property of the 
        ///             given component type.
        /// </summary>
        /// <param name="component">The interface type.</param><param name="property">The property name to be injected.</param><param name="value">The value to assign to the <paramref name="property"/>.</param>
        public void ConfigureProperty(Type component, string property, object value)
        {
            serviceContainer.Initialize(sr => sr.ServiceType == component, (factory, instance) => component.GetProperty(property).SetValue(instance, value));
        }

        private ILifetime GetLifeTime(DependencyLifecycle dependencyLifecycle)
        {
            if (dependencyLifecycle == DependencyLifecycle.SingleInstance)
            {
                return new PerContainerLifetime();
            }
            if (dependencyLifecycle == DependencyLifecycle.InstancePerUnitOfWork)
            {
                return new PerScopeLifetime();
            }
            return null;
        }


        /// <summary>
        /// Registers the given instance as the singleton that will be returned for the given type.
        /// </summary>
        /// <param name="lookupType">The interface type.</param><param name="instance">The implementation instance.</param>
        public void RegisterSingleton(Type lookupType, object instance)
        {
            var serviceRegistration = new ServiceRegistration();
            serviceRegistration.ServiceType = lookupType;
            serviceRegistration.ServiceName = string.Empty;
            serviceRegistration.Lifetime = new PerContainerLifetime();
            serviceRegistration.Value = instance;
            serviceContainer.Register(serviceRegistration);
        }

        public bool HasComponent(Type componentType)
        {
            return serviceContainer.CanGetInstance(componentType, string.Empty);
        }

        public void Release(object instance)
        {
            //throw new NotImplementedException();
        }
    }
   
    public static class ContainerExtensions
    {
        public static void Register(this IServiceRegistry registry, Type serviceType, Func<object> factoryDelegate, ILifetime lifetime, string serviceName)
        {
            var invokeExpression = Expression.Invoke(Expression.Constant(factoryDelegate));
            var castExpression = Expression.Convert(invokeExpression, serviceType);
            var lambdaExpression = Expression.Lambda(castExpression,Expression.Parameter(typeof(IServiceFactory),"factory"));
            var lambda = lambdaExpression.Compile();
            var serviceRegistration = new ServiceRegistration();
            serviceRegistration.ServiceType = serviceType;
            serviceRegistration.ServiceName = serviceName;
            serviceRegistration.FactoryExpression = lambda;
            serviceRegistration.Lifetime = lifetime;
            registry.Register(serviceRegistration);
        }
    }
}
