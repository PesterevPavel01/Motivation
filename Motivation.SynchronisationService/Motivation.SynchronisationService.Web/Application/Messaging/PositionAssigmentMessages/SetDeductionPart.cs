using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.SynchronisationService.Web.Application.Messaging.PositionAssigmentMessages;

public class SetDeductionPart
{
    public record Request(DeductionDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request setDeductionRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Set deduction.");

            if (String.IsNullOrWhiteSpace(setDeductionRequest.Model.EmployeeCode))
            {
                return Operation.Error("Employee code is null or white space!");
            }

            if (String.IsNullOrWhiteSpace(setDeductionRequest.Model.PositionCode))
            {
                return Operation.Error("Position code is null or white space!");
            }
            
            var code = CodeValue.Create(setDeductionRequest.Model.Code);
            if (!code.Ok)
            {
                return Operation.Error(code.Error);
            }

            var title = TitleValue.Create(setDeductionRequest.Model.Title);
            if (!title.Ok)
            {
                return Operation.Error(title.Error);
            }

            var deductionValue = DeductionValue.Create(setDeductionRequest.Model.Value);
            if (!deductionValue.Ok)
            {
                return Operation.Error(deductionValue.Error);
            }

            var month = MonthValue.Create(setDeductionRequest.Model.Month);
            if (!month.Ok)
            {
                return Operation.Error(month.Error);
            }

            var year = YearValue.Create(setDeductionRequest.Model.Year);
            if (!year.Ok)
            {
                return Operation.Error(year.Error);
            }

            var deduction = Deduction.Create(title.Result, code.Result, deductionValue.Result, month.Result, year.Result);
            if (!deduction.Ok)
            {
                return Operation.Error(deduction.Error);
            }

            var employeeCode = CodeValue.Create(setDeductionRequest.Model.EmployeeCode);
            if (!employeeCode.Ok)
            {
                return Operation.Error(employeeCode.Error);
            }

            var positionCode = CodeValue.Create(setDeductionRequest.Model.PositionCode);
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

            var setDeductionResult = employeePosition.SetDeduction(deduction.Result);
            if (!setDeductionResult.Ok)
            {
                return Operation.Error(setDeductionResult.Error);
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
