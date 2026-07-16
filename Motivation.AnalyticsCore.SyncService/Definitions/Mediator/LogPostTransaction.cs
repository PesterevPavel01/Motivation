using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.AnaliticsCore.SyncService.Definitions.Mediator.Base;
using Motivation.Contracts.ViewModels;

namespace Motivation.AnaliticsCore.SyncService.Definitions.Mediator
{
    public class LogPostTransactionBehavior : TransactionBehavior<IRequest<Operation<EventItemViewModel>>, Operation<EventItemViewModel>>
    {
        public LogPostTransactionBehavior(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
