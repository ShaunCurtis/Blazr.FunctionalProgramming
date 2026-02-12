/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.ResultMonad;

public record Result
{
    public bool Success { get; private init; }

    public string? Message { get; private init; }

    public void Match(Action success, Action<string> fail)
    {
        if (Success)
            success.Invoke();
        else
            fail.Invoke(this.Message!);
    }

    private Result(string message)
    {
        Success = false;
        Message = message;
    }

    private Result()
    {
        Success = true;
    }

    public static Result Succeeded => new Result();
    public static Result Failed(string message) => new Result(message);
}
