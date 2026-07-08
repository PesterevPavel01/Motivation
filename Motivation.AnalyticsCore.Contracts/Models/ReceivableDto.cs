namespace Motivation.AnalyticsCore.Contracts.Models;

public class ReceivableDto
{
    public decimal Amount { get; set; }
    public required CustomerDto Customer { get; set; }
    public required DateTime AsOfDate { get; set; }
    public required CompanyDto Company { get; set; }
}
