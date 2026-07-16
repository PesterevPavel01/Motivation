using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnaliticsCore.SyncService.Application.Interfaces;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.AnaliticsCore.SyncService.Application.Executor.Steps
{
    public class SyncCustomerGroupsStep : ISyncStep
    {
        private readonly IEntityServices<CustomerGroupDto> _customerGroupService;
        private readonly IUnitOfWork _unitOfWork;

        public SyncCustomerGroupsStep(IEntityServices<CustomerGroupDto> customerGroupService, IUnitOfWork unitOfWork)
        {
            _customerGroupService = customerGroupService;
            _unitOfWork = unitOfWork;
        }

        public int OrderIndex => 2;

        public async Task<Operation<bool, string>> ExecuteAsync(
            SyncDto model,
            DateTime dateStart,
            DateTime dateEnd,
            CancellationToken cancellationToken)
        {
            var customerGroupResult = await _customerGroupService
                .CreateOrEditAsync(
                    model.CustomerGroups
                        .ToDictionary(
                            x => string.Concat(x.Code),
                            x => x,
                            StringComparer.OrdinalIgnoreCase),
                    dateStart,
                    dateEnd,
                    cancellationToken);

            if (!customerGroupResult.Ok)
            {
                return Operation.Error(customerGroupResult.Error);
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
