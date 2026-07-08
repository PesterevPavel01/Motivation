namespace Motivation.AnalyticsCore.Contracts.Models;

public sealed class CustomerTransactionDto
{
    public required string Code { get; set; }
    public string? Title { get; set; }
    public decimal Amount { get; set; }
    public required CustomerDto Customer { get; set; }
    public required DateTime AsOfDate { get; set; }
    public required string Responsible { get; set; }
    public required string Type { get; set; }
    public List<TransactionProductDto> TransactionProducts { get; set; } = [];
    public required CompanyDto Company { get; set; }
}