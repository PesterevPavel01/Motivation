namespace Motivation.AnalyticsCore.Contracts.Models;

public class CustomerDto
{
    public string? FullName { get; set; }
    public required string Code { get; set; }
    public required string Title { get; set; }
    public string? Manager { get; set; }
    public CustomerDto? MasterCustomer { get; set; }
    public CustomerGroupDto? CustomerGroup { get; set; }
}
