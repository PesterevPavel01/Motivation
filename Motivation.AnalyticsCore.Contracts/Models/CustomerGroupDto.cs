namespace Motivation.AnalyticsCore.Contracts.Models;

public class CustomerGroupDto
{
    public required string Title { get; set; }
    public required string Code { get; set; }
    public CustomerGroupDto? ParentGroup { get; set; }
}
