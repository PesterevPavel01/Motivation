using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;
using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Interfaces;
using Motivation.KpiCalculator.Core.Specifications;

namespace Motivation.KpiCalculator.Core;

public class KpiQueryBuilder : IKpiQueryBuilder
{
    private readonly ITransactionDataProvider _dataProvider;
    private readonly IHierarchyService _hierarchyService;

    private DateTime _periodFrom;
    private DateTime _periodTo;
    private ISpecification<CustomerTransaction> _customerSpec = new TrueSpecification<CustomerTransaction>();
    private ISpecification<CustomerTransaction> _companySpec = new TrueSpecification<CustomerTransaction>();
    private ISpecification<CustomerTransactionProduct> _productSpec = new TrueSpecification<CustomerTransactionProduct>();
    private ISpecification<CustomerTransactionProduct> _productStatusSpec = new TrueSpecification<CustomerTransactionProduct>();
    private ISpecification<CustomerTransactionProduct> _productSupplierSpec = new TrueSpecification<CustomerTransactionProduct>();
    private ISpecification<CustomerTransaction> _customerManagerSpec = new TrueSpecification<CustomerTransaction>();
    private ISpecification<CustomerTransaction> _salesResponsibleSpec = new TrueSpecification<CustomerTransaction>();
    private ISpecification<CustomerTransaction> _productResponsibleSpec = new TrueSpecification<CustomerTransaction>();

    public KpiQueryBuilder(
        ITransactionDataProvider dataProvider,
        IHierarchyService hierarchyService)
    {
        _dataProvider = dataProvider;
        _hierarchyService = hierarchyService;
    }

    public IKpiQueryBuilder SetPeriod(DateTime from, DateTime to)
    {
        _periodFrom = from;
        _periodTo = to;
        return this;
    }

    public IKpiQueryBuilder ApplyCustomerFilter(IEnumerable<KpiFilterDto> filters)
    {
        _customerSpec = FilterSpecifications.BuildCustomerFilterSpecification(
            filters,
            _hierarchyService.GetCustomerGroupHierarchy);
        return this;
    }

    public IKpiQueryBuilder ApplyProductFilter(IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null)
        {
            filters = [];
        }
        _productSpec = FilterSpecifications.BuildProductFilterSpecification(
            filters,
            _hierarchyService.GetProductGroupHierarchy);
        return this;
    }

    public IKpiQueryBuilder ApplyProductSupplierFilter(IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null)
        {
            filters = [];
        }
        _productSupplierSpec = FilterSpecifications.BuildProductSupplierFilterSpecification(filters);
        return this;
    }

    public IKpiQueryBuilder ApplyProductStatusFilter(IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null)
        {
            filters = [];
        }
        _productStatusSpec = FilterSpecifications.BuildStatusProductFilterSpecification(filters);
        return this;
    }

    public IKpiQueryBuilder ApplyCompanyFilter(IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null)
        {
            filters = [];
        }
        _companySpec = FilterSpecifications.BuildCompanyFilterSpecification(filters);
        return this;
    }

    public IKpiQueryBuilder ApplyResponsibleFilters(
        IEnumerable<KpiFilterDto> customerResponsibleFilter,
        IEnumerable<KpiFilterDto> salesResponsibleFilter,
        IEnumerable<KpiFilterDto> productResponsibleFilter)
    {
        var managerValues = new List<ManagerValue>();
        if (customerResponsibleFilter is not null)
        {
            foreach (var filter in customerResponsibleFilter)
            {
                var manager = ManagerValue.Create(filter.Title);
                if (manager.Ok)
                    managerValues.Add(manager.Result);
            }
        }
        _customerManagerSpec = new CustomerManagerSpecification(managerValues);

        var responsibleValues = new List<ResponsibleValue>();
        if (salesResponsibleFilter is not null)
        {
            foreach (var filter in salesResponsibleFilter)
            {
                var responsible = ResponsibleValue.Create(filter.Title);
                if (responsible.Ok)
                    responsibleValues.Add(responsible.Result);
            }
        }
        _salesResponsibleSpec = new SalesResponsibleSpecification(responsibleValues);

        var purchaserValues = new List<PurchaserValue>();
        if (productResponsibleFilter is not null)
        {
            foreach (var filter in productResponsibleFilter)
            {
                var purchaser = PurchaserValue.Create(filter.Title);
                if (purchaser.Ok)
                    purchaserValues.Add(purchaser.Result);
            }
        }
        _productResponsibleSpec = new ProductResponsibleSpecification(purchaserValues);

        return this;
    }

    public async Task<KpiQueryResult> BuildAsync(CancellationToken cancellationToken = default)
    {
        var transactions = await _dataProvider.GetTransactionsAsync(
            _periodFrom,
            _periodTo,
            cancellationToken);

        var filteredTransactions = transactions.AsEnumerable();

        var resultList = ApplyAllSpecifications(filteredTransactions);

        return new KpiQueryResult
        {
            Transactions = resultList
        };
    }

    private IEnumerable<CustomerTransactionDto> ApplyAllSpecifications(
        IEnumerable<CustomerTransaction> transactions)
    {
        var result = transactions;

        result = result.Where(x => _customerSpec.IsSatisfiedBy(x)).ToArray();
        result = result.Where(x => _companySpec.IsSatisfiedBy(x)).ToArray();

        result = result.Where(x => _customerManagerSpec.IsSatisfiedBy(x)).ToArray();
        result = result.Where(x => _salesResponsibleSpec.IsSatisfiedBy(x)).ToArray();
        result = result.Where(x => _productResponsibleSpec.IsSatisfiedBy(x)).ToArray();

        var resultModel = result
        .Select(ct => CustomerTransactionMapper.ToDto(ct, product =>
        {
            return _productStatusSpec.IsSatisfiedBy(product)
                && _productSpec.IsSatisfiedBy(product)
                && _productSupplierSpec.IsSatisfiedBy(product);
        }))
        .Where(dto => dto.TransactionProducts.Any()).ToArray();

        return resultModel;
    }
}