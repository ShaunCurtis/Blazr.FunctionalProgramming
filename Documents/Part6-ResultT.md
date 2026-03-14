# `Result<T>` - A Multistate Container

In Parts 1 through to 5 you learnt about *Containers*, *Monads* and *Functors* and how to build a simple *Container* that implements the *Monad* and *Functor* patterns.

In this article I'll demonstrate how to build a fully functional multistate *container*.

It's called `Result<T>` and handles both:

1. A Nullable value of `T`. 
1. Exceptions.

It provides a mechanism to flow errors and exceptions from input to output: ideal for data pipelines.

## The Demo Project

The starting point is:

```csharp
var input = Console.ReadLine();

if (input is null)
    Console.WriteLine("You must enter a value.");

if (double.TryParse(input, out double value))
{
    value = Math.Sqrt(value);
    value = double.Round(value, 2);

    Console.WriteLine($"The square root is {value}.");
}
else
{
    Console.WriteLine("The value entered was not a number.");
}
```

There are three possible states:

1. Value
2. Null
3. Exception

We can compress this into two states using a custom exception to represent the Null state:

1. Success
1. Failure

### ResultException

First we need the custom result exception:

```csharp
public class ResultException : Exception
{
    public ResultException(string message)
        : base(message) { }

    public static ResultException Create(string message) => new ResultException(message);
    public static ResultException Null => new ResultException("The input value was null");
}
```

### `Result<T>`

`ResultT` uses the discriminated union pattern to define the two states.  It's an abstract record with two derived records: `SuccessResult<T>` and `FailureResult<T>`.  This is a common pattern in *FP* languages, but not so in C#.  It provides a clean way to define the two states:  

1. Each state defines only the values it requires.
1. State properties are not required: the type indicates success or failure.  `SuccessResult<T>` contains the value of type `T`, while `FailureResult<T>` contains an exception.
1. No nullables: the type system ensures values are always present.  Code is safer and more robust.

The abstract base object:

```csharp
public abstract record Result<T> { }
```

And two *State* objects:

```csharp
public record SuccessResult<T>(T Value) : Result<T>;
public record FailureResult<T>(Exception Exception) : Result<T>;
```

#### Constructors


*Newing up* is OK, but a set of static `ResultT` constructors provide better ways. 

Some examples:

Accepting a Nullable `T` and creating the correct object type:

```csharp
public static Result<T> Read<T>(T? value)
        => value is not null ? new SuccessResult<T>(value!) : new FailureResult<T>(ResultException.Null);
```

Accepting a Nullable `T` and a custom error message:

```csharp
public static Result<T> Read<T>(T? value, string exceptionMessage)
        => value is not null ? new SuccessResult<T>(value!) : new FailureResult<T>(ResultException.Create(exceptionMessage));
```

Accepting a `Func<T>`:

```csharp
public static Result<T> Read<T>(Func<T> func)
{
    try
    {
        var value = func.Invoke();
        return value is null ? new SuccessResult<T>(value!) : new FailureResult<T>(ResultException.Null);
    }
    catch (Exception e)
    {
        return new FailureResult<T>(e);
    }
}
```

Failure with a custom message:

```csharp
public static Result<T> Fail<T>(string message)
        => new FailureResult<T>(ResultException.Create(message));
```

*ResultT.Static.cs* contains the full set.

#### Write

Writes, outputting a result, are defined in the `ResultFunctionalExtensions` static class.

There are three:
1. Outputs a `T` or a provided  `T failureValue` depending on state.
2. Calls provided *Action*s depending on state.
3. Outputs a `TOut` by calling the provided *Func*s depending on state.

```csharp
public static class ResultFunctionalExtensions
{
    extension<T>(Result<T> @this)
    {
        public T Write(T failureValue)
            => @this switch
            {
                SuccessResult<T> s => s.Value,
                FailureResult<T> f => failureValue,
                _ => throw new NotImplementedException("Result Object type is undefined.")
            };

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

        public TOut Write<TOut>(Func<T, TOut> success, Func<Exception, TOut> failure)
            => @this switch
            {
                SuccessResult<T> s => success.Invoke(s.Value),
                FailureResult<T> f => failure.Invoke(f.Exception),
                _ => throw new NotImplementedException("Result Object type is undefined.")
            };
    }
```

### Map/Bind

These are defined in `ResultTMonadExtensions`.  They use type *pattern matching*.  `Map` captures any exceptions. 

```csharp
public static class ResultTMonadExtensions
{
    extension<T>(Result<T> @this)
    {
        public Result<TResult> Map<TResult>(Func<T, TResult> func)
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

        public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> func)
            => @this switch
            {
                SuccessResult<T> s => func.Invoke(s.Value),
                FailureResult<T> f => new FailureResult<TResult>(f.Exception),
                _ => new FailureResult<TResult>(ResultException.Null)
            };
    }
}
```

### Match

Finally a new pattern you may not have yet met - `Match` - similar to `Write` but passes through the `Result<T>` object. 

```csharp
public static class ResultTMonadExtensions
{
    extension<T>(Result<T> @this)
    {
        public Result<T> Match(Action<T>? success = null, Action<Exception>? failure = null)
        {
            switch (@this)
            {
                case SuccessResult<T> s:
                    success?.Invoke(s.Value);
                    break;
                case FailureResult<T> f:
                    failure?.Invoke(f.Exception);
                    break;
            }

            return @this;
        }
    }
}
```

## App

Firstly, we need a *Monadic Function* to do the parsing using `TryParse`.  This is more efficient that using `Double.Parse` and capturing the exception. 

```csharp
public static Result<double> ParseInputToDouble(string? Value)
    => double.TryParse(Value, out double value)
        ? value.ToResultT
        : ResultT.Fail<double>("The value entered was not a number.");
```

And we can refactor the app:

```csharp
Console.WriteLine(
    Console.ReadLine()
    .ToResultT
    .Bind(ConsoleApp.ParseInputToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(
        success: value => $"The result is: {value}",
        failure: exception => $"An error occured: {exception.Message}")
    );
```

## Next

So far everything is synchonous.  However, modern I/O operations are asynchronous and revolve around `Task<T>`.  The next article shows how to add the necessary extension on to `ResultT` and `Task<T>`.

[Part 7 - Async ResultT](Part7-ResultTAsync.md)
[Part 5 - Monadic Functions](Part5-Monadic-Functions.md)



