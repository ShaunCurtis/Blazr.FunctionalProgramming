/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.ResultMonad;

public static class ResultFunctionalExtensions
{
    extension<T>(Result<T> @this)
    {
        public Result<TResult> Then<TResult>(Func<T, TResult> func)
        {
            try
            {
                return @this switch
                {
                    SuccessResult<T> hv => ResultT.Read(func.Invoke(hv.Value)),
                    _ => new FailureResult<TResult>(ResultException.Null)
                };
            }
            catch (Exception e)
            {
                return new FailureResult<TResult>(e);
            }
        }

        public void Write(Action<T> success, Action<Exception> failure)
        {
            switch (@this)
            {
                case SuccessResult<T> s:
                    success.Invoke(s.Value);
                    break;
                case FailureResult<T> f:
                    failure.Invoke(f.Exception);
                    break;
            }
        }

        public T Write(T failureValue)
            => @this switch
            {
                SuccessResult<T> s => s.Value,
                FailureResult<T> f => failureValue,
                _ => throw new NotImplementedException("Result Object type is undefined.")
            };
    }

    public static Result<T> ToResult<T>(this T value) =>
        value is null ? new SuccessResult<T>(value) : new FailureResult<T>(ResultException.Null);
}

