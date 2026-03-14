#  Monads in C#

Programing is becoming more *Functional*.  Mainstream languages are aging.  The functional paradigm is a better fit in many of today's distributed systems architecture.

*Functional* dictates a good understanding of the *Monad Pattern*.  You don't need to understand the maths, just the principles.

While you will almost certainly use a functional progamming library, rather than building your own, you will need to understand the *Monad Pattern* to use it effectively.

Before we talk monads, let's step back and consider this simple console app written in the classic imperative style:

```csharp
var input = Console.ReadLine();

if (double.TryParse(input, out double result))
{
    double sqrt = Math.Sqrt(result);
    double rounded = Math.Round(sqrt, 2);
    Console.WriteLine($"Success: The transformed value is: {rounded}");
}
else
    Console.WriteLine("The input value could not be parsed.");
```

My issues with this code are:

1. The main logic of transforming the input value is buried inside lots of implementation detail.  
1. There are two pathways depending on whether the input is valid or not.  This leads to nested code with lots of defensive programming.

Consider this more explicit implementation:

```csharp
Console.ReadLine()
    .Then(ConvertToDouble)
    .Then(Square)
    .Then(RoundToTwoDecimalPlaces)
    .Then(WriteToConsole);
```

We can see exactly what's going on.  `Then` provides a way to chain operations together.  Each step is testable.

We can code this:

```csharp
public static class ThenExtensions
{
    extension(string? @this)
    {
        public double Then(Func<string?, double> then) => then.Invoke(@this);
    }

    extension(double @this)
    {
        public double Then(Func<double, double> then) => then.Invoke(@this);

        public void Then(Action<double> then) => then.Invoke(@this);
    }
}
```

The individual operations implemented like this:

```csharp
double ConvertToDouble(string? input) => double.TryParse(input, out double value) ? value : throw new ArgumentNullException();

double Square(double value) => Math.Sqrt(value);

double RoundToTwoDecimalPlaces(double value) => Math.Round(value, 2);

void WriteToConsole(double value) => Console.WriteLine($"Success: The transformed value is: {value}");
```

This works, but has issues: 

1. The need for `Then` implementations for every object you want to process.
1. It doesn't handle failure - if `ConvertToDouble` fails, it throws an exception which will crash the app.

## The Container Pattern

Forget about failure handling initially, and just focus on chaining.

The **Container Pattern**: wraps a value inside a generic object.  The basic pattern:

```csharp
public record Container<T>(T Value)
```

You should recognize this:  `IEnumerable<T>`, `Task<T>`,....

`Then` is implementated on the container.  Note the second `Then` has become `Write` - it's outputting data, not applying a `Tin -> TOut` function:

```csharp
public Container<TResult> Then<TResult>(Func<T, TResult> func)
    => new Container<TResult>(func.Invoke(Value));

public void Write(Action<T> action)
    => action.Invoke(Value);
```

And a static helper class to create containers:

```csharp
public static class Container
{
    extension<T>(T @this)
    {
        public Container<T> Containerize
        => new Container<T>(func.Invoke());
    }
}
```

We can now rewrite our top level code as:

```csharp
Console.ReadLine
    .Containerize
    .Then(ConvertToDouble)
    .Then(Square)
    .Then(RoundToTwoDecimalPlaces)
    .Write(WriteToConsole);
```

## Handling Failure

In the above example, if `ConvertToDouble` fails, it throws an exception which will crash the app.  `Container` doesn't handle this, so we need a new type of container that does.

First a base abstract object:

```csharp
public abstract record Result<T> { }
```

And then two derived records representing the two states:

```csharp
public record SuccessResult<T>(T Value) : Result<T>;
public record FailureResult<T>(Exception Exception) : Result<T>;
```

Next a static helper class to create an object from a value:

```csharp
public static class Result
{
    public static Result<T> Read<T>(T? value)
         => value is null ? new SuccessResult<T>(value!) : new FailureResult<T>(ResultException.Null);
```

