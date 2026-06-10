using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.SynchronisationService.Web.Application.Messaging.PositionMessages.Queries;

public class PostKpi
{
    public record Request(KpiCreateDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request kpiRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug("Creating new Kpi");

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

            var kpi = Kpi.Create(
                title.Result,
                code.Result,
                abbreviation.Result,
                calculationType.Result,
                measurementUnit);
            if (!kpi.Ok)
            {
                return Operation.Error(kpi.Error);
            }

            if (kpiRequest.Model.CustomerFilter.Any())
            {
                foreach (var item in kpiRequest.Model.CustomerFilter)
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
                    kpi.Result.AddFilter(filterItem.Result);
                }
            }
            if (kpiRequest.Model.ProductFilter.Any())
            {
                foreach (var item in kpiRequest.Model.ProductFilter)
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
                    kpi.Result.AddFilter(filterItem.Result);
                }
            }
            if (kpiRequest.Model.CompanyFilter.Any())
            {
                foreach (var item in kpiRequest.Model.CompanyFilter)
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
                    kpi.Result.AddFilter(filterItem.Result);
                }
            }

            await unitOfWork.GetRepository<Kpi>().InsertAsync(kpi.Result, cancellationToken);
            var result = await unitOfWork.SaveChangesAsync();
            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return true;
        }
    }
}
