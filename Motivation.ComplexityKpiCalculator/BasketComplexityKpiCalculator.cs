using Calabonga.OperationResults;
using Microsoft.Extensions.Logging;
using Motivation.Contracts.Dto;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Enum;
using Python.Runtime;

namespace Motivation.KpiComplexityCalculator;

public sealed class BasketComplexityKpiCalculator : ICalculator
{
    private readonly ILogger<BasketComplexityKpiCalculator> _logger;
    private readonly string _pythonScriptPath;
    private bool _isInitialized;

    public KpiType KpiType => KpiType.Complexity;

    public BasketComplexityKpiCalculator(ILogger<BasketComplexityKpiCalculator> logger)
    {
        _pythonScriptPath = @"D:\Work\Dizag\Back\Motivation\source\Motivation.ComplexityKpiCalculator\PytonSctripts";
        _logger = logger;
        InitializePython();
    }

    public async Task<Operation<decimal, string>> CalculateKpiAsync(CalculateEmployeeKpiDto model)
    {
        if (!_isInitialized)
        {
            return Operation.Error("Python engine is not initialized. Check Python installation and scripts.");
        }

        var kpi = model.Employee.Positions
            .FirstOrDefault(x => x.Position?.Code == model.PositionCode)?
            .Position?.Kpis?.FirstOrDefault(x => x.KpiCode == model.KpiCode);

        if (kpi is null || kpi.Kpi is null)
        {
            return Operation.Error("Kpi not found!");
        }

        var result = ExecutePythonCalculation(
                model.Employee.Code,
                model.PositionCode,
                model.KpiCode,
                model.PeriodFrom,
                model.PeriodTo,
                kpi);

        return result;
    }

    private Operation<decimal, string> ExecutePythonCalculation(
    string employeeCode,
    string positionCode,
    string kpiCode,
    DateTime periodFrom,
    DateTime periodTo,
    PositionKpiDto kpi)
    {
        try
        {
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(_pythonScriptPath);

                dynamic calculatorModule = Py.Import("complexity_calculator");

                var data = new PyDict();
                data["employee_code"] = new PyString(employeeCode);
                data["position_code"] = new PyString(positionCode);
                data["kpi_code"] = new PyString(kpiCode);
                data["period_from"] = new PyString(periodFrom.ToString("yyyy-MM-dd"));
                data["period_to"] = new PyString(periodTo.ToString("yyyy-MM-dd"));
                data["base_value"] = new PyFloat(1000.0);

                dynamic pyResult = calculatorModule.calculate_complexity_kpi(data);
                var result = Convert.ToDecimal((double)pyResult);

                _logger?.LogInformation(
                    "Python calculation result for KPI {KpiCode}: {Value}",
                    kpiCode,
                    result);

                return result;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Python calculation failed");
            return Operation.Error($"Python calculation failed: {ex.Message}");
        }
    }

    private void InitializePython()
    {
        try
        {
            var scriptFile = Path.Combine(_pythonScriptPath, "complexity_calculator.py");

            if (!File.Exists(scriptFile))
            {
                _logger?.LogWarning($"Python script not found at: {scriptFile}");
                _isInitialized = false;
                return;
            }

            if (!PythonEngine.IsInitialized)
            {
                var pythonDll = Environment.GetEnvironmentVariable("PYTHON_DLL");
                if (!string.IsNullOrEmpty(pythonDll))
                {
                    Runtime.PythonDLL = pythonDll;
                }
                else
                {
                    Runtime.PythonDLL = @"C:\Users\Dupree\AppData\Local\Programs\Python\Python314\python314.dll";
                }

                _logger.LogInformation($"Using Python DLL: {Runtime.PythonDLL}");

                PythonEngine.Initialize();

                using (Py.GIL())
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_pythonScriptPath);

                    _logger.LogInformation($"Python path: {_pythonScriptPath}");
                    _logger.LogInformation($"Python version: {sys.version}");
                }
            }

            _isInitialized = true;
            _logger.LogInformation("Python engine initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Python engine");
            _isInitialized = false;
        }
    }
}