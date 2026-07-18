using Calabonga.AspNetCore.AppDefinitions;
using Motivation.HRWeb.Application.Clients;
using Motivation.HRWeb.Application.Validators;
using Motivation.HRWeb.Components;
using Motivation.HRWeb.Components.Pages.EmployeesPage.ViewModels;
using Motivation.HRWeb.Components.Pages.ExtraPartsAndDeductions.ViewModels;
using Motivation.HRWeb.Components.Pages.HomePage.ViewModels;
using Motivation.HRWeb.Components.Pages.KpiPage.ViewModels;
using Motivation.HRWeb.Components.Pages.TargetPage.ViewModels;
using Motivation.HRWeb.Components.Pages.TimeSheet.ViewModels;
using Motivation.HRWeb.Extensions;

namespace Motivation.HRWeb.Definitions.Common;

public class CommonDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });

            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        app.MapGroup("/authentication").MapLoginAndLogout();
    }

    /// <summary>
    /// Configure services for current microservice
    /// </summary>
    /// <param name="builder"></param>
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents(options =>
            {
                options.DetailedErrors = true;
            });

        builder.Services.AddAuthorization();
        builder.Services.AddScoped<EmployeeApiClient>();
        builder.Services.AddScoped<EmployeesViewModel>();
        builder.Services.AddScoped<FilterViewModel>();
        builder.Services.AddScoped<CreateEmployeeModalViewModel>();
        builder.Services.AddScoped<EditEmployeeModalViewModel>();
        builder.Services.AddScoped<DeleteEmployeeModalViewModel>();
        builder.Services.AddScoped<AssignEmployeeModalViewModel>();
        builder.Services.AddScoped<DismissEmployeeModalViewModel>();
        builder.Services.AddScoped<PositionApiClient>();
        builder.Services.AddScoped<StandardHoursApiClient>();
        builder.Services.AddScoped<KpiApiClient>();
        builder.Services.AddScoped<TimeSheetViewModel>();
        builder.Services.AddScoped<ExtraPartAndDeductionViewModel>();
        builder.Services.AddScoped<EditExtraPartModalViewModel>();
        builder.Services.AddScoped<KpiViewModel>();
        builder.Services.AddScoped<EditKpiViewModel>();
        builder.Services.AddScoped<TargetsViewModel>();
        builder.Services.AddScoped<HomeViewModel>();
        builder.Services.AddScoped<MeasurementUnitApiClient>();
        builder.Services.AddScoped<IKpiValidator,KpiValidator>();
        builder.Services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        builder.Services.AddHttpClient();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddHttpContextAccessor();
    }
}