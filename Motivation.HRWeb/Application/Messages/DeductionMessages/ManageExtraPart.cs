using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;
using Motivation.HRWeb.Application.Mappings;

namespace Motivation.HRWeb.Application.Messages.DeductionMessages;

public class ManageExtraPart
{
    public record Request(string EmployeeCode, ExtraPartDto CurrentExtraPart) : IRequest<Operation<ExtraPartDto, string>>;

    public class Handler(EmployeeApiClient client) : IRequestHandler<Request, Operation<ExtraPartDto, string>>
    {
        public async ValueTask<Operation<ExtraPartDto, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request.CurrentExtraPart is null)
            {
                return Operation.Error("ExtraPart is null!");
            }
            var createModel = request.CurrentExtraPart.ToCreateDto();
            if (createModel is null)
            {
                return Operation.Error("ExtraPart is null!");
            }
            if (request.CurrentExtraPart.Value <= 0)
            {
                return Operation.Error("Значение должно быть больше нуля!");
            }

            if (request.CurrentExtraPart is not null && !string.IsNullOrWhiteSpace(request.CurrentExtraPart.Code))
            {
                var removeModel = new RemoveExtraPartDto(request.CurrentExtraPart.PositionCode,request.CurrentExtraPart.Code);
                var removeResult = await client.RemoveExtraPartAsync(request.EmployeeCode, removeModel, cancellationToken);
                if (!removeResult.Ok)
                    return Operation.Error(removeResult.Error);
            }

            var createResult = await client.SetExtraPartAsync(request.EmployeeCode, createModel, cancellationToken);
            if (!createResult.Ok)
                return Operation.Error(createResult.Error);

            return createResult.Result;
        }
    }
}