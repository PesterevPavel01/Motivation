namespace Motivation.Contracts;

public sealed class ErrorMessages
{
    public const string NotFound = "Not found!";
    public const string InternalServerError = "Internal server error";
    public const string Unauthorized = "Unauthorized";
    public const string Forbidden = "Forbidden";

    public const string EmployeeNotFound = "Employee not found";
    public const string EmployeeAlreadyExists = "Employee already exists";

    public const string PositionNotFound = "Position not found";

    public const string StandardHoursNotFound = "Standard hours not found for the selected period";
    public const string StandardHoursAlreadyExists = "Standard hours already defined for this period";

    public const string InvalidData = "Invalid data provided";
}
