using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.ViewModels;
using Motivation.KpiCalculationService.Definitions.Mediator.Base;

namespace Motivation.KpiCalculationService.Definitions.Mediator
{
    public class LogPostTransactionBehavior : TransactionBehavior<IRequest<Operation<EventItemViewModel>>, Operation<EventItemViewModel>>
    {
        public LogPostTransactionBehavior(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
