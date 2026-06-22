using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Motivation.IdentityServer.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Motivation.HRService.Application.Messaging.EmployeeMessages.Queries;

public class RemoveDeduction
{
    public record Request(string EmployeeCode, RemoveDeductionDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
     : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<Employee>();

            var employeeCode = CodeValue.Create(request.EmployeeCode);
            if (!employeeCode.Ok)
            {
                return Operation.Error(employeeCode.Error);
            }
            var positionCode = CodeValue.Create(request.Model.PositionCode);
            if (!positionCode.Ok)
            {
                return Operation.Error(positionCode.Error);
            }
            var deductionCode = CodeValue.Create(request.Model.DeductionCode);
            if (!deductionCode.Ok)
            {
                return Operation.Error(deductionCode.Error);
            }

            var entity = await repository
                .GetFirstOrDefaultAsync(
                    predicate: x => x.Code == employeeCode.Result,
                    include: query => query
                        .Include(x => x.EmployeePositions)
                            .ThenInclude(x => x.Position)
                        .Include(x => x.EmployeePositions)
                            .ThenInclude(x => x.DeductionHistory),
                    trackingType: TrackingType.Tracking);
            if (entity == null)
            {
                return Operation.Error("Employee not found!");
            }
            var removeDeduction = entity.RemoveDeduction(positionCode.Result, deductionCode.Result);
            if (!removeDeduction.Ok)
            {
                return Operation.Error(removeDeduction.Error);
            }

            repository.Update(entity);
            await unitOfWork.SaveChangesAsync();
            if (!unitOfWork.Result.Ok)
            {
                return Operation.Error(unitOfWork.Result.Exception?.Message ?? AppData.Exceptions.SomethingWrong);
            }

            return true;
        }
    }
}
