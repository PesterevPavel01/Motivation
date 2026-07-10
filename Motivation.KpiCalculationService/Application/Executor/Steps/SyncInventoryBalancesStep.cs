using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.KpiCalculationService.Application.Interfaces;

namespace Motivation.KpiCalculationService.Application.Executor.Steps;

public class SyncInventoryBalancesStep : ISyncStep
{
    private readonly IEntityServices<InventoryBalanceDto> _inventoryBalanceService;
    private readonly IUnitOfWork _unitOfWork;

    public SyncInventoryBalancesStep(IEntityServices<InventoryBalanceDto> inventoryBalanceService, IUnitOfWork unitOfWork)
    {
        _inventoryBalanceService = inventoryBalanceService;
        _unitOfWork = unitOfWork;
    }

    public int OrderIndex => 7;

    public async Task<Operation<bool, string>> ExecuteAsync(SyncDto model, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        using var inventoryTransaction = await _unitOfWork
            .BeginTransactionAsync();

        var deleteInventoryBalances = await _inventoryBalanceService.DeleteAsync(dateStart, dateEnd);
        
        if (!deleteInventoryBalances.Ok)
            return Operation.Error(deleteInventoryBalances.Error);
        
        var result = await _unitOfWork.SaveChangesAsync();

        if (_unitOfWork.Result.Exception is not null)
        {
            await inventoryTransaction.RollbackAsync(cancellationToken);
            return Operation.Error(_unitOfWork.Result.Exception.Message);
        }

        var inventoryBalances = await _inventoryBalanceService
            .CreateOrEditAsync(
                model.InventoryBalances
                    .ToDictionary(
                        x => string.Concat(x.ProductCode, x.WarehouseCode, x.AsOfDate.ToString()),
                        x => x,
                        StringComparer.OrdinalIgnoreCase),
                dateStart,
                dateEnd,
                cancellationToken);

        if (!inventoryBalances.Ok)
        {
            return Operation.Error(inventoryBalances.Error);
        }

        result = await _unitOfWork.SaveChangesAsync();

        if (_unitOfWork.Result.Exception is not null)
        {
            await inventoryTransaction.RollbackAsync(cancellationToken);
            return Operation.Error(_unitOfWork.Result.Exception.Message);
        }

        await inventoryTransaction.CommitAsync(cancellationToken);

        return true;
    }
}
