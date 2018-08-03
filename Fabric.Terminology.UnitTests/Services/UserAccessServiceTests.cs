namespace Fabric.Terminology.UnitTests.Services
{
    using System;
    using System.Security.Claims;

    using Catalyst.DosApi.Authorization;
    using Catalyst.DosApi.Authorization.Compliance;
    using Catalyst.DosApi.Authorization.Models;
    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.API.Constants;
    using Fabric.Terminology.API.Infrastructure;
    using Fabric.Terminology.API.Services;
    using Fabric.Terminology.SqlServer;
    using Fabric.Terminology.TestsBase.Fixtures;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using IdentityModel;

    using Moq;

    using Nancy;

    using Xunit;
    using Xunit.Abstractions;

    public class UserAccessServiceTests : IClassFixture<AppConfigurationFixture>
    {
        private const string Subject = "Subject";

        private const string ClientId = "ClientId";

        private readonly AppConfigurationFixture fixture;

        private readonly ITestOutputHelper output;

        private readonly ClaimsPrincipal claimsPrincipal;

        public UserAccessServiceTests(AppConfigurationFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
            var claims = new Claim[]
            {
                new Claim(JwtClaimTypes.Subject, Subject),
                new Claim(Claims.ClientId, ClientId)
            };

            this.claimsPrincipal = new TestPrincipal(claims);
        }

        [Fact]
        public void CanGetSubject()
        {
            // Arrange
            var service = this.BuildUserAccessService();

            // Act
            var subject = service.GetSubject();

            // Assert
            subject.Single().Should().BeEquivalentTo(Subject);
        }

        [Fact]
        public void CanGetClientId()
        {
            // Arrange
            var service = this.BuildUserAccessService();

            // Act
            var subject = service.GetClientId();

            // Assert
            subject.Single().Should().BeEquivalentTo(ClientId);
        }

        [Theory]
        [InlineData("app/authorization.read")]
        [InlineData("termniology/valuesets.read")]
        [InlineData("termniology/valuesets.write")]
        public void CanGenerateCacheKeys(string pn)
        {
            // Arrange
            var permissionName = PermissionName.Create(pn);
            this.output.WriteLine(permissionName.ToString());
            var service = this.BuildUserAccessService();
            var expected = FormattableString.Invariant($"{typeof(AccessPermissions)}.{Subject}.{permissionName}");

            // Act
            var cacheKey = service.GetCacheKey(permissionName);
            this.output.WriteLine(cacheKey);

            // Assert
            cacheKey.Should().BeEquivalentTo(expected);
        }

        private UserAccessService BuildUserAccessService()
        {
            var nancyContextWrapper = new NancyContextWrapper(new NancyContext()
            {
                CurrentUser = this.claimsPrincipal
            });

            var memoryCacheProvider = new MemoryCacheProvider(
                this.fixture.AppConfiguration.TerminologySqlSettings.AsMemoryCacheProviderSettings());

            var userPermissionService = new Mock<IUserPermissionsService>().Object;

            return new UserAccessService(nancyContextWrapper, userPermissionService, memoryCacheProvider);
        }
    }
}
