using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace BullMarket.WebUI.Helpers
{
    public static class AuthorizationHelpers
    {
        public static Dictionary<string, string[]> RetrieveClientRoles(IEnumerable<Claim> claims)
        {
            var clientRoles = claims.FirstOrDefault(claim => claim.Type == "roles");

            if (clientRoles == null || string.IsNullOrEmpty(clientRoles.Value))
            {
                return new Dictionary<string, string[]>();
            }

            return JsonSerializer.Deserialize<Dictionary<string, string[]>>(clientRoles.Value);
        }
    }
}
