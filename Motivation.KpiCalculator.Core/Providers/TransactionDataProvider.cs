using Calabonga.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.KpiCalculator.Core.Interfaces;
using Motivation.AnalyticsCore.Application.Mappers;

namespace Motivation.KpiCalculator.Core.Providers;

public class TransactionDataProvider : ITransactionDataProvider<CustomerTransactionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public TransactionDataProvider(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CustomerTransactionDto>> GetTransactionsAsync(
        DateTime periodFrom,
        DateTime periodTo,
        CancellationToken cancellationToken = default)
    {
        var transactionRepository = _unitOfWork.GetRepository<CustomerTransaction>();

        var transactions = (await transactionRepository
            .GetAllAsync(
                predicate: ct =>
                    ct.AsOfDate >= periodFrom &&
                    ct.AsOfDate <= periodTo,
                include: query => query
                    .Include(ct => ct.Customer)
                        .ThenInclude(c => c.CustomerGroup!)
                            .ThenInclude(cg => cg.ParentGroup!)
                                .ThenInclude(cg => cg.ParentGroup!)
                                    .ThenInclude(cg => cg.ParentGroup!)
                                        .ThenInclude(cg => cg.ParentGroup!)
                                            .ThenInclude(cg => cg.ParentGroup!)
                    .Include(ct => ct.Customer)
                        .ThenInclude(c => c.MasterCustomer)
                    .Include(ct => ct.Company)
                    .Include(ct => ct.CustomerTransactionProducts)
                        .ThenInclude(ctp => ctp.Product)
                            .ThenInclude(p => p.ProductGroup!)
                                .ThenInclude(pg => pg.ParentGroup!)
                                    .ThenInclude(pg => pg.ParentGroup!)
                                        .ThenInclude(pg => pg.ParentGroup!)
                                            .ThenInclude(pg => pg.ParentGroup!)))
            .ToList();

        return transactions
            .Select(x => x.ToDto())
            .ToList();
    }
}
