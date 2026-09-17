using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Provider.Application.Provider.Interfaces;
using Provider.Application.Provider.Response;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Provider.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace authentication with a test scheme that always succeeds
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                // Replace IProviderService with a test implementation to avoid DB dependencies
                services.AddScoped<IProviderService, TestProviderService>();
            });
        });
    }

    [Fact(Skip = "Integration test skipped in unit test run - enable when environment is configured")]
    public async Task GetAvailable_Returns_OK()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/providers/available");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    // Simple auth handler for tests
    private class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, System.Text.Encodings.Web.UrlEncoder encoder, TimeProvider timeProvider)
            : base(options, logger, encoder, (ISystemClock)timeProvider)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private class TestProviderService : IProviderService
    {
        public Task<IEnumerable<ProviderResponse>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<ProviderResponse>>(new[] { new ProviderResponse(Guid.NewGuid(), "A", 0, 0, 4.9, true, 0.0, 0.0) });

        public Task<ProviderResponse?> OptimizeAsync(double userLat, double userLon, CancellationToken cancellationToken = default)
            => Task.FromResult<ProviderResponse?>(new ProviderResponse(Guid.NewGuid(), "A", 0, 0, 4.9, true, 0.0, 0.0));

        public Task<ProviderResponse?> AssignProviderAsync(Provider.Application.Provider.Requests.AssignProviderRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult<ProviderResponse?>(new ProviderResponse(Guid.NewGuid(), "A", 0, 0, 4.9, true, 0.0, 0.0));
    }
}
