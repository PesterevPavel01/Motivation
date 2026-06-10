using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.SynchronisationService.Web.Application.Messaging.EventItemMessages.ViewModels;
using Motivation.SynchronisationService.Web.Definitions.Mediator.Base;

namespace Motivation.SynchronisationService.Web.Definitions.Mediator
{
    public class LogPostTransactionBehavior : TransactionBehavior<IRequest<Operation<EventItemViewModel>>, Operation<EventItemViewModel>>
    {
        public LogPostTransactionBehavior(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
