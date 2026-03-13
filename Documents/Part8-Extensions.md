# C# Extension Methods

> A short note on the use of extension methods

You will have noticed that I use extension methods extensively throughout the code.

`ResultT` looks like this:

```csharp
public abstract record Result<T> { }

public record SuccessResult<T>(T Value) : Result<T>;
public record FailureResult<T>(Exception Exception) : Result<T>;
```

And then the Monad functionality is added like this:

```csharp
public static class ResultTMonadExtensions
{
    extension<T>(Result<T> @this)
    {
        public Result<TResult> Map<TResult>(Func<T, TResult> func);
        public Result<T> Match(Action<T>? success = null, Action<Exception>? failure = null);
        public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> func);
    }
}
```

Why?

1. We are dealing with immutable objects.  Functions take the initial object and output a new object.  They don't manipulate the internal data within the object.  It seems right to me to separate separate data from functionality.

2. Add and remove functionality easily.  Static classes containing extension methods can be in different namespaces.

You should also see the definition of a generic property added to all objects:

```csharp
public static class ResultFunctionalExtensions
{
    extension<T>(T @this)
    {
        public Result<T> ToResultT =>
            @this is not null ? new SuccessResult<T>(@this) : new FailureResult<T>(ResultException.Null);
    }

    //.. More methods
}
```
