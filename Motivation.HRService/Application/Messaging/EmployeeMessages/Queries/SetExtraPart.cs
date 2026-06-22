using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Motivation.HRService.Application.Messaging.EmployeeMessages.Queries
{
    public class SetExtraPart
    {
        public record Request(string EmployeeCode, ExtraPartDto Model) : IRequest<Operation<bool, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<bool, string>>
        {
            public async ValueTask<Operation<bool, string>> Handle(Request setExtraPartRequest, CancellationToken cancellationToken)
            {
                if (String.IsNullOrWhiteSpace(setExtraPartRequest.EmployeeCode))
                {
                    return Operation.Error("Employee code is null or white space!");
                }

                if (String.IsNullOrWhiteSpace(setExtraPartRequest.Model.PositionCode))
                {
                    return Operation.Error("Position code is null or white space!");
                }

                var code = CodeValue.Create(setExtraPartRequest.Model.Code ?? Guid.CreateVersion7().ToString());
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

                var employeeCode = CodeValue.Create(setExtraPartRequest.EmployeeCode);
                if (!employeeCode.Ok)
                {
                    return Operation.Error(employeeCode.Error);
                }

                var positionCode = CodeValue.Create(setExtraPartRequest.Model.PositionCode);
                if (!positionCode.Ok)
                {
                    return Operation.Error(positionCode.Error);
                }

                var repository = unitOfWork.GetRepository<Employee>();

                var employee = await repository
                    .GetFirstOrDefaultAsync(
                    predicate: x => x.Code == employeeCode.Result,
                    include: query => query
                        .Include(x => x.EmployeePositions)
                            .ThenInclude(x => x.Position)
                        .Include(x => x.EmployeePositions)
                            .ThenInclude(x => x.DeductionHistory),
                    trackingType: TrackingType.Tracking);
                if (employee is null)
                {
                    return Operation.Error("Employee position is not found!");
                }

                var setExtraPartResult = employee.SetExtraPart(positionCode.Result, extraPart.Result);
                if (!setExtraPartResult.Ok)
                {
                    return Operation.Error(setExtraPartResult.Error);
                }

                repository.Update();
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