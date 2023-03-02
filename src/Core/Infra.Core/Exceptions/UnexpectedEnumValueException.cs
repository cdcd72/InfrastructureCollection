namespace Infra.Core.Exceptions;

public class UnexpectedEnumValueException : Exception
{
    public UnexpectedEnumValueException(string message) : base(message)
    {

    }
}