And an overload that takes a function:

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
}
```

And finally `Then`:

```csharp
public static class ResultMonadExtensions
{
    extension<T>(Result<T> @this)
    {
        public Result<TResult> Then<TResult>(Func<T, TResult> func)
        {
            try
            {
                return @this switch
                {
                    SuccessResult<T> hv => Result.Read(func.Invoke(hv.Value)),
                    _ => new FailureResult<TResult>(ResultException.Null)
                };
            }
            catch (Exception e)
            {
                return new FailureResult<TResult>(e);
            }
        }
```

 and `Write` as extension methods:

 ```csharp
        public void Write(Action<T> success, Action failure)
        {
            switch (@this)
            {
                case SuccessResult<T> hv:
                    success.Invoke(hv.Value);
                    break;
                case FailureResult<T>:
                    failure.Invoke();
                    break;
            }
        }
```

`Then` implements a try to catch exceptions, and returns a `FailureResult` object when they occur.

`Write` uses pattern matching to determine which action to invoke.


We can now re-write our app like this:

```csharp
NullT.Read(Console.ReadLine)
    .Then(ConvertToDouble)
    .Then(Square)
    .Then(RoundToTwoDecimalPlaces)
    .Write(
        success: value => Console.WriteLine($"Success: The transformed value is: {value}"),
        failure: () => Console.WriteLine($"Failure: The value provided was not a number."));
```

Much cleaner.  The main logic is now clear.

Failure handling is abstracted away into the `Result<T>` type.  No more defensive code in sight.  The solution implements the *Railway Orientated Programming* paradigm to handle failure.  Any failure in the chain causes all subsequent operations to be skipped.

In classic functional programming terminology, this is a basic **Maybe Monad**.  `Then` is `Map`, and `Write` is `Match`.

> The really important thing to realise is that `Result<T>` abstracts away whether a value is present or not.  You no longer need to care, no more defensive code, `Result<T>.Map` handles it for you.

## Monadic Functions

Our current implementation of `Result<T>` isn't strictly a *Manad*.  *Then/Map*:

`TIn -> Functor Function -> TOut` 

is known as a *Functor*.

*Monads* implement *Monadic Functions* that themselves return a `Monad<TOut>` object: 

`TIn -> Monadic Function -> Monad<TOut>`

 and is normally called `Bind`.

Here's `Bind` for `Result<T>` object.

```csharp
public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> func)
    => @this switch
    {
        SuccessResult<T> s => func.Invoke(s.Value),
        FailureResult<T> f => new FailureResult<TResult>(f.Exception),
        _ => new FailureResult<TResult>(ResultException.Null)
    };
```

With this addition, we can now chain operations that themselves return `Result<T>` objects.

For example, we could modify `ConvertToDouble` to return a `Result<double>`:

```csharp
Result<double> StringToDouble(string? input)
{
    try
    {
        if (double.TryParse(input, out double value))
            return new SuccessResult<double>(value);
        else
            return new FailureResult<double>(ResultException.Null);
    }
    catch (Exception e)
    {
        return new FailureResult<double>(ResultException.Null);
    }
}
```

And our final app code would look like this:

```csharp
Result.Read(Console.ReadLine)
    .Bind(StringToDouble)
    .Map(Square)
    .Map(RoundToTwoDecimalPlaces)
    .Match(
        hasValue: value => Console.WriteLine($"Success: The transformed value is: {value}"),
        hasNoValue: () => Console.WriteLine($"Failure: The value provided was not a number."));
```

### ToResultT

We can add a global property to all objects like this:

```csharp
public static class ResultFunctionalExtensions
{
    extension<T>(T @this)
    {
        public Result<T> ToResultT =>
            @this is not null ? new SuccessResult<T>(@this) : new FailureResult<T>(ResultException.Null);
    }
}
```

And our app looks like this:

```csharp
Console.ReadLine()
    .ToResultT
    .Bind(StringToDouble)
    .Map(Square)
    .Map(RoundToTwoDecimalPlaces)
    .Match(
        success: value => Console.WriteLine($"Success: The transformed value is: {value}"),
        failure: () => Console.WriteLine($"Failure: The value provided was not a number."));
```

## Summary

This is a quick tour through the two Monadic containers in my `Blazr.Manganese` library.  The library implementation are little different, the base objects are more screwed down, and they include *async* operations.  