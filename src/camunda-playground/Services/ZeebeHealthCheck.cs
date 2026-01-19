namespace camunda_playground.Services;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zeebe.Client;

public sealed class ZeebeHealthCheck : IHealthCheck
{
    private readonly IZeebeClient _client;

    public ZeebeHealthCheck(IZeebeClient client)
    {
        _client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple lightweight call
            await _client
                .TopologyRequest()
                .Send(cancellationToken);

            return HealthCheckResult.Healthy("Zeebe gateway is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Zeebe gateway is NOT reachable",
                ex);
        }
    }
}
