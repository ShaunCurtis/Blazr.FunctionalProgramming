/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolTAsyncExtensions
{
    extension<T, TOut>(Bool<T> @this)
    {
        public async Task<Bool<TOut>> BindAsync(Func<T, Task<Bool<TOut>>> function)
            => @this.HasValue
                ? await function(@this.Value!).ContinueWith(CheckForTaskException)
                : Bool<TOut>.Failure(@this.Exception!);

        public async Task<Bool<TOut>> MapAsync(Func<T, Task<TOut>> function)
            => @this.HasValue
                ? await function(@this.Value!).ContinueWith(CheckForTaskException)
                : Bool<TOut>.Failure(@this.Exception!);
    }

    extension<T, TOut>(Task<Bool<T>> @this)
    {
        public async Task<Bool<TOut>> BindAsync(Func<T, Task<Bool<TOut>>> function)
            => await (await @this.ContinueWith(CheckForTaskException))
                .BindAsync(function);

        public async Task<Bool<TOut>> MapAsync(Func<T, Task<TOut>> function)
            => await (await @this.ContinueWith(CheckForTaskException))
                .MapAsync(function);
    }

    extension<T, TOut>(Task<Bool<T>> @this)
    {
        public async Task<Bool<TOut>> BindAsync(Func<T, Bool<TOut>> function)
            => (await @this.ContinueWith(CheckForTaskException))
                .Bind(function);

        public async Task<Bool<TOut>> MapAsync(Func<T, TOut> function)
            => (await @this.ContinueWith(CheckForTaskException))
                .Map<T, TOut>(function);

        public async Task<Bool<TOut>> TryMapAsync(Func<T, TOut> function)
            => (await @this.ContinueWith(CheckForTaskException))
                .TryMap<T, TOut>(function);

        public async Task<T> WriteAsync(T ExceptionOutput)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(ExceptionOutput);

        public async Task WriteAsync(Action<T>? hasValue = null, Action? hasNoValue = null, Action<Exception>? hasException = null)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(hasValue, hasNoValue, hasException);

        public async Task<TOut> WriteAsync(Func<T, TOut> hasValue, Func<TOut> hasNoValue, Func<Exception, TOut> hasException)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(hasValue, hasNoValue, hasException);

    }

    private static Bool<T> CheckForTaskException<T>(Task<T> @this)
        => @this.IsCompletedSuccessfully
            ? BoolT.Read(@this.Result)
            : Bool<T>.Failure(@this.Exception
                ?? new Exception("The Task failed to complete successfully"));

    private static Bool<T> CheckForTaskException<T>(Task<Bool<T>> @this)
    => @this.IsCompletedSuccessfully
        ? @this.Result
        : Bool<T>.Failure(@this.Exception
            ?? new Exception("The Task failed to complete successfully"));
}