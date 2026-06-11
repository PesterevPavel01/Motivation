using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.PositionMessages.Queries
{
    public class PostPosition
    {
        public record Request(PositionCreateDto Model) : IRequest<Operation<bool, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<bool, string>>
        {
            public async ValueTask<Operation<bool, string>> Handle(Request positionRequest, CancellationToken cancellationToken)
            {
                logger.LogDebug("Creating new Position");

                var codeResult = CodeValue.Create(positionRequest.Model.Code);
                if (!codeResult.Ok)
                {
                    return Operation.Error(codeResult.Error);
                }

                var titleResult = TitleValue.Create(positionRequest.Model.Title);
                if (!titleResult.Ok)
                {
                    return Operation.Error(titleResult.Error);
                }

                var baseSalaryResult = SalaryValue.Create(positionRequest.Model.BaseSalary);
                if (!baseSalaryResult.Ok)
                {
                    return Operation.Error(baseSalaryResult.Error);
                }

                var motivationPartValueResult = MotivationPartValue.Create(positionRequest.Model.MotivationPart);
                if (!motivationPartValueResult.Ok)
                {
                    return Operation.Error(baseSalaryResult.Error);
                }

                var motivationPartResult = MotivationPart.Create(motivationPartValueResult.Result);
                if (!motivationPartResult.Ok)
                {
                    return Operation.Error(motivationPartResult.Error);
                }

                WorkWeekType workWeekType = positionRequest.Model.WorkWeekType.Trim().ToLower() == "шестидневка" ?
                    WorkWeekType.SixDayWeek
                    : WorkWeekType.FiveDayWeek;

                var positionResult = Position.Create(titleResult.Result, codeResult.Result, baseSalaryResult.Result, workWeekType, motivationPartResult.Result);
                if (!positionResult.Ok)
                {
                    return Operation.Error(positionResult.Error);
                }

                await unitOfWork.GetRepository<Position>().InsertAsync(positionResult.Result, cancellationToken);
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
