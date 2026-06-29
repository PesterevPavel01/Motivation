using Calabonga.AspNetCore.AppDefinitions;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefinitions(typeof(Program));

var app = builder.Build();

app.UseDefinitions();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (CryptographicException exeption)
    {
        throw;
    }
});

app.Run();