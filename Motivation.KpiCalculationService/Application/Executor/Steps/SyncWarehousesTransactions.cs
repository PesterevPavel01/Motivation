using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.KpiCalculationService.Application.Interfaces;

namespace Motivation.KpiCalculationService.Application.Executor.Steps;

public class SyncWarehousesTransactions : ISyncStep
{
    private readonly IEntityServices<WarehouseTransactionDto> _warehouseTransactionService;
    private readonly IUnitOfWork _unitOfWork;

    public SyncWarehousesTransactions(IEntityServices<WarehouseTransactionDto> warehouseTransactionService, IUnitOfWork unitOfWork)
    {
        _warehouseTransactionService = warehouseTransactionService;
        _unitOfWork = unitOfWork;
    }

    public int OrderIndex => 8;

    public async Task<Operation<bool, string>> ExecuteAsync(SyncDto model, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        using var warehouseTransaction = await _unitOfWork
            .BeginTransactionAsync();

        var warehouseTransactions = await _warehouseTransactionService
            .CreateOrEditAsync(
                model.WarehouseTransactions
                    .ToDictionary(
                        x => x.Code,
                        x => x,
                        StringComparer.OrdinalIgnoreCase),
                dateStart,
                dateEnd, 
                cancellationToken);

        if (!warehouseTransactions.Ok)
        {
            return Operation.Error(warehouseTransactions.Error);
        }

        var result = await _unitOfWork.SaveChangesAsync();

        if (_unitOfWork.Result.Exception is not null)
        {
            await warehouseTransaction.RollbackAsync(cancellationToken);
            return Operation.Error(_unitOfWork.Result.Exception.Message);
        }

        await warehouseTransaction.CommitAsync(cancellationToken);

        return true;
    }
}
