namespace NServiceBus.ContainerTests
{
    using System;
    using LightInject.NServiceBus;
    using NServiceBus.ObjectBuilder.Common;

    public static class TestContainerBuilder
    {
        //public static Func<IContainer> ConstructBuilder = () => (IContainer)Activator.CreateInstance(Type.GetType("NServiceBus.ObjectBuilder.Autofac.AutofacObjectBuilder,NServiceBus.Core"));
        public static Func<IContainer> ConstructBuilder = () => new LightInjectObjectBuilder();
    }
}