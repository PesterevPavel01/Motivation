using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnaliticsCore.SyncService.Application.Interfaces;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.AnaliticsCore.SyncService.Application.Executor.Steps
{
    public class SyncProductsStep : ISyncStep
    {
        private readonly IEntityServices<ProductDto> _productService;
        private readonly IUnitOfWork _unitOfWork;

        public SyncProductsStep(IEntityServices<ProductDto> productService, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
        }

        public int OrderIndex => 5;

        public async Task<Operation<bool, string>> ExecuteAsync(
            SyncDto model,
            DateTime dateStart,
            DateTime dateEnd,
            CancellationToken cancellationToken)
        {
            var products = model.Products
                .ToDictionary(
                    x => string.Concat(x.Code),
                    x => x,
                    StringComparer.OrdinalIgnoreCase);

            var productResult = await _productService.CreateOrEditAsync(products, dateStart, dateEnd, cancellationToken);

            if (!productResult.Ok)
            {
                return Operation.Error(productResult.Error);
            }

            var result = await _unitOfWork.SaveChangesAsync();

            if (_unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(_unitOfWork.Result.Exception.Message);
            }

            return true;
        }
    }
}
