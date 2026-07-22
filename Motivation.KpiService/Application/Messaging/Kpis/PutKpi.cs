using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.ValueObjects;

namespace Motivation.KpiService.Application.Messaging.Kpis;

public class PutKpi
{
    public record Request(KpiUpdateDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request kpiRequest, CancellationToken cancellationToken)
        {
            var codeResult = CodeValue.Create(kpiRequest.Model.Code);
            if (!codeResult.Ok)
            {
                return Operation.Error(codeResult.Error);
            }
            var repository = unitOfWork.GetRepository<Kpi>();
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

            var concreteKpi = entity.MapToConcrete();

            var title = TitleValue.Create(kpiRequest.Model.Title);
            if (!title.Ok)
            {
                return Operation.Error(codeResult.Error);
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

            var measurementUnitCode = CodeValue.Create(kpiRequest.Model.MeasurementUnitCode);
            if (!measurementUnitCode.Ok)
            {
                return Operation.Error(measurementUnitCode.Error);
            }

            var measurementUnit = await unitOfWork
                .GetRepository<MeasurementUnit>()
                .GetFirstOrDefaultAsync(
                    predicate: x => x.Code == measurementUnitCode.Result,
                    trackingType: TrackingType.Tracking);

            if (measurementUnit is null)
            {
                return Operation.Error("MeasurementUnit not found!");
            }

            var kpiFilters = KpiFilterMapper.FromString(
                customerFilter: kpiRequest.Model.CustomerFilter,
                supplierFilter: kpiRequest.Model.SupplierFilter,
                productFilter: kpiRequest.Model.ProductFilter,
                productStatusesFilter: kpiRequest.Model.ProductStatusFilter,
                companyFilter: kpiRequest.Model.CompanyFilter,
                customerResponsibleFilter: kpiRequest.Model.CustomerResponsibleFilter,
                productResponsibleFilter: kpiRequest.Model.ProductResponsibleFilter,
                salesResponsibleFilter: kpiRequest.Model.SalesResponsibleFilter);

            if (!kpiFilters.Ok)
            {
                return Operation.Error(kpiFilters.Error);
            }

            switch (concreteKpi)
            {
                case SalesKpi salesKpi:
                    var updateSalesResult = salesKpi.Update(
                        title: title.Result,
                        code: codeResult.Result,
                        abbreviation: abbreviation.Result,
                        calculationType: calculationType.Result,
                        measurementUnit: measurementUnit,
                        filters: kpiFilters.Result
                    );

                    if (!updateSalesResult.Ok)
                        return Operation.Error(updateSalesResult.Error);
                    break;
                
                case ComplexityKpi complexityKpi:
                    var updateComplexityResult = complexityKpi.Update(
                        title: title.Result,
                        code: codeResult.Result,
                        abbreviation: abbreviation.Result,
                        measurementUnit: measurementUnit,
                        filters: kpiFilters.Result
                    );

                    if (!updateComplexityResult.Ok)
                        return Operation.Error(updateComplexityResult.Error);
                    break;

                default:
                    return Operation.Error($"Unsupported KPI type: {concreteKpi.KpiType}");
            }

            using var transaction = await unitOfWork.BeginTransactionAsync();

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
