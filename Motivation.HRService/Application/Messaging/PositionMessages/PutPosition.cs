using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.PositionMessages;

public class PutPosition
{
    public record Request(string Code, PositionUpdateDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request positionRequest, CancellationToken cancellationToken)
        {
            var code = CodeValue.Create(positionRequest.Code);
            if (!code.Ok)
            {
                return Operation.Error(code.Error);
            }

            var repository = unitOfWork.GetRepository<Position>();
            var entity = await repository.GetFirstOrDefaultAsync(predicate: x => x.Code == code.Result, trackingType: TrackingType.Tracking);
            if (entity == null)
            {
                return Operation.Error("Position not found!");
            }

            var workWeekType = WorkWeekTypeMapper.FromRussianString(positionRequest.Model.WorkWeekType);
            if (!workWeekType)
            {
                return Operation.Error("Work week type not found!");
            }

            var newCode = CodeValue.Create(positionRequest.Model.Code);
            if (!newCode.Ok)
            {
                return Operation.Error(newCode.Error);
            }

            var title = TitleValue.Create(positionRequest.Model.Title);
            if (!title.Ok)
            {
                return Operation.Error(title.Error);
            }

            var baseSalary = SalaryValue.Create(positionRequest.Model.BaseSalary);
            if (!baseSalary.Ok)
            {
                return Operation.Error(baseSalary.Error);
            }

            var motivationPartValue = MotivationPartValue.Create(positionRequest.Model.MotivationPart);
            if (!motivationPartValue.Ok)
            {
                return Operation.Error(baseSalary.Error);
            }

            var updateMotivationPart = entity.MotivationPart.Update(motivationPartValue.Result);
            if (!updateMotivationPart.Ok)
            {
                return Operation.Error(updateMotivationPart.Error);
            }

            var updateResult = entity
                .Update(
                    title: title.Result,
                    code: newCode.Result,
                    baseSalary: baseSalary.Result,
                    workWeekType: workWeekType.Result
                );

            repository.Update(entity);
            var result = await unitOfWork.SaveChangesAsync();
            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return true;
        }
    }
}
