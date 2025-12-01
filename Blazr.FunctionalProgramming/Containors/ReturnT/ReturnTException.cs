/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public class ReturnTException : Exception
{
    public ReturnTException() : base("The Bool operation failed.") { }
    public ReturnTException(string message) : base(message) { }

    public static ReturnTException Create(string message)
        => new (message);
}
|