using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.EmployeeMessages;

public class AssignEmployeeToPosition
{
    public record Request(string EmployeeCode, AssignEmployeeToPositionDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger, IServiceProvider serviceProvider)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var employeeCode = CodeValue.Create(request.EmployeeCode);
            if (!employeeCode.Ok)
            {
                return Operation.Error(employeeCode.Error);
            }

            var repository = unitOfWork.GetRepository<Employee>();
            var employee = await repository
                .GetFirstOrDefaultAsync(
                    predicate: x => x.Code == employeeCode.Result,
                    include: query => query
                        .Include(x => x.EmployeePositions)
                            .ThenInclude(x => x.Position),
                    trackingType: TrackingType.Tracking);
            if (employee == null)
            {
                return Operation.Error("Employee not found!");
            }

            var positionCode = CodeValue.Create(request.Model.PositionCode);
            if (!positionCode.Ok)
            {
                return Operation.Error(positionCode.Error);
            }

            var position = await unitOfWork
                .GetRepository<Position>()
                .GetFirstOrDefaultAsync(
                    predicate: x => x.Code == positionCode.Result,
                    include: query => query.Include(x => x.EmployeePositions),
                    trackingType: TrackingType.Tracking);
            if (position == null)
            {
                return Operation.Error("Position not found!");
            }

            var assigmentResult = employee.AssignToPosition(position, request.Model.AssignmentDate);
            if (!assigmentResult.Ok)
            {
                return Operation.Error(assigmentResult.Error);
            }

            repository.Update(employee);
            var result = await unitOfWork.SaveChangesAsync();
            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }
            return true;

        }
    }
}
