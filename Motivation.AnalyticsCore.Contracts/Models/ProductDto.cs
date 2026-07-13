namespace Motivation.AnalyticsCore.Contracts.Models;

public sealed class ProductDto
{
    public string? Supplier { get; set; }
    public string? Purchaser { get; set; }
    public required string Code { get; set; }
    public required string Title { get; set; }
    public string? Status { get; set; }
    public WarehouseDto? Warehouse { get; set; }
    public ProductGroupDto? ProductGroup { get; set; }
}