namespace Fabric.Terminology.UnitTests.IoC
{
    using System;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;

    using FluentAssertions;

    using Nancy.TinyIoc;

    using Swagger.ObjectModel;

    using Xunit;

    public class ContainerResolutionTests : IClassFixture<ContainerFixture>
    {
        public readonly TinyIoCContainer Container;

        public ContainerResolutionTests(ContainerFixture fixture)
        {
            this.Container = fixture.Container;
        }

        [Fact]
        public void CanResolveCachingManagerFactory()
        {
            // Arrange

            var prim = Primitive.IsPrimitive(typeof(Guid));

            // Act
            var factory = this.Container.Resolve<ICachingManagerFactory>();

            var valueSetCodeManager = factory.ResolveFor<IValueSetCode>();
            var valueSetCodeCountManager = factory.ResolveFor<IValueSetCodeCount>();
            var valueSetBackingItemManager = factory.ResolveFor<IValueSetBackingItem>();

            // Assert
            valueSetCodeManager.Should().NotBeNull();
            valueSetCodeCountManager.Should().NotBeNull();
            valueSetBackingItemManager.Should().NotBeNull();
        }
    }
}