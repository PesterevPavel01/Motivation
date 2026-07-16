using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnaliticsCore.SyncService.Application.Interfaces;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.AnaliticsCore.SyncService.Application.Executor.Steps
{
    public class SyncReceivablesStep : ISyncStep
    {
        private readonly IEntityServices<ReceivableDto> _receivablesService;
        private readonly IUnitOfWork _unitOfWork;

        public SyncReceivablesStep(IEntityServices<ReceivableDto> receivablesService, IUnitOfWork unitOfWork)
        {
            _receivablesService = receivablesService;
            _unitOfWork = unitOfWork;
        }

        public int OrderIndex => 7;

        public async Task<Operation<bool, string>> ExecuteAsync(SyncDto model, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
        {
            using var receivableTransaction = await _unitOfWork
                .BeginTransactionAsync();

            var deleteReceivables = await _receivablesService.DeleteAsync(dateStart, dateEnd);

            if (!deleteReceivables.Ok)
                return Operation.Error(deleteReceivables.Error);

            var result = await _unitOfWork.SaveChangesAsync();

            if (_unitOfWork.Result.Exception is not null)
            {
                await receivableTransaction.RollbackAsync(cancellationToken);
                return Operation.Error(_unitOfWork.Result.Exception.Message);
            }
            var receivables = model.Receivables
                .ToDictionary(
                    x => string.Concat(x.Company.Code, x.Customer.Code, x.AsOfDate.ToString()),
                    x => x,
                    StringComparer.OrdinalIgnoreCase);

            var receivablesResult = await _receivablesService.CreateOrEditAsync(receivables, dateStart, dateEnd, cancellationToken);

            if (!receivablesResult.Ok)
            {
                return Operation.Error(receivablesResult.Error);
            }

            result = await _unitOfWork.SaveChangesAsync();

            if (_unitOfWork.Result.Exception is not null)
            {
                await receivableTransaction.RollbackAsync(cancellationToken);
                return Operation.Error(_unitOfWork.Result.Exception.Message);
            }

            await receivableTransaction.CommitAsync(cancellationToken);

            return true;
        }
    }
}
