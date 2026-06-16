using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Motivation.KpiService.Application.Messaging.Kpis.Queries;

public class PutSalesKpi
{
    public record Request(string Code, KpiCreateDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request kpiRequest, CancellationToken cancellationToken)
        {
            var codeResult = CodeValue.Create(kpiRequest.Code);
            if (!codeResult.Ok)
            {
                return Operation.Error(codeResult.Error);
            }

            var repository = unitOfWork.GetRepository<SalesKpi>();
            var entity = await repository
                .GetFirstOrDefaultAsync(
                    predicate: x => x.Code == codeResult.Result, 
                    include: query => query
                        .Include(x => x.MeasurementUnit)
                        .Include(x => x.Filters),
                    trackingType: TrackingType.Tracking);
            if (entity == null)
            {
                return Operation.Error("Position not found!");
            }

            var newCode = CodeValue.Create(kpiRequest.Model.Code);
            if (!newCode.Ok)
            {
                return Operation.Error(newCode.Error);
            }

            var title = TitleValue.Create(kpiRequest.Model.Title);
            if (!title.Ok)
            {
                return Operation.Error(title.Error);
            }

            var abbreviation = AbbreviationValue.Create(kpiRequest.Model.Abbreviation);
            if (!abbreviation.Ok)
            {
                return Operation.Error(abbreviation.Error);
            }

            var calculationType = CalculationTypeMapper.FromRussianString(kpiRequest.Model.CalculationType);
            if (!calculationType.Ok)
            {
                return Operation.Error(calculationType.Error);
            }

            var existingFilters = await unitOfWork
                .GetRepository<KpiFilter>()
                .GetAllAsync(
                    predicate: x => entity.Filters.Select(e => e.Id).Contains(x.Id),
                    trackingType: TrackingType.Tracking);

            var filters = KpiFilterMapper.FromString(
                customerFilter: kpiRequest.Model.CustomerFilter,
                productFilter: kpiRequest.Model.ProductFilter,
                companyFilter: kpiRequest.Model.CompanyFilter);
            if (!filters)
            {
                return Operation.Error(filters.Error);
            }

            var updateResult = entity
                .Update(
                    title: title.Result,
                    code: newCode.Result,
                    abbreviation: abbreviation.Result,
                    filters: filters.Result);
            if (!updateResult)
            {
                return Operation.Error(updateResult.Error);
            }

            var measurementUnitCode = CodeValue.Create(kpiRequest.Model.MeasurementUnitCode);
            if (!measurementUnitCode.Ok)
            {
                return Operation.Error(newCode.Error);
            }
            if (entity.MeasurementUnit.Code != measurementUnitCode.Result)
            {
                var measurementUnit = await unitOfWork.GetRepository<MeasurementUnit>().GetFirstOrDefaultAsync(predicate: x => x.Code == measurementUnitCode.Result, trackingType: TrackingType.Tracking);
                if (measurementUnit == null)
                {
                    return Operation.Error("Measurement unit not found!");
                }
                var setMeasurementUnit = entity.SetMeasurementUnit(measurementUnit);
                if (!setMeasurementUnit.Ok)
                {
                    return Operation.Error(setMeasurementUnit.Error);
                }
            };

            using var transaction = await unitOfWork.BeginTransactionAsync();

            unitOfWork.GetRepository<KpiFilter>().Delete(existingFilters);
            await unitOfWork.GetRepository<KpiFilter>().InsertAsync(filters.Result);
            repository.Update(entity);

            var result = await unitOfWork.SaveChangesAsync();
            if (unitOfWork.Result.Exception is not null)
            {
                await transaction.RollbackAsync();
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
    }
}
