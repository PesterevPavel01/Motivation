using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.ValueObjects;

namespace Motivation.KpiService.Application.Messaging.Kpis.Queries
{
    public class PostSalesKpi
    {
        public record Request(KpiCreateDto Model) : IRequest<Operation<bool, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<bool, string>>
        {
            public async ValueTask<Operation<bool, string>> Handle(Request kpiRequest, CancellationToken cancellationToken)
            {
                logger.LogDebug("Creating new SalesKpi");

                var code = CodeValue.Create(kpiRequest.Model.Code);
                if (!code.Ok)
                {
                    return Operation.Error(code.Error);
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

                var kpiFilters = CreateKpiFilters(
                    customerFilter: kpiRequest.Model.CustomerFilter,
                    productFilter: kpiRequest.Model.ProductFilter,
                    companyFilter: kpiRequest.Model.CompanyFilter);

                if (!kpiFilters.Ok)
                {
                    return Operation.Error(kpiFilters.Error);
                }

                var kpi = SalesKpi.Create(
                    title.Result,
                    code.Result,
                    abbreviation.Result,
                    calculationType.Result,
                    measurementUnit,
                    kpiFilters.Result);
                if (!kpi.Ok)
                {
                    return Operation.Error(kpi.Error);
                }

                await unitOfWork.GetRepository<Kpi>().InsertAsync(kpi.Result, cancellationToken);
                var result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }

                return true;
            }

            private Operation<List<KpiFilter>, string> CreateKpiFilters(
                IEnumerable<string> customerFilter,
                IEnumerable<string> productFilter,
                IEnumerable<string> companyFilter)
            {
                List<KpiFilter> kpiFilters = [];

                if (customerFilter.Any())
                {
                    foreach (var item in customerFilter)
                    {
                        var filterTitle = TitleValue.Create(item);
                        if (!filterTitle.Ok)
                        {
                            return Operation.Error(filterTitle.Error);
                        }
                        var filterItem = KpiFilterCustomer.Create(filterTitle.Result);
                        if (!filterItem.Ok)
                        {
                            return Operation.Error(filterItem.Error);
                        }
                        kpiFilters.Add(filterItem.Result);
                    }
                }
                if (productFilter.Any())
                {
                    foreach (var item in productFilter)
                    {
                        var filterTitle = TitleValue.Create(item);
                        if (!filterTitle.Ok)
                        {
                            return Operation.Error(filterTitle.Error);
                        }
                        var filterItem = KpiFilterProduct.Create(filterTitle.Result);
                        if (!filterItem.Ok)
                        {
                            return Operation.Error(filterItem.Error);
                        }
                        kpiFilters.Add(filterItem.Result);
                    }
                }
                if (companyFilter.Any())
                {
                    foreach (var item in companyFilter)
                    {
                        var filterTitle = TitleValue.Create(item);
                        if (!filterTitle.Ok)
                        {
                            return Operation.Error(filterTitle.Error);
                        }
                        var filterItem = KpiFilterCompany.Create(filterTitle.Result);
                        if (!filterItem.Ok)
                        {
                            return Operation.Error(filterItem.Error);
                        }
                        kpiFilters.Add(filterItem.Result);
                    }
                }
                return kpiFilters;
            }
        }
    }
}
