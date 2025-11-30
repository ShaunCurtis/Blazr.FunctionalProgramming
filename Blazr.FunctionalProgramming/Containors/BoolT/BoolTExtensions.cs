/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolTExtensions
{
    extension<T>(Bool<T> @this)
    {
        public Bool<TOut> Bind<TOut>(Func<T, Bool<TOut>> function)
            => @this.HasValue
                ? function(@this.Value)
                : Bool<TOut>.Failure(@this.Exception);

        public Bool<TOut> Map<TOut>(Func<T, TOut> function)
             => @this.HasValue
                ? BoolT.Read(function.Invoke(@this.Value!)) 
                    ?? Bool<TOut>.Failure(new BoolTException("The function returned a null value."))
                : Bool<TOut>.Failure(@this.Exception!);


        public Bool<TOut> TryMap<TOut>(Func<T, TOut> function)
        {
            try
            {
                return @this.HasValue
                    ? BoolT.Read(function.Invoke(@this.Value)) 
                            ?? Bool<TOut>.Failure(new BoolTException("The function returned a null value."))
                    : Bool<TOut>.Failure(@this.Exception);
            }
            catch (Exception ex)
            {
                return Bool<TOut>.Failure(ex);
            }
        }

        public void Write(Action<T>? hasValue = null, Action? hasNoValue = null, Action<Exception>? hasException = null)
        {
            switch (@this.HasValue, @this.HasException)
            {
                case (true, _) when hasValue is not null:
                    hasValue.Invoke(@this.Value!);
                    break;
                case (false, false) when hasNoValue is not null:
                    hasNoValue.Invoke();
                    break;
                case (false, true) when hasException is not null:
                    hasException.Invoke(@this.Exception!);
                    break;
            }
        }

        public TOut Write<TOut>(Func<T, TOut> hasValue, Func<TOut> hasNoValue, Func<Exception, TOut> hasException)
            => (@this.HasValue, @this.HasException) switch
            {
                (true, _) => hasValue.Invoke(@this.Value!),
                (false, false) => hasNoValue.Invoke(),
                (false, true) => hasException.Invoke(@this.Exception!)
            };

        public T Write(T defaultValue)
            => @this.HasValue
                ? @this.Value
                : defaultValue;
    }
}
