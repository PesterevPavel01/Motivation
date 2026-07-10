using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Services;

public class CompanyService : IEntityServices<CompanyDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Operation<bool, string>> CreateOrEditAsync(Dictionary<string, CompanyDto> models, DateTime dateStart, DateTime dateEnd, CancellationToken cancellationToken)
    {
        if (models.Values.Any(c => c is null || string.IsNullOrWhiteSpace(c.Code)))
            return Operation.Error("Company (or company code) is null or white space!");

        var companyCodes = models.Values
            .Select(x => CodeValue.Create(x.Code));

        if (companyCodes.Any(x => !x.Ok))
            return Operation.Error(companyCodes.FirstOrDefault(x => !x.Ok).Error);

        var existingCompany = (await _unitOfWork
            .GetRepository<Company>()
            .GetAllAsync(
                predicate: x => companyCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToList();

        var newCompany = models.Values
            .Where(x => !existingCompany.Select(p => p.Code.Value).Contains(x.Code))
            .ToArray();

        List<Company> companies = [];
        foreach (var model in newCompany)
        {
            var company = CompanyMapper.ToEntity(model);
            if (!company)
                return Operation.Error(company.Error);
            companies.Add(company.Result);
        }

        _unitOfWork.GetRepository<Company>()
            .Insert(companies);

        var editResult = Edit(models, existingCompany);
        if (!editResult)
            return Operation.Error(editResult.Error);

        _unitOfWork.GetRepository<Company>()
            .Update(existingCompany);

        return true;
    }

    public Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
    {
        throw new NotImplementedException();
    }

    private Operation<bool, string> Edit(Dictionary<string,CompanyDto> models, List<Company> entities)
    {
        List<Customer> companies = [];
        foreach (var entity in entities)
        {
            if (!models.TryGetValue(entity.Code.Value, out var model))
                return Operation.Error("Company not found!");
            if (model is null)
                return Operation.Error($"Model {typeof(CompanyDto).Name} not found!");
            var editResult = entity.EditEntity(model);
            if (!editResult.Ok)
                return Operation.Error(editResult.Error);
        }
        return true;
    }
}
