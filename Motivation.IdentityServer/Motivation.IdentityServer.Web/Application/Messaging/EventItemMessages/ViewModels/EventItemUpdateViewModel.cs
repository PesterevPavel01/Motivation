using Motivation.IdentityServer.Domain.Base;

namespace Motivation.IdentityServer.Web.Application.Messaging.EventItemMessages.ViewModels
{
    public class EventItemUpdateViewModel : ViewModelBase
    {
        public string Logger { get; set; } = null!;

        public string Level { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}