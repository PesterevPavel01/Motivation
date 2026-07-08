namespace Motivation.AnalyticsCore.Contracts.Models;

public sealed class WarehouseDto
{
    public required string Code { get; set; }
    public required string Title { get; set; }
    public List<WarehouseTransactionDto> WarehouseTransactions { get; set; } = [];
}