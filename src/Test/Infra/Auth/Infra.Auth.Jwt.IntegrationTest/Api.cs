using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

#pragma warning disable CA1852

namespace Infra.Auth.Jwt.IntegrationTest;

internal class Api : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // shared extra set up goes here
        return base.CreateHost(builder);
    }
}
