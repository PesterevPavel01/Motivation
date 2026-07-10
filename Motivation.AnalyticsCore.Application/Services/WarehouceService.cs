using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Services
{
    public sealed class WarehouseService : IEntityServices<WarehouseDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, WarehouseDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
        {
            if (models.Values.Any(c => c is null || string.IsNullOrWhiteSpace(c.Code)))
                return Operation.Error("Warehouse (or warehouse code) is null or white space!");

            var warehouseCodes = models.Values
                .Select(x => CodeValue.Create(x.Code));

            if (warehouseCodes.Any(x => !x.Ok))
                return Operation.Error(warehouseCodes.FirstOrDefault(x => !x.Ok).Error);

            var existingWarehouses = (await _unitOfWork
            .GetRepository<Warehouse>()
            .GetAllAsync(
                predicate: x => warehouseCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToList();

            var newWarehouses = models.Values
                .Where(x => !existingWarehouses.Select(p => p.Code.Value).Contains(x.Code))
                .ToArray();


            List<Warehouse> warehouses = [];
            foreach (var model in newWarehouses)
            {
                var warehouse = WarehouseMapper.ToEntity(model);
                if (!warehouse)
                    return Operation.Error(warehouse.Error);
                warehouses.Add(warehouse.Result);
            }

            _unitOfWork.GetRepository<Warehouse>()
                .Insert(warehouses);

            var editResult = Edit(models, existingWarehouses);
            if (!editResult)
                return Operation.Error(editResult.Error);

            _unitOfWork.GetRepository<Warehouse>()
                .Update(existingWarehouses);

            return true;
        }

        public Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
        {
            throw new NotImplementedException();
        }

        private Operation<bool, string> Edit(Dictionary<string, WarehouseDto> models, List<Warehouse> entities)
        {
            List<Warehouse> warehouses = [];
            foreach (var entity in entities)
            {
                if (!models.TryGetValue(entity.Code.Value, out var model))
                    return Operation.Error("Warehouse not found!");
                if (model is null)
                    return Operation.Error($"Model {typeof(WarehouseDto).Name} not found!");
                var editResult = entity.EditEntity(model);
                if (!editResult.Ok)
                    return Operation.Error(editResult.Error);
            }
            return true;
        }
    }
}
