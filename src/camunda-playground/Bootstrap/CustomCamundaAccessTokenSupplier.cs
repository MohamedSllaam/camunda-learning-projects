using Duende.AccessTokenManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Zeebe.Client.Api.Builder;

namespace camunda_playground.Bootstrap;
internal class CustomCamundaAccessTokenSupplier(IClientCredentialsTokenManagementService service) : IAccessTokenSupplier
{
    public async Task<string> GetAccessTokenForRequestAsync(string authUri = null,
        CancellationToken cancellationToken = new())
    {
        var token = await service.GetAccessTokenAsync("zb-client", cancellationToken: cancellationToken);
        if (token.AccessToken is null)
        {
            throw new AuthenticationException("Failed to get access token");
        }

        return token.AccessToken;
    }
}
