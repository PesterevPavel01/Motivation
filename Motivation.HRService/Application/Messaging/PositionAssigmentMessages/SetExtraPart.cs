using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.PositionAssigmentMessages
{
    public class SetExtraPart
    {
        public record Request(ExtraPartDto Model) : IRequest<Operation<bool, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<bool, string>>
        {
            public async ValueTask<Operation<bool, string>> Handle(Request setExtraPartRequest, CancellationToken cancellationToken)
            {
                logger.LogDebug($"Set extra part.");

                if (String.IsNullOrWhiteSpace(setExtraPartRequest.Model.EmployeeCode))
                {
                    return Operation.Error("Employee code is null or white space!");
                }

                if (String.IsNullOrWhiteSpace(setExtraPartRequest.Model.PositionCode))
                {
                    return Operation.Error("Position code is null or white space!");
                }

                var code = CodeValue.Create(setExtraPartRequest.Model.Code);
                if (!code.Ok)
                {
                    return Operation.Error(code.Error);
                }

                var title = TitleValue.Create(setExtraPartRequest.Model.Title);
                if (!title.Ok)
                {
                    return Operation.Error(title.Error);
                }

                var extraPartValue = ExtraPartValue.Create(setExtraPartRequest.Model.Value);
                if (!extraPartValue.Ok)
                {
                    return Operation.Error(extraPartValue.Error);
                }

                var extraPart = ExtraPart.Create(title.Result, code.Result, extraPartValue.Result, setExtraPartRequest.Model.ValidFrom, setExtraPartRequest.Model.ValidTo);
                if (!extraPart.Ok)
                {
                    return Operation.Error(extraPart.Error);
                }

                var employeeCode = CodeValue.Create(setExtraPartRequest.Model.EmployeeCode);
                if (!employeeCode.Ok)
                {
                    return Operation.Error(employeeCode.Error);
                }

                var positionCode = CodeValue.Create(setExtraPartRequest.Model.PositionCode);
                if (!positionCode.Ok)
                {
                    return Operation.Error(positionCode.Error);
                }

                var employeePosition = await unitOfWork.GetRepository<EmployeePosition>()
                    .GetFirstOrDefaultAsync(
                    predicate: x => x.Employee.Code == employeeCode.Result
                        && x.Position.Code == positionCode.Result,
                    trackingType: TrackingType.Tracking);
                if (employeePosition is null)
                {
                    return Operation.Error("Employee position is not found!");
                }

                var setExtraPartResult = employeePosition.SetExtraPart(extraPart.Result);
                if (!setExtraPartResult.Ok)
                {
                    return Operation.Error(setExtraPartResult.Error);
                }

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
