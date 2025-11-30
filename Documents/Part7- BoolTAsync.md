# BoolT Async - The Async Extensions tp the BoolT Containor

No data pipeline is complete without async support. Here I show you how to add the async extensions to the BoolT container.

Before I start we need to consider how we want to handle exceptions. In this example I have chosen to catch any exceptions that occur during the execution of the Task and convert them into a Failure state within the BoolT container. This way, we can maintain the functional programming paradigm by treating exceptions as part of the computation flow rather than allowing them to propagate and potentially disrupt the program's execution.

To do so, there are a couple of helper methods that will check the Task for exceptions and convert them into the appropriate BoolT state.

The first unwraps a `Task<T>` and returns a `Bool<T>` if it completed successfully or an excpetion if it didn't.

```csharp
private static Bool<T> CheckForTaskException<T>(Task<T> @this)
    => @this.IsCompletedSuccessfully
        ? BoolT.Read(@this.Result)
        : Bool<T>.Failure(@this.Exception
            ?? new Exception("The Task failed to complete successfully"));
```

The second unwraps a `Task<Bool<T>>` and returns the result if it completed successfully or an excpetion if it didn't.

```csharp
    private static Bool<T> CheckForTaskException<T>(Task<Bool<T>> @this)
    => @this.IsCompletedSuccessfully
        ? @this.Result
        : Bool<T>.Failure(@this.Exception
            ?? new Exception("The Task failed to complete successfully"));
```

These will be applied as continuations to the async methods we define below.

## Sync to Async Switching

The first two are `Map` and `Bind`extensions on `Bool<T>` that switch from sync to async operations.

```csharp
public async Task<Bool<TOut>> BindAsync(Func<T, Task<Bool<TOut>>> function)
    => @this.HasValue
        ? await function(@this.Value!).ContinueWith(CheckForTaskException)
        : Bool<TOut>.Failure(@this.Exception!);

public async Task<Bool<TOut>> MapAsync(Func<T, Task<TOut>> function)
    => @this.HasValue
        ? await function(@this.Value!).ContinueWith(CheckForTaskException)
        : Bool<TOut>.Failure(@this.Exception!);
```

They check the current `Bool<T>` instance has a value.

1. On True, they invoke the provided async function and await its result, applying the `CheckForTaskException` method to handle any exceptions that may occur during the async operation.

1. On False, they propagate the existing exception by returning a new `Bool<TOut>` in failure state with the same exception.

## Chaining Async Operations

Next, we have the `Map` and `Bind` extensions on `Task<Bool<T>>` that allow chaining async operations.

```csharp
public async Task<Bool<TOut>> BindAsync(Func<T, Task<Bool<TOut>>> function)
    => await (await @this.ContinueWith(CheckForTaskException))
        .BindAsync(function);

public async Task<Bool<TOut>> MapAsync(Func<T, Task<TOut>> function)
    => await (await @this.ContinueWith(CheckForTaskException))
        .MapAsync(function);
```

These methods:

1. Await the `Task<Bool<T>>` to get the underlying `Bool<T>` instance, applying the `CheckForTaskException` method to handle any exceptions that may occur during the task execution.

1. Once the `Bool<T>` instance is obtained, they call the corresponding async `BindAsync` or `MapAsync` method defined earlier to continue the computation.

This allows for seamless chaining of async operations while maintaining the functional programming principles of the BoolT container.

## Async to Sync Switching

The final set of extensions are `Map` and `Bind` and `WRite` on `Task<T>` that switch from async back to sync operations.

```csharp
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
```

They all await the `Task<T>` to get the underlying value, applying the `CheckForTaskException` method to handle any exceptions that may occur during the task execution., before calling the corresponding sync method on the `Bool<T>` instance.

Finally a new ecyension method to handle the string async write:

```csharp
public static class Extensions
{
    extension(Task<string> @this)
    {
        public async Task WriteAsync(Action<string> writer)
            => writer.Invoke(await @this);
    }
}
```

And the console app looks like this:

```csharp
await DataProvider.GetDataAsync()
  .MapAsync(Math.Sqrt)
  .MapAsync(value => Math.Round(value, 2))
  .WriteAsync(
      hasValue: value => $"Success: The transformed value is: {value}",
      hasNoValue: () => "The input value could not be parsed.",
      hasException: ex => $"An error occurred: {ex.Message}")
  .WriteAsync(Console.WriteLine);
```

There are other ways to write this:

```csharp
(await DataProvider.GetDataAsync())
  .Map(Math.Sqrt)
  .Map(value => Math.Round(value, 2))
  .Write(
      hasValue: value => $"Success: The transformed value is: {value}",
      hasNoValue: () => "The input value could not be parsed.",
      hasException: ex => $"An error occurred: {ex.Message}")
  .Write(Console.WriteLine);
```