using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.KpiCalculationService.Application.Interfaces;

namespace Motivation.KpiCalculationService.Application.Executor.Steps;

public class SyncCustomerTransactionStep : ISyncStep
{
    private readonly IEntityServices<CustomerTransactionDto> _customerTransactionService;
    private readonly IUnitOfWork _unitOfWork;

    public SyncCustomerTransactionStep(IEntityServices<CustomerTransactionDto> customerTransactionService, IUnitOfWork unitOfWork)
    {
        _customerTransactionService = customerTransactionService;
        _unitOfWork = unitOfWork;
    }

    public int OrderIndex => 8;

    public async Task<Operation<bool, string>> ExecuteAsync(SyncDto model, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        using var CustomerTransactionsTransaction = await _unitOfWork
                    .BeginTransactionAsync();

        var customerTransactions = await _customerTransactionService
            .CreateOrEditAsync(
                model.CustomerTransactions
                    .ToDictionary(
                        x => x.Code,
                        x => x,
                        StringComparer.OrdinalIgnoreCase),
                dateStart,
                dateEnd,
                cancellationToken);

        if (!customerTransactions.Ok)
        {
            return Operation.Error(customerTransactions.Error);
        }

        var result = await _unitOfWork.SaveChangesAsync();

        if (_unitOfWork.Result.Exception is not null)
        {
            await CustomerTransactionsTransaction.RollbackAsync(cancellationToken);
            return Operation.Error(_unitOfWork.Result.Exception.Message);
        }

        await CustomerTransactionsTransaction.CommitAsync(cancellationToken);

        return true;
    }
}
