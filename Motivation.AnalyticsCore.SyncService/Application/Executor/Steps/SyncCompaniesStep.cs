using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnaliticsCore.SyncService.Application.Interfaces;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.AnaliticsCore.SyncService.Application.Executor.Steps
{
    public class SyncCompaniesStep : ISyncStep
    {
        private readonly IEntityServices<CompanyDto> _companyService;
        private readonly IUnitOfWork _unitOfWork;

        public SyncCompaniesStep(IEntityServices<CompanyDto> companyService, IUnitOfWork unitOfWork)
        {
            _companyService = companyService;
            _unitOfWork = unitOfWork;
        }

        public int OrderIndex => 1;

        public async Task<Operation<bool, string>> ExecuteAsync(
            SyncDto model,
            DateTime dateStart,
            DateTime dateEnd,
            CancellationToken cancellationToken)
        {
            var companyResult = await _companyService
                .CreateOrEditAsync(
                    model.Companies
                        .ToDictionary(
                            x => string.Concat(x.Code),
                            x => x,
                            StringComparer.OrdinalIgnoreCase),
                    dateStart,
                    dateEnd,
                    cancellationToken);

            if (!companyResult.Ok)
            {
                return Operation.Error(companyResult.Error);
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
