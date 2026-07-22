using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.KpiService.Application.Messaging.Employees;

public class GetEmployeeKpis
{
    public record Request(string EmployeeCode) : IRequest<Operation<EmployeeDto, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger, IServiceProvider serviceProvider)
        : IRequestHandler<Request, Operation<EmployeeDto, string>>
    {
        public async ValueTask<Operation<EmployeeDto, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var employeeCode = CodeValue.Create(request.EmployeeCode);
            if (!employeeCode.Ok)
            {
                return Operation.Error(employeeCode.Error);
            }

            var repository = unitOfWork.GetRepository<Employee>();

            var employee = await unitOfWork
                .GetRepository<Employee>()
                    .GetFirstOrDefaultAsync(
                        predicate: x => 
                            x.Enabled == true
                            && x.Code == employeeCode.Result,
                        include: query => query
                            .Include(x => x.TargetHistory)
                            .Include(x => x.EmployeePositions)
                                .ThenInclude(x => x.Position)
                                    .ThenInclude(x => x.PositionKpiHistory)
                                        .ThenInclude(x => x.Kpi)
                                            .ThenInclude(x => x.MeasurementUnit)
                            .Include(x => x.EmployeePositions)
                                .ThenInclude(x => x.Position)
                                    .ThenInclude(x => x.PositionKpiHistory)
                                        .ThenInclude(x => x.TargetHistory)
                                            .ThenInclude(x => x.KpiFact)
                            .Include(x => x.EmployeePositions)
                                .ThenInclude(x => x.Position)
                                    .ThenInclude(x => x.PositionKpiHistory)
                                        .ThenInclude(x => x.TargetHistory)
                                            .ThenInclude(x => x.Employee)
                            .Include(x => x.EmployeePositions)
                                .ThenInclude(x => x.ActualHoursHistory)
                            .Include(x => x.EmployeePositions)
                                .ThenInclude(x => x.ExtraPartHistory)
                            .Include(x => x.EmployeePositions)
                                .ThenInclude(x => x.DeductionHistory));

            if (employee is null)
            {
                return Operation.Error("Employee not found!");
            }

            return employee.MapToDto();
        }
    }
}
