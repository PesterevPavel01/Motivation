using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Motivation.AnaliticsCore.SyncService.Definitions.Authorizations
{
    public static class AuthData
    {
        //public const string AuthSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
        public const string AuthSchemes = JwtBearerDefaults.AuthenticationScheme;
    }
}
