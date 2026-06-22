using Calabonga.Blazor.AppDefinitions;
using System.Reflection;

namespace Motivation.WebClient.Components
{
    public partial class Routes
    {
        private IEnumerable<Assembly> Assemblies => ModuleDefinitions.Instance.Assemblies;
    }
}