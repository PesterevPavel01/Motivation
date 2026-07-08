using Mediator;
using Motivation.Contracts.Dto.CalculateDto;

namespace Motivation.KpiService.Application.Commands;

public sealed record RecalculateKpisCommand(CalculateKpiDto Model) : INotification { }
