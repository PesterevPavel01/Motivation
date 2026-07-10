using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Motivation.AnalyticsCore.Application.Mappers;

namespace Motivation.AnalyticsCore.Application.Services
{
    public sealed class WarehouseTransactionService : IEntityServices<WarehouseTransactionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, WarehouseTransactionDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
        {
            var warehouseTransactionCodes = models.Values
                .GroupBy(x => x.Code)
                .Select(x => CodeValue.Create(x.Key));
            if (warehouseTransactionCodes.Any(x => !x.Ok))
            {
                return Operation.Error(warehouseTransactionCodes.FirstOrDefault(x => !x.Ok).Error);
            }
            var saveTransactionCodes = warehouseTransactionCodes.Select(x => x.Result.Value).ToHashSet();
            var existingWarehouseTransaction = (await _unitOfWork
            .GetRepository<WarehouseTransaction>()
            .GetAllAsync(
                predicate: x =>
                    x.AsOfDate >= dateStart
                    && x.AsOfDate <= dateEnd
                || warehouseTransactionCodes.Select(c => c.Result).Contains(x.Code),
                include: query => query
                    .Include(x => x.Product)
                    .Include(x=>x.Warehouse),
                trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

            var deleteMissingEntities = DeleteAllExceptAsync(existingWarehouseTransaction ?? [], saveTransactionCodes);
            if (!deleteMissingEntities.Ok)
            {
                return Operation.Error(deleteMissingEntities.Error);
            }

            var productCodes = models.Values
             .Select(x => x.ProductCode)
             .Distinct()
             .Select(CodeValue.Create);
            if (productCodes.Any(x => !x.Ok))
                return Operation.Error(productCodes.FirstOrDefault(x => !x.Ok).Error);

            var warehouseCodes = models.Values
                .Select(x => x.WarehouseCode)
                .Distinct()
                .Select(CodeValue.Create);
            if (productCodes.Any(x => !x.Ok))
                return Operation.Error(productCodes.FirstOrDefault(x => !x.Ok).Error);

            var products = (await _unitOfWork
                .GetRepository<Product>()
                .GetAllAsync(
                    predicate: x => productCodes.Select(x => x.Result).Contains(x.Code),
                    trackingType: TrackingType.Tracking))
                .ToDictionary(
                    x => x.Code.Value,
                    x => x,
                    StringComparer.OrdinalIgnoreCase
                );

            var warehouses = (await _unitOfWork
                .GetRepository<Warehouse>()
                .GetAllAsync(
                    predicate: x => warehouseCodes.Select(x => x.Result).Contains(x.Code),
                    trackingType: TrackingType.Tracking))
                .ToDictionary(
                    x => x.Code.Value,
                    x => x,
                    StringComparer.OrdinalIgnoreCase
                );

            var edit = Edit(existingWarehouseTransaction ?? [], models, products, warehouses);
            if (!edit.Ok)
            {
                return Operation.Error(edit.Error);
            }

            var create = Create(existingWarehouseTransaction ?? [], models, products, warehouses);
            if (!create.Ok)
            {
                return Operation.Error(create.Error);
            }
            return true;
        }

        private Operation<bool, string> Create(
            Dictionary<string, WarehouseTransaction> existingEntities,
            Dictionary<string, WarehouseTransactionDto> models, 
            Dictionary<string, Product> products, 
            Dictionary<string, Warehouse> warehouses)
        {
            var newWarehouseTransactions = models.Values
                .Where(x => !existingEntities.Values.Any(t => t.Code.Value == x.Code));
            
            List<WarehouseTransaction> warehouseTransactions = [];
            foreach (var model in newWarehouseTransactions)
            {
                if (!products.TryGetValue(model.ProductCode, out var product))
                    return Operation.Error("Product not found!");
                if (product is null)
                    return Operation.Error("Product is null!");

                if (!warehouses.TryGetValue(model.WarehouseCode, out var warehouse))
                    return Operation.Error("Warehouse not found!");
                if (warehouse is null)
                    return Operation.Error("Warehouse is null!");

                var warehouseTransaction = WarehouseTransactionMapper.ToEntity(model, product, warehouse);
                if (!warehouseTransaction.Ok)
                    return Operation.Error(warehouseTransaction.Error);

                warehouseTransactions.Add(warehouseTransaction.Result);
            }

            if (warehouseTransactions.Any())
            {
                _unitOfWork.GetRepository<WarehouseTransaction>()
                    .Insert(warehouseTransactions);
            }
            return true;
        }

        private Operation<bool, string> Edit(
            Dictionary<string, WarehouseTransaction> existingEntities,
            Dictionary<string, WarehouseTransactionDto> models,
            Dictionary<string, Product> products, 
            Dictionary<string, Warehouse> warehouses)
        {
            var editWarehouseTransaction = existingEntities.Values
            .Where(x => models.Values.Any(t => t.Code == x.Code.Value))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

            foreach (var entity in editWarehouseTransaction.Values)
            {
                if (!products.TryGetValue(entity.Product.Code.Value, out var product))
                    return Operation.Error("Product not found!");
                if (product is null)
                    return Operation.Error("Product is null!");

                if (!warehouses.TryGetValue(entity.Warehouse.Code.Value, out var warehouse))
                    return Operation.Error("Warehouse not found!");
                if (warehouse is null)
                    return Operation.Error("Warehouse is null!");

                if (!models.TryGetValue(entity.Code.Value, out var model))
                    return Operation.Error("Model not found!");
                if (model is null)
                    return Operation.Error("Model is null!");

                var editResult = entity.EditEntity(model, warehouse, product);
                if (!editResult.Ok)
                    return Operation.Error(editResult.Error);
            }
            if (editWarehouseTransaction.Any())
            {
                _unitOfWork.GetRepository<WarehouseTransaction>()
                    .Update(editWarehouseTransaction.Values);
            }
            return true;
        }

        private Operation<bool, string> DeleteAllExceptAsync(Dictionary<string, WarehouseTransaction> existingEntities, HashSet<string> saveCodes)
        {
            var deleteCustomerTransactions = existingEntities.Values
                .Where(x => !saveCodes.Any(t => t == x.Code.Value))
                .ToDictionary(
                    x => x.Code.Value,
                    x => x,
                    StringComparer.OrdinalIgnoreCase
                );

            if (deleteCustomerTransactions.Any())
            {
                _unitOfWork.GetRepository<WarehouseTransaction>()
                    .Delete(deleteCustomerTransactions.Values);
            }
            return true;
        }

        public Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
        {
            throw new NotImplementedException();
        }
    }
}
