/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public class BoolTException : Exception
{
    public BoolTException() : base("The Bool operation failed.") { }
    public BoolTException(string message) : base(message) { }

    public static BoolTException Create(string message)
        => new (message);
}
