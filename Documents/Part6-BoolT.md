# NullT - A fully Functional Containor

In Parts 1 through to 5 you learnt about *Containors*, *Monads* and *Functors* in programming terms:  They are just coding patterns.  You also learnt how to build a simple *Containor* and two state *NullableContainor* that implemented the *Monad* and *Functor* patterns.

In this article I'll demostrate how to build a more complex three state *containor* that I use in my projects.

It's called `Null<T>` and handles both:

1. Nullable returns : as implemented ib `Null<T>` 
1. Input Errors/Exceptions.

It provides a path to flow errors/exceptions from input to output: in my applications, from the CQS data pipeline to the UI or API interface.

## Our Demo Project

This is basically the same as the other articles.

THe sarting point is the one from the end of part 5.

```csharp
  ConsoleReader.ReadLine()
    .Bind(TryParseToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.")
    .Write(Console.WriteLine);
```

THe three states are:

1. True/Success
2. False/Failure
3. False/Failure with Exception

## `Bool<T>`

The basic *Containor* definition

```csharp
public sealed record Bool<T>
{
    public Exception? Exception { get; private init; }
    public T? Value { get; private init; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool HasValue { get; private init; }

    public bool HasException => Exception is not null;
}
```

Everything is public so I can write extension methods where I need them.

Notes:
1. Sealed - there's no valid reason to inherit.
2. It's a `record` rather than a `readonly record struct` because we need constructor control.
3. Compiler help attributes to qualify nullable state.

### Custom Exception

 There's a custom exception to pass "messages".  End points can differential between a real exception and a message.

```csharp
public class BoolException : Exception
{
    public BoolException() : base("The Bool operation failed.") { }
    public BoolException(string message) : base(message) { }

    public static BoolException Create(string message)
        => new (message);
}
```

### Constructors

The `new` constructors are private.

```csharp
    private Bool(T value)
    {
        Value = value;
        HasValue = true;
    }
    
    private Bool(Exception? exception) => Exception = exception;
    private Bool() => this.HasValue = false;
```

There are four basic static constructors:

```csharp
    public static Bool<T> Success(T value) => new(value);
    public static Bool<T> Failure() => new();
    public static Bool<T> Failure(Exception exception) => new(exception);
    public static Bool<T> Failure(string message) => new(new BoolException(message));
```

And four `Read` constructors.

```csharp
    public static Bool<T> Read(T? value)
        => value is null
        ? new(new BoolException("T was null."))
        : new(value);

    public static Bool<T> Read(T? value, string errorMessage) =>
        value is null
            ? new(new BoolException(errorMessage))
            : new(value);

    public static Bool<T> Read(object? value = null  ) => new();
    public static Bool<T> Read(Func<T?> input) => Read(input.Invoke());
```

And four statics on `BoolT`:

```csharp
public static class BoolT
{
    public static Bool<T> Success<T>(T value) => Bool<T>.Success(value);
    public static Bool<T> Read<T>(T? value) => Bool<T>.Read(value);
    public static Bool<T> Read<T>(T? value, string errorMessage) => Bool<T>.Read(value);
    public static Bool<T> Read<T>(Func<T?> input) => Read(input.Invoke());
}
```

### Map

The standard `Map` *Functor* function and a `TryMap` that encapsulates a `try` to capture any exceptions in the execution of `function`.   

```csharp
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
```

### Bind

The *Monadic function* implementation as an extension:

```csharp
public Bool<TOut> Bind<TOut>(Func<T, Bool<TOut>> function)
    => @this.HasValue
        ? function(@this.Value)
        : Bool<TOut>.Failure(@this.Exception);
```

### Output

Finally a set of three `Write` methods. 

```csharp
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
```

 ## FP Demo App

A new version of the double parser.

 ```csharp
static Bool<double> TryParseAsDouble(string? value)
{
    if (double.TryParse(value, out double result))
        return BoolT.Read(result);

    return Bool<double>.Failure();
}
```

And `ConsoleReader`

```csharp
public static class ConsoleReader
{
    public static NullableContainor<string?> ReadLine()
        => NullableContainor.Read(Console.ReadLine());
    
    public static Bool<string?> ReadInput()
       => Bool<string?>.Read(Console.ReadLine());
}
```
And then the refactored app:  

```csharp
ConsoleReader.ReadInput()
  .Bind(TryParseAsDouble)
  .Map(Math.Sqrt)
  .Map(value => Math.Round(value, 2))
  .Write(
      hasValue: value => $"Success: The transformed value is: {value}",
      hasNoValue: () => "The input value could not be parsed.",
      hasException: ex => $"An error occurred: {ex.Message}")
  .Write(Console.WriteLine);
```

