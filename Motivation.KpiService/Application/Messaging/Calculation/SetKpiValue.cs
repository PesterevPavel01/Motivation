using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Domain.Entities;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.ValueObjects;

namespace Motivation.KpiService.Application.Messaging.Calculation;

public class SetKpiValue
{
    public record Request(KpiCalculationTaskResult Result) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request.Result.PeriodFrom > request.Result.PeriodTo)
            {
                return Operation.Error("DateStart cannot be greater than DateEnd.");
            }

            var repository = unitOfWork
                .GetRepository<PositionKpi>();

            var positionKpi = await repository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == request.Result.PositionKpiId,
                include: query => query
                    .Include(x => x.Position)
                        .ThenInclude(x => x.MotivationPart)
                    .Include(x => x.TargetHistory)
                        .ThenInclude(x => x.KpiFact!),
                trackingType: TrackingType.Tracking);

            if (positionKpi is null)
            { 
                return Operation.Error("Position kpi not found!");
            }

            var target = positionKpi.TargetHistory
                .FirstOrDefault(x => 
                    x.ValidFrom >= request.Result.PeriodFrom
                    && x.ValidTo <= request.Result.PeriodTo);

            if (target is null)
            {
                return Operation.Error("Target not found!");
            }

            if (target.KpiFact is null)
            {
                var creationResult = Create(target, positionKpi.Position.MotivationPart, request.Result, positionKpi.Weight);
                if (!creationResult.Ok)
                {
                    return Operation.Error(creationResult.Error);
                }
            }
            else 
            {
                var editResult = Edit(target, positionKpi.Position.MotivationPart,  request.Result, positionKpi.Weight);
                if (!editResult.Ok)
                {
                    return Operation.Error(editResult.Error);
                }
            }

            repository.Update(positionKpi);

            var result = await unitOfWork.SaveChangesAsync();

            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return true;
        }
        private Operation<bool, string> Create(KpiTarget target, MotivationPart motivationPart, KpiCalculationTaskResult result, WeightValue weight)
        {
            var fact = FactValue.Create(result.Value);
            if (!fact.Ok)
            { 
                return Operation.Error(fact.Error);
            }

            var achievementValue = AchievementValue.Create(result.Achievement);
            if (!achievementValue.Ok)
            {
                return Operation.Error(achievementValue.Error);
            }
            //тут должна быть учтена логика расчета бонуса в зависимости от процента выполнения
            var bonus = BonusAmountValue.Create(motivationPart.MotivationPartValue.Value * weight.Value * result.Achievement);
            if (!bonus.Ok)
            {
                return Operation.Error(bonus.Error);
            }

            var kpiFact = KpiFact.Create(fact.Result, achievementValue.Result, bonus.Result);
            if (!kpiFact.Ok)
            {
                return Operation.Error(kpiFact.Error);
            }

            var setTarget = target.SetFact(kpiFact.Result);
            if (!setTarget.Ok)
            {
                return Operation.Error(setTarget.Error);
            }

            return true;
        }

        private Operation<bool, string> Edit(KpiTarget target, MotivationPart motivationPart, KpiCalculationTaskResult result, WeightValue weight)
        {
            if(target.KpiFact is null)
            {
                return Operation.Error("Fact not found!");
            }

            var fact = FactValue.Create(result.Value);
            if (!fact.Ok)
            {
                return Operation.Error(fact.Error);
            }

            var achievementValue = AchievementValue.Create(result.Achievement);
            if (!achievementValue.Ok)
            {
                return Operation.Error(achievementValue.Error);
            }
            //тут должна быть учтена логика расчета бонуса в зависимости от процента выполнения
            var bonus = BonusAmountValue.Create(motivationPart.MotivationPartValue.Value * weight.Value * result.Achievement);
            if (!bonus.Ok)
            {
                return Operation.Error(bonus.Error);
            }

            var editFact = target.KpiFact!.Edit(fact.Result, achievementValue.Result, bonus.Result);
            if (!editFact.Ok)
            {
                return Operation.Error(editFact.Error);
            }
            return true;
        }
    }
}
