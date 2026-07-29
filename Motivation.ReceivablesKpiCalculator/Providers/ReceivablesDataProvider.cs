using Calabonga.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Application.Mappers;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.ReceivablesKpiCalculator.Providers;

internal class ReceivablesDataProvider : ITransactionDataProvider<ReceivableDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public ReceivablesDataProvider(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ReceivableDto>> GetTransactionsAsync(
        DateTime periodFrom,
        DateTime periodTo,
        CancellationToken cancellationToken = default)
    {
        var receivable = _unitOfWork.GetRepository<Receivable>();

        var transactions = (await receivable
            .GetAllAsync(
                predicate: ct =>
                    ct.AsOfDate >= periodFrom 
                    && ct.AsOfDate <= periodTo
                    && ct.Customer.MasterCustomer != null,
                include: query => query
                    .Include(r => r.Customer)
                        .ThenInclude(c => c.MasterCustomer)
                            .ThenInclude(mc => mc!.CustomerGroup!)
                                .ThenInclude(cg => cg.ParentGroup!)
                                    .ThenInclude(cg => cg.ParentGroup!)
                                        .ThenInclude(cg => cg.ParentGroup!)
                                            .ThenInclude(cg => cg.ParentGroup!)
                                                .ThenInclude(cg => cg.ParentGroup!)
                    .Include(r => r.Company)))
            .ToList();

        return transactions.Select(x=>x.ToDto());
    }
}
