using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.EmployeeMessages.Queries;

public class DismissEmployeeFromPosition
{
    public record Request(string EmployeeCode, string PositionCode) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request employeeRequest, CancellationToken cancellationToken)
        {
            var employeeCode = CodeValue.Create(employeeRequest.EmployeeCode);

            if (!employeeCode.Ok)
            {
                return Operation.Error(employeeCode.Error);
            }

            var repository = unitOfWork.GetRepository<Employee>();
            var entity = await repository
                .GetFirstOrDefaultAsync(
                predicate: x => x.Code == employeeCode.Result,
                include: query => query
                    .Include(x => x.EmployeePositions)
                        .ThenInclude(x => x.Position)
                    .Include(x => x.EmployeePositions)
                        .ThenInclude(x => x.ActualHoursHistory),
                trackingType: TrackingType.Tracking);

            if (entity is null)
            {
                return Operation.Error("Employee not found!");
            }

            var positionCode = CodeValue.Create(employeeRequest.PositionCode);
            if (!positionCode.Ok)
            {
                return Operation.Error(positionCode.Error);
            }

            var employeePosition = entity.EmployeePositions
                .FirstOrDefault(x => x.Position.Code == positionCode.Result);
            if (employeePosition is null)
            {
                return Operation.Error("Position not found!");
            }

            var dismissEmployee = entity.Dismiss(employeePosition.Position);
            if (!dismissEmployee.Ok)
            {
                return Operation.Error(dismissEmployee.Error);
            }

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
