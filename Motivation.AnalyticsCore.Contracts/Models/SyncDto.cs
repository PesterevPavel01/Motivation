namespace Motivation.AnalyticsCore.Contracts.Models;

public class SyncDto
{
    public IEnumerable<CompanyDto> Companies { get; set; } = [];
    public IEnumerable<ProductDto> Products { get; set; } = [];
    public IEnumerable<CustomerDto> Customers { get; set; } = [];
    public IEnumerable<ReceivableDto> Receivables { get; set; } = [];
    public IEnumerable<CustomerTransactionDto> CustomerTransactions { get; set; } = [];
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
}
