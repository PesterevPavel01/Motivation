using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Services;

public class ReceivableService : IEntityServices<ReceivableDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReceivableService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, ReceivableDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        var customerCodes = models.Values
            .GroupBy(x => x.Customer.Code)
            .Select(x => CodeValue.Create(x.Key));
        if (customerCodes.Any(x => !x.Ok))
            return Operation.Error(customerCodes.FirstOrDefault(x => !x.Ok).Error);

        var companyCodes = models.Values
            .GroupBy(x => x.Company.Code)
            .Select(x => CodeValue.Create(x.Key))
            .ToArray();
        if (companyCodes.Any(x => !x.Ok))
            return Operation.Error(companyCodes.FirstOrDefault(x => !x.Ok).Error);

        var customers = (await _unitOfWork
            .GetRepository<Customer>()
            .GetAllAsync(
                predicate: x => customerCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value.Trim(),
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        var companies = (await _unitOfWork
            .GetRepository<Company>()
            .GetAllAsync(
                predicate: x => companyCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToDictionary(
                x => x.Code.Value,
                x => x,
                StringComparer.OrdinalIgnoreCase
            );

        List<Receivable> receivables = [];

        foreach (var model in models.Values)
        {
            if (!companies.TryGetValue(model.Company.Code, out var company))
                return Operation.Error("Company not found!");
            if (company is null)
                return Operation.Error("Company is null!");

            if (!customers.TryGetValue(model.Customer.Code, out var customer))
                return Operation.Error("Customer not found!");
            if (customer is null)
                return Operation.Error("Customer is null!");

            var receivable = ReceivableMapper.ToEntity(model, company, customer);
            if (!receivable.Ok)
                return Operation.Error(receivable.Error);
            receivables.Add(receivable.Result);
        }

        _unitOfWork.GetRepository<Receivable>()
            .Insert(receivables);
        return true;
    }

    public async Task<Operation<bool, string>> DeleteAsync(List<ReceivableDto> models)
    {
        var customerCodes = models
              .Select(x => new { Code = CodeValue.Create(x.Customer.Code), x.AsOfDate });

        if (customerCodes.Any(x => !x.Code.Ok))
            return Operation.Error(customerCodes.FirstOrDefault(x => !x.Code.Ok)?.Code.Error ?? "Необработанная ошибка!");

        var existingCustomers = (await _unitOfWork
            .GetRepository<Customer>()
            .GetAllAsync(
                predicate: x => customerCodes.Select(x => x.Code.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToList();

        var receivables = await _unitOfWork
            .GetRepository<Receivable>()
                .GetAllAsync(
                    predicate: x => customerCodes.Any(c =>
                        c.Code.Result == x.Customer.Code
                        && c.AsOfDate == x.AsOfDate),
                    trackingType: TrackingType.Tracking);

        _unitOfWork.GetRepository<Receivable>().Delete(receivables);

        return true;
    }

    public async Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
    {
        var receivables = await _unitOfWork
            .GetRepository<Receivable>()
            .GetAllAsync(
                predicate: x => x.AsOfDate >= dateStart && x.AsOfDate <= dateEnd,
                trackingType: TrackingType.Tracking,
                include: null);

        _unitOfWork.GetRepository<Receivable>().Delete(receivables);

        return true;
    }
}
