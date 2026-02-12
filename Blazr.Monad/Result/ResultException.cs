namespace Blazr.Monad;

public class ResultException : Exception
{
    public ResultException(string message)
        : base(message) { }

    public static ResultException Create(string message) => new ResultException(message);
    public static ResultException Null => new ResultException("The input value was null");
}
