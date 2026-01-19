using camunda_playground.Bootstrap;
using camunda_playground.Config;
using camunda_playground.Interfaces;
using camunda_playground.Services;
using System.Reflection;
using Zeebe.Client;
using Zeebe.Client.Accelerator.Extensions;
using Zeebe.Client.Accelerator.Options;
using Zeebe.Client.Api.Builder;

namespace camunda_playground;


public static class CamundaEnvironmentConfig
{
    private const string ZeebeAddressEnvVar = "ZEEBE_ADDRESS";
    private const string ZeebeClientIdEnvVar = "ZEEBE_CLIENT_ID";
    private const string ZeebeClientSecretEnvVar = "ZEEBE_CLIENT_SECRET";
    private const string ZeebeAuthServerEnvVar = "ZEEBE_AUTHORIZATION_SERVER_URL";

    public static string? GetGatewayAddress() => GetFromEnv(ZeebeAddressEnvVar);
    public static string? GetClientId() => GetFromEnv(ZeebeClientIdEnvVar);
    public static string? GetClientSecret() => GetFromEnv(ZeebeClientSecretEnvVar);
    public static string? GetAuthServer() => GetFromEnv(ZeebeAuthServerEnvVar);

    private static string? GetFromEnv(string key)
    {
        char[] trimChars = [' ', '\''];
        return Environment.GetEnvironmentVariable(key)?.Trim(trimChars);
    }
}

public static class DependencyInjection
{
    public static WebApplicationBuilder AddCamunda(this WebApplicationBuilder builder, Assembly assembly, bool bootstrap = false)
    {
        //var _isReadOnly = builder.Configuration.GetValue<bool>("IsReadOnlyInstance", false);
        
            builder.Services.AddScoped<ICamundaService, CamundaService>();

            var zeebeOptions = builder.Configuration.GetSection(ZeebeConfiguration.Key).Get<ZeebeClientAcceleratorOptions?>();

            var gatewayAddress = CamundaEnvironmentConfig.GetGatewayAddress() ?? zeebeOptions?.Client?.GatewayAddress;

            var authUrl = CamundaEnvironmentConfig.GetAuthServer() ?? zeebeOptions?.Client?.Cloud?.AuthorizationServerUrl;
            var clientId = CamundaEnvironmentConfig.GetClientId() ?? zeebeOptions?.Client?.Cloud?.ClientId;
            var clientSecret = CamundaEnvironmentConfig.GetClientSecret() ?? zeebeOptions?.Client?.Cloud?.ClientSecret;

            var isCloud = authUrl != null && clientId != null && clientSecret != null;
            if (isCloud)
            {
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddClientCredentialsTokenManagement(options =>
                {
                    options.CacheLifetimeBuffer = 120;
                }).AddClient("zb-client", client =>
                {
                    client.TokenEndpoint = authUrl;
                    client.ClientId = clientId;
                    client.ClientSecret = clientSecret;
                });
                builder.Services.AddScoped<IAccessTokenSupplier, CustomCamundaAccessTokenSupplier>();
            }

            if (!bootstrap)
            {
                if (isCloud)
                {
                    builder.Services.AddScoped<IZeebeClient>((sp) =>
                    {
                        var client = Zeebe.Client.ZeebeClient.Builder()
                                                             .UseGatewayAddress(gatewayAddress)
                                                             .UseTransportEncryption()
                                                             .UseAccessTokenSupplier(sp.GetService<IAccessTokenSupplier>())
                                                             .Build();
                        return client;
                    });
                }
                else
                {
                    builder.Services.AddScoped<IZeebeClient>((sp) =>
                    {
                        var client = Zeebe.Client.ZeebeClient.Builder()
                                                             .UseGatewayAddress(gatewayAddress)
                                                             .UsePlainText()
                                                             .Build();
                        return client;
                    });
                }
            }
            else
            {
                builder.Services.BootstrapZeebe(
                    builder.Configuration.GetSection(ZeebeConfiguration.Key),
                    (options) =>
                    {
                        options.Client.GatewayAddress = gatewayAddress;
                        if (isCloud)
                        {
                            options.Client.Cloud = new ZeebeClientAcceleratorOptions.ClientOptions.CloudOptions
                            {
                                AuthorizationServerUrl = authUrl,
                                ClientId = clientId,
                                ClientSecret = clientSecret,
                            };
                        }
                    },
                    assembly
                );
            }
        
        return builder;
    }
}