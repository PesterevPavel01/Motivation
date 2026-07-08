using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Motivation.HRService.Application.Messaging.EmployeeMessages;

public class RemoveActualHours
{
    public record Request(string Code, RemoveActualHoursDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request employeeRequest, CancellationToken cancellationToken)
        {
            var code = CodeValue.Create(employeeRequest.Code);

            if (!code.Ok)
            {
                return Operation.Error(code.Error);
            }

            var repository = unitOfWork.GetRepository<Employee>();
            var entity = await repository
                .GetFirstOrDefaultAsync(
                predicate: x => x.Code == code.Result,
                include: query => query
                    .Include(x => x.EmployeePositions)
                        .ThenInclude(x => x.Position)
                    .Include(x => x.EmployeePositions)
                        .ThenInclude(x => x.ActualHoursHistory),
                trackingType: TrackingType.Tracking);

            if (entity == null)
            {
                return Operation.Error("Employee not found!");
            }

            var positionCode = CodeValue.Create(employeeRequest.Model.PositionCode);
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

            var month = MonthValue.Create(employeeRequest.Model.Month);
            if (!month.Ok)
            {
                return Operation.Error(month.Error);
            }

            var year = YearValue.Create(employeeRequest.Model.Year);
            if (!year.Ok)
            {
                return Operation.Error(year.Error);
            }

            var setActualHours = entity.RemoveActualHours(employeePosition.Position, month.Result, year.Result);
            if (!setActualHours.Ok)
            {
                return Operation.Error(setActualHours.Error);
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
