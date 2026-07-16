using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Microsoft.Extensions.Logging;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.AnalyticsCore.Infrastructure;

namespace Motivation.AnalyticsCore.Application.Services;

public class ProductService : IEntityServices<ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductService> _logger;
    private readonly AnalyticsCoreDbContext _dbContext;
    public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger, AnalyticsCoreDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, ProductDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        var productCodes = models.Values
            .Select(x => CodeValue.Create(x.Code));
        if (productCodes.Any(x => !x.Ok))
            return Operation.Error(productCodes.FirstOrDefault(x => !x.Ok).Error);

        var existingProducts = (await _unitOfWork
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
            .GetAllAsync(trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var productGroups = (await _unitOfWork
            .GetRepository<ProductGroup>()
            .GetAllAsync(trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var existingCodes = existingProducts
            .Keys.ToHashSet();

        var newProducts = models.Values
            .Where(x => !existingCodes.Contains(x.Code))
            .ToDictionary(
                x => x.Code,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        List<Product> products = [];
        foreach (var model in newProducts.Values)
        {
            var product = ProductMapper.ToEntity(model, warehouses, productGroups);
            if (!product)
                return Operation.Error(product.Error);
            products.Add(product.Result);
        }

        var editResult = Edit(models, existingProducts, warehouses, productGroups);
        if (!editResult)
            return Operation.Error(editResult.Error);

        if (products.Any())
        {
            await _unitOfWork.GetRepository<Product>().InsertAsync(products);
        }

        if (existingProducts.Any())
        {
            _unitOfWork.GetRepository<Product>().Update(existingProducts.Values);
        }

        products.AddRange(existingProducts.Values);
        return true;
    }

    public Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
    {
        throw new NotImplementedException();
    }

    private Operation<bool, string> Edit(Dictionary<string, ProductDto> models, Dictionary<string, Product> entities, Dictionary<string, Warehouse> warehouses, Dictionary<string, ProductGroup> productGroups) 
    {
        foreach (var entity in entities.Values)
        {
            if (!models.TryGetValue(entity.Code.Value, out var model))
                return Operation.Error($"Model for product '{entity.Code.Value}' not found!");

            var editResult = entity.EditEntity(model, warehouses, productGroups);
            if (!editResult.Ok)
                return Operation.Error(editResult.Error);
        }
        return true;
    }
}
