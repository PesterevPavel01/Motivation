using Calabonga.AspNetCore.AppDefinitions;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;

try
{
    Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

    var builder = WebApplication.CreateBuilder(args);

    // ✅ Поддержка замены переменных в appsettings.json
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));
    builder.AddDefinitions(typeof(Program));

    var app = builder.Build();

    app.Use(async (context, next) =>
    {
        Console.WriteLine($"=== BEFORE context.Request.Scheme = https ===");
        Console.WriteLine($"Scheme: {context.Request.Scheme}");
        Console.WriteLine($"X-Forwarded-Proto: {context.Request.Headers["X-Forwarded-Proto"]}");
        Console.WriteLine($"Path: {context.Request.Path}");
        Console.WriteLine("==============================");
        await next();
    });

    app.Use(async (context, next) =>
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-Proto", out var proto) && proto == "https")
        {
            context.Request.Scheme = "https";
        }
        await next();
    });

    app.Use(async (context, next) =>
    {
        Console.WriteLine($"=== BEFORE ForwardedHeaders ===");
        Console.WriteLine($"Scheme: {context.Request.Scheme}");
        Console.WriteLine($"X-Forwarded-Proto: {context.Request.Headers["X-Forwarded-Proto"]}");
        Console.WriteLine($"Path: {context.Request.Path}");
        Console.WriteLine("==============================");
        await next();
    });

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        ForwardedProtoHeaderName = "X-Forwarded-Proto",
        ForwardedHostHeaderName = "X-Forwarded-Host"
    });

    //app.UsePathBase("/identity");

    app.Use(async (context, next) =>
    {
        Console.WriteLine($"=== AFTER ForwardedHeaders ===");
        Console.WriteLine($"Scheme: {context.Request.Scheme}");
        Console.WriteLine($"X-Forwarded-Proto: {context.Request.Headers["X-Forwarded-Proto"]}");
        Console.WriteLine("==============================");
        await next();
    });

    app.UseDefinitions();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.Use(async (context, next) =>
    {
        Console.WriteLine($"Static file request: {context.Request.Path}");
        await next();
    });
    app.UseStaticFiles();
    app.MapRazorPages();
    app.MapDefaultControllerRoute();

    app.Use(async (context, next) =>
    {
        Console.WriteLine($"=== AFTER UseHttpsRedirection and other definitions===");
        Console.WriteLine($"Scheme: {context.Request.Scheme}");
        Console.WriteLine($"X-Forwarded-Proto: {context.Request.Headers["X-Forwarded-Proto"]}");
        Console.WriteLine($"Path: {context.Request.Path}");
        Console.WriteLine("==============================");
        await next();
    });

    app.Run();

    return 0;
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("HostAbortedException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, "Unhandled exception");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
