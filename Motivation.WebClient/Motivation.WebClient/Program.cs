using Calabonga.Blazor.AppDefinitions;
using Motivation.WebClient.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddBlazorModulesDefinitions("Modules", typeof(App));

var app = builder.Build();

app.UseBlazorDefinitions();

app.Run();
