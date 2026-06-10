using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.IdentityServer.Web.Application.Messaging.EventItemMessages.ViewModels;
using Motivation.IdentityServer.Web.Definitions.Mediator.Base;

namespace Motivation.IdentityServer.Web.Definitions.Mediator
{
    public class EventItemPostTransactionBehavior(IUnitOfWork unitOfWork)
        : TransactionBehavior<IRequest<Operation<EventItemViewModel>>, Operation<EventItemViewModel>>(unitOfWork);
}
