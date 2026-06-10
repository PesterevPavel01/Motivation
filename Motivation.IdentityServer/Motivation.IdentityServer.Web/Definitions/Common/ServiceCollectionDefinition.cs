using Calabonga.AspNetCore.AppDefinitions;
using Motivation.IdentityServer.Domain.Interfaces;
using Motivation.IdentityServer.Infrastructure;
using Motivation.IdentityServer.Infrastructure.Configurations.IdentityServerClients;
using OpenIddict.Abstractions;

namespace Motivation.IdentityServer.Web.Definitions.Common;

public class ServiceCollectionDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IIdentityClientConfiguration<OpenIddictApplicationDescriptor>, MachineToMachineClientConfiguration>();
        builder.Services.AddScoped<IIdentityClientConfiguration<OpenIddictApplicationDescriptor>, SyncMachineToMachineClientConfiguration>();
        builder.Services.AddScoped<IIdentityClientConfiguration<OpenIddictApplicationDescriptor>, UserToServiceClientConfiguration>();
        builder.Services.AddScoped<IIdentityClientProcessor, OpenIddictClientProcessor>();
        
    }

}
