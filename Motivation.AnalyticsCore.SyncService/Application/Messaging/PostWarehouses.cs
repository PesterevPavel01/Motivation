using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;

namespace Motivation.AnaliticsCore.SyncService.Application.Messaging
{
    public class PostWarehouses
    {
        public record Request(IEnumerable<CompanyDto> Models) : IRequest<Operation<bool, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<bool, string>>
        {
            public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
            {
                var companies = request.Models
                    .Select(x => CompanyMapper.ToEntity(x));

                if (companies.Any(x => !x.Ok))
                    return Operation.Error(companies.FirstOrDefault(x => !x.Ok).Error);

                var entities = companies.Select(x => x.Result);

                unitOfWork.GetRepository<Company>()
                    .Insert(entities);

                var result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }

                return true;
            }
        }
    }
}
