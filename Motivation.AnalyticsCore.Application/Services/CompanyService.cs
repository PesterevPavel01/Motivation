using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Services;

public class CompanyService : IEntityServices<Company, CompanyDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Operation<List<Company>, string>> CreateOrEditAsync(List<CompanyDto> models)
    {
        if (models.Any(c => c is null || string.IsNullOrWhiteSpace(c.Code)))
            return Operation.Error("Company (or company code) is null or white space!");

        var companyCodes = models
            .Select(x => CodeValue.Create(x.Code));

        if (companyCodes.Any(x => !x.Ok))
            return Operation.Error(companyCodes.FirstOrDefault(x => !x.Ok).Error);

        var existingCompany = (await _unitOfWork
            .GetRepository<Company>()
            .GetAllAsync(
                predicate: x => companyCodes.Select(x => x.Result).Contains(x.Code),
                trackingType: TrackingType.Tracking))
            .ToList();

        var newCompany = models
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

        companies.AddRange(existingCompany);
        return companies;
    }

    public Task<Operation<bool, string>> DeleteAsync(DateTime dateStart, DateTime dateEnd)
    {
        throw new NotImplementedException();
    }

    private Operation<bool, string> Edit(List<CompanyDto> models, List<Company> entities)
    {
        List<Customer> customers = [];
        foreach (var entity in entities)
        {
            var model = models.FirstOrDefault(x => x.Code == entity.Code.Value);
            if (model is null)
                return Operation.Error($"Model {typeof(CustomerDto).Name} not found!");
            var editResult = entity.EditEntity(model);
            if (!editResult.Ok)
                return Operation.Error(editResult.Error);
        }
        return true;
    }
}
