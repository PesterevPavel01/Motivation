namespace Motivation.AnalyticsCore.Contracts.Models;

public class ProductGroupDto
{
    public required string Title { get; set; }
    public required string Code { get; set; }
    public ProductGroupDto? ParentGroup { get; set; }
}