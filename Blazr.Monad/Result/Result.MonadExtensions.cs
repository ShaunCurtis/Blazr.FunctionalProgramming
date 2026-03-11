/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultMonadExtensions
{
    extension<T>(Result<T> @this)
    {
        public Result<TResult> Bind<TResult>(Func<T, TResult> func)
        {
            try
            {
                if (@this is FailureResult<T> f)
                    return new FailureResult<TResult>(f.Exception);

                var previousSuccessValue = ((SuccessResult<T>)@this).Value;
                return func(previousSuccessValue).ToResultT;
            }
            catch (Exception e)
            {
                return new FailureResult<TResult>(e);
            }
        }
    }
}

