using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.DeductionMessages;

public class ManageDeduction
{
    public record Request(string? DeductionCode, string EmployeeCode, int Month, int Year, string PositionCode, decimal NewValue) : IRequest<Operation<DeductionDto, string>>;

    public class Handler(EmployeeApiClient client) : IRequestHandler<Request, Operation<DeductionDto, string>>
    {
        public async ValueTask<Operation<DeductionDto, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request.NewValue <= 0)
            {
                return Operation.Error("Значение должно быть больше нуля!");
            }

            if (request.DeductionCode is not null)
            {
                var removeDto = new RemoveDeductionDto(request.DeductionCode, request.PositionCode);
                var removeResult = await client.RemoveDeductionAsync(request.EmployeeCode, removeDto, cancellationToken);
                if (!removeResult.Ok) 
                    return Operation.Error(removeResult.Error);
            }

            var createDto = new SetDeductionDto(
                null,
                null,
                PositionCode: request.PositionCode,
                Value: request.NewValue,
                Month: request.Month,
                Year: request.Year
            );
            var createResult = await client.SetDeductionAsync(request.EmployeeCode, createDto, cancellationToken);
            if (!createResult.Ok) 
                return Operation.Error(createResult.Error);

            return createResult.Result;
        }
    }
}