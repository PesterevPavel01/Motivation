using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnaliticsCore.SyncService.Application.Interfaces;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.AnaliticsCore.SyncService.Application.Executor.Steps
{
    public class SyncCustomersStep : ISyncStep
    {
        private readonly IEntityServices<CustomerDto> _customerService;
        private readonly IUnitOfWork _unitOfWork;

        public SyncCustomersStep(IEntityServices<CustomerDto> customerService, IUnitOfWork unitOfWork)
        {
            _customerService = customerService;
            _unitOfWork = unitOfWork;
        }

        public int OrderIndex => 3;

        public async Task<Operation<bool, string>> ExecuteAsync(SyncDto model, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
        {
            var customerResult = await _customerService
                .CreateOrEditAsync(
                    model.Customers
                        .ToDictionary(
                            x => string.Concat(x.Code),
                            x => x,
                            StringComparer.OrdinalIgnoreCase),
                    dateStart,
                    dateEnd,
                    cancellationToken);

            if (!customerResult.Ok)
            {
                return Operation.Error(customerResult.Error);
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
