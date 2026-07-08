using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.AnalyticsCore.Contracts.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;

namespace Motivation.KpiCalculationService.Application.Messaging
{
    public class SyncSourceData
    {
        public record Request(SyncDto SyncModel) : IRequest<Operation<bool, string>>;

        public class Handler(
            IEntityServices<Product, ProductDto> productServices,
            IEntityServices<Customer, CustomerDto> customerServices,
            IEntityServices<Company, CompanyDto> companyServices,
            IEntityServices<Receivable, ReceivableDto> receivableServices,
            IEntityServices<CustomerTransaction, CustomerTransactionDto> customerTransactionServices,
            IUnitOfWork unitOfWork,
            ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<bool, string>>
        {
            public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
            {
                var companies = await companyServices.CreateOrEditAsync(request.SyncModel.Companies.ToList());
                if (!companies.Ok)
                    return Operation.Error(companies.Error);

                var result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }

                var products = await productServices.CreateOrEditAsync(request.SyncModel.Products.ToList());
                if (!products.Ok)
                    return Operation.Error(products.Error);

                result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }

                var customers = await customerServices.CreateOrEditAsync(request.SyncModel.Customers.ToList());
                if (!customers.Ok)
                    return Operation.Error(customers.Error);

                result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }

                using var receivableTransaction = await unitOfWork
                    .BeginTransactionAsync();

                var deleteReceivables = await receivableServices.DeleteAsync(request.SyncModel.DateStart, request.SyncModel.DateEnd);
                if (!deleteReceivables.Ok)
                    return Operation.Error(deleteReceivables.Error);
                result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    await receivableTransaction.RollbackAsync(cancellationToken);
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }
                
                var receivables = await receivableServices.CreateOrEditAsync(request.SyncModel.Receivables.ToList());
                if (!receivables.Ok)
                    return Operation.Error(receivables.Error);
                result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    await receivableTransaction.RollbackAsync(cancellationToken);
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }

                await receivableTransaction.CommitAsync(cancellationToken);

                using var CustomerTransactionsTransaction = await unitOfWork
                    .BeginTransactionAsync();

                var deleteCustomerTransactions = await customerTransactionServices.DeleteAsync(request.SyncModel.DateStart, request.SyncModel.DateEnd);
                if (!deleteCustomerTransactions.Ok)
                    return Operation.Error(deleteCustomerTransactions.Error);
                result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    await CustomerTransactionsTransaction.RollbackAsync(cancellationToken);
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }

                var customerTransactions = await customerTransactionServices.CreateOrEditAsync(request.SyncModel.CustomerTransactions.ToList());
                if (!customerTransactions.Ok)
                    return Operation.Error(customerTransactions.Error);
                result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    await CustomerTransactionsTransaction.RollbackAsync(cancellationToken);
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }

                await CustomerTransactionsTransaction.CommitAsync(cancellationToken);
                
                return true;
            }
        }
    }
}
