using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnaliticsCore.SyncService.Application.Interfaces;
using Motivation.AnalyticsCore.Application.Services;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.AnalyticsCore.Mappers
{
    public sealed class SyncProductGroupsStep : ISyncStep
    {
        private readonly IEntityServices<ProductGroupDto> _productGroupService;
        private readonly IUnitOfWork _unitOfWork;

        public SyncProductGroupsStep(IEntityServices<ProductGroupDto> productGroupService, IUnitOfWork unitOfWork)
        {
            _productGroupService = productGroupService;
            _unitOfWork = unitOfWork;
        }
        public int OrderIndex => 4;

        public async Task<Operation<bool, string>> ExecuteAsync(SyncDto model, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
        {
            var productGroupResult = await _productGroupService
                .CreateOrEditAsync(
                    model.ProductGroups
                        .ToDictionary(
                            x => string.Concat(x.Code),
                            x => x,
                            StringComparer.OrdinalIgnoreCase),
                    dateStart,
                    dateEnd,
                    cancellationToken);

            if (!productGroupResult.Ok)
            {
                return Operation.Error(productGroupResult.Error);
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
