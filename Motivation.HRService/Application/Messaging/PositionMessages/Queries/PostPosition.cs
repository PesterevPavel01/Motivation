using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.PositionMessages.Queries
{
    public class PostPosition
    {
        public record Request(PositionCreateDto Model) : IRequest<Operation<PositionDto, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<PositionDto, string>>
        {
            public async ValueTask<Operation<PositionDto, string>> Handle(Request positionRequest, CancellationToken cancellationToken)
            {
                var codeResult = CodeValue.Create(positionRequest.Model.Code ?? Guid.CreateVersion7().ToString());
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

                var workWeekType = WorkWeekTypeMapper.FromRussianString(positionRequest.Model.WorkWeekType);
                if (!workWeekType)
                {
                    return Operation.Error("Work week type not found!");
                }

                var positionResult = Position.Create(titleResult.Result, codeResult.Result, baseSalaryResult.Result, workWeekType.Result, motivationPartResult.Result);
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

                return positionResult.Result.MapToDto();
            }
        }
    }
}
