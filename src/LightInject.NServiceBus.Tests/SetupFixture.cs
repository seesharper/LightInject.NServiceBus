namespace LightInject.NServiceBus.Tests
{
    using global::NServiceBus.ContainerTests;
    using NUnit.Framework;

    [SetUpFixture]
    public class SetUpFixture
    {
        [SetUp]
        public void Setup()
        {
            TestContainerBuilder.ConstructBuilder = () => new LightInjectObjectBuilder();
        }
    }
}