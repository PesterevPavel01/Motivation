using Motivation.Contracts;

namespace Motivation.HRWeb.Exceptions;

public class RedirectToLoginException : Exception
{
    public RedirectToLoginException() : base(ErrorMessages.Unauthorized){}
}
