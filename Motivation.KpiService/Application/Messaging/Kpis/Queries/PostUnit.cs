using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.KpiService.Application.Messaging.Kpis.Queries
{
    public class PostUnit
    {
        public record Request(UnitCreateDto Model) : IRequest<Operation<bool, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<bool, string>>
        {
            public async ValueTask<Operation<bool, string>> Handle(Request unitRequest, CancellationToken cancellationToken)
            {
                logger.LogDebug("Creating new Unit");

                var code = CodeValue.Create(unitRequest.Model.Code);
                if (!code.Ok)
                {
                    return Operation.Error(code.Error);
                }

                var title = TitleValue.Create(unitRequest.Model.Title);
                if (!title.Ok)
                {
                    return Operation.Error(title.Error);
                }

                var measurementUnit = MeasurementUnit.Create(
                    title.Result,
                    code.Result);
                if (!measurementUnit.Ok)
                {
                    return Operation.Error(measurementUnit.Error);
                }

                await unitOfWork.GetRepository<MeasurementUnit>().InsertAsync(measurementUnit.Result, cancellationToken);
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
