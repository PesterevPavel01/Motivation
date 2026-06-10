namespace Motivation.IdentityServer.Domain.Base
{
    /// <summary>
    /// Static data container
    /// </summary>
    public static partial class AppData
    {
        /// <summary>
        /// Current service name
        /// </summary>
        public const string ServiceName = "Identity service";

        /// <summary>
        /// Microservice Template with integrated OpenIddict
        /// for OpenID Connect server and Token Validation
        /// </summary>
        public const string ServiceDescription = "OAuth2/OpenID Connect provider built with OpenIddict";

        /// <summary>
        /// Default policy name for CORS
        /// </summary>
        public const string PolicyCorsName = "CorsPolicy";

        /// <summary>
        /// Default policy name for API
        /// </summary>
        public const string PolicyDefaultName = "DefaultPolicy";

        /// <summary>
        /// Policy name for synchronization access
        /// </summary>
        public const string PolicySyncAccess = "SyncAccessPolicy";

        /// <summary>
        /// Default scope name for API
        /// </summary>
        public const string ScopeApi = "api";

        /// <summary>
        /// Scope name for synchronization
        /// </summary>
        public const string ScopeSync = "sync";

        /// <summary>
        /// Default audience for API
        /// </summary>
        public const string AudienceApi = "MotivationApi";

        /// <summary>
        /// "SystemAdministrator"
        /// </summary>
        public const string SystemAdministratorRoleName = "Administrator";

        /// <summary>
        /// "BusinessOwner"
        /// </summary>
        public const string ManagerRoleName = "Manager";


        /// <summary>
        /// Roles
        /// </summary>
        public static IEnumerable<string> Roles
        {
            get
            {
                yield return SystemAdministratorRoleName;
                yield return ManagerRoleName;
            }
        }
    }
}
