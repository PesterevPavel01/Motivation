using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.KpiService.Application.Messaging.Kpis;

public class PostKpi
{
    public record Request(KpiCreateDto Model) : IRequest<Operation<KpiDto, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<KpiDto, string>>
    {
        public async ValueTask<Operation<KpiDto, string>> Handle(Request kpiRequest, CancellationToken cancellationToken)
        {
            var codeResult = CodeValue.Create(Guid.CreateVersion7().ToString());
            if (!codeResult.Ok)
            {
                return Operation.Error(codeResult.Error);
            }

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
                companyFilter: kpiRequest.Model.CompanyFilter,
                customerResponsibleFilter: kpiRequest.Model.CustomerResponsibleFilter,
                productResponsibleFilter: kpiRequest.Model.ProductResponsibleFilter,
                salesResponsibleFilter: kpiRequest.Model.SalesResponsibleFilter,
                productStatusesFilter: kpiRequest.Model.ProductStatusFilter);

            if (!kpiFilters.Ok)
            {
                return Operation.Error(kpiFilters.Error);
            }

            Kpi kpi;
            var kpiType = KpiTypeMapper.FromRussianString(kpiRequest.Model.KpiType);
            if(!kpiType.Ok)
            {
                return Operation.Error(kpiType.Error);
            }

            switch (kpiType.Result)
            {
                case KpiType.Sales:
                    var createdSalesResult = SalesKpi.Create(
                        title: title.Result,
                        code: codeResult.Result,
                        abbreviation: abbreviation.Result,
                        measurementUnit: measurementUnit,
                        calculationType: calculationType.Result,
                        filters: kpiFilters.Result
                    );

                    if (!createdSalesResult.Ok)
                        return Operation.Error(createdSalesResult.Error);
                    kpi = createdSalesResult.Result;
                    break;

                case KpiType.Complexity:
                    var createComplexityResult = ComplexityKpi.Create(
                        title: title.Result,
                        code: codeResult.Result,
                        abbreviation: abbreviation.Result,
                        measurementUnit: measurementUnit,
                        calculationType: calculationType.Result,
                        filters: kpiFilters.Result
                    );

                    if (!createComplexityResult.Ok)
                        return Operation.Error(createComplexityResult.Error);
                    kpi = createComplexityResult.Result;
                    break;

                default:
                    return Operation.Error($"Unsupported KPI type!");
            }

            await unitOfWork.GetRepository<Kpi>().InsertAsync(kpi, cancellationToken);
            var result = await unitOfWork.SaveChangesAsync();
            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return kpi.MapToDto();
        }
    }
}
