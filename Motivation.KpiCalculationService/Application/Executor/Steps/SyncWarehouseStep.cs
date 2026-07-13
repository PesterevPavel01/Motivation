using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.KpiCalculationService.Application.Interfaces;

namespace Motivation.KpiCalculationService.Application.Executor.Steps;

public class SyncWarehouseStep : ISyncStep
{

    private readonly IEntityServices<WarehouseDto> _warehouseService;
    private readonly IUnitOfWork _unitOfWork;

    public SyncWarehouseStep(IEntityServices<WarehouseDto> warehouseService, IUnitOfWork unitOfWork)
    {
        _warehouseService = warehouseService;
        _unitOfWork = unitOfWork;
    }

    public int OrderIndex => 6;

    public async Task<Operation<bool, string>> ExecuteAsync(SyncDto model, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        var warehouses = model.Warehouses
            .ToDictionary(
                x => string.Concat(x.Code),
                x => x,
                StringComparer.OrdinalIgnoreCase);

        var warehouseResult = await _warehouseService.CreateOrEditAsync(warehouses, dateStart, dateEnd, cancellationToken);

        if (!warehouseResult.Ok)
        {
            return Operation.Error(warehouseResult.Error);
        }

        var result = await _unitOfWork.SaveChangesAsync();
        
        if (_unitOfWork.Result.Exception is not null)
        {
            return Operation.Error(_unitOfWork.Result.Exception.Message);
        }

        return true;
    }
}
