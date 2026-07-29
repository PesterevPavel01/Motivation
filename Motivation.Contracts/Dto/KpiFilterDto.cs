using Motivation.Domain.Enum;

namespace Motivation.Contracts.Dto;

public sealed record KpiFilterDto(string Title, bool Include, KpiFilterType Type);
