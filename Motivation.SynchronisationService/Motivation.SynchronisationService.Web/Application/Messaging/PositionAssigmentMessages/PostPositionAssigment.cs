using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.SynchronisationService.Web.Application.Messaging.PositionAssigmentMessages;

public class PostPositionAssigment
{
    public record Request(PositionAssigmentDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request positionAssigmentRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Position assigment Employee");

            if (String.IsNullOrWhiteSpace(positionAssigmentRequest.Model.EmployeeCode))
            {
                return Operation.Error("Employee code is null or white space!");
            }

            if (String.IsNullOrWhiteSpace(positionAssigmentRequest.Model.PositionCode))
            {
                return Operation.Error("Position code is null or white space!");
            }

            var employeeCodeValueResult = CodeValue.Create(positionAssigmentRequest.Model.EmployeeCode);
            if (!employeeCodeValueResult.Ok)
            {
                return Operation.Error(employeeCodeValueResult.Error);
            }

            var positionCodeValueResult = CodeValue.Create(positionAssigmentRequest.Model.PositionCode);
            if (!positionCodeValueResult.Ok)
            {
                return Operation.Error(positionCodeValueResult.Error);
            }

            var employee = await unitOfWork.GetRepository<Employee>()
                .GetFirstOrDefaultAsync(
                predicate: x => x.Code == employeeCodeValueResult.Result,
                trackingType: TrackingType.Tracking);
            if (employee is null)
            {
                return Operation.Error("Employee is not found!");
            }

            var position = await unitOfWork.GetRepository<Position>()
                .GetFirstOrDefaultAsync(
                predicate: x => x.Code == positionCodeValueResult.Result,
                trackingType: TrackingType.Tracking);
            if (position is null)
            {
                return Operation.Error("Position is not found!");
            }

            var assigmentResult = employee.AssignToPosition(position, positionAssigmentRequest.Model.AssignmentDate);
            if (!assigmentResult.Ok)
            {
                return Operation.Error(assigmentResult.Error);
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
