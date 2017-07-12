using System;
using NServiceBus.ContainerTests;
using NUnit.Framework;

namespace NServiceBus.ContainerTests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        [OneTimeSetUp]
        public void Setup()
        {
            TestContainerBuilder.ConstructBuilder = () => new LightInject.NServiceBus.LightInjectObjectBuilder();
        }
    }
}
