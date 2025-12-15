# C# The Functor Pattern

In coding terms, a *Functor* is a method that takes a `T => TOut` function as it's input.  The *containor* applies the function to it's internal value and returns a new *containor* instance of `TOut`.  The *Functor* method is normally called `Map`.

In the `Containor` context it can be defined like this:

```csharp
public Containor<TOut> Map(Func<T, TOut> func);
```

It maps a standard function, such as `Math.Sqrt(double)`.  `T` and `TOut` may be the same type.  Many of the functions we use or write fit this pattern.

The `Map` implementation in the `Containor` context:

```csharp
public Containor<TOut> Map<TOut>(Func<T, TOut> func)
    => new Containor<TOut>(func.Invoke(this.Value));
```

And the console app using double.Parse:

```csharp
Functor.Read(Console.ReadLine)
    .Map(value => double.Parse(value ?? string.Empty))
    .Write<double>(Console.WriteLine);
```

Note the llambda expression to handle nullables.

Ok so far, but code contains a flaw: `double.Parse` will raise an exception if it can't parse the input.  Using a `try` will work, but what does `Map` return if it catches an exception.  We need a new, more powerful, containor to handle this scenario.

## `Null<T>`

`NullT` has two states:

1. HasValue - True
2. HasNoValue - False

First build a `Null<T>` based on `Containor`.

The core object and new constructor:

```csharp
public readonly record struct Null<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    private bool HasValue { get; init; } = false;
    private T? Value { get; init; } = default!;

    private Null(T? value)
    {
        if (value is not null)
        {
            Value = value;
            HasValue |= true;
        }
    }
}
```

Three static constructors:

```csharp

    public static Null<T> Read(T Value)
        => new Null<T>(Value);

    public static Null<T> Read(Func<T> input)
        => new Null<T>(input.Invoke());

    public static Null<T> NoValue()
        => new Null<T>();
```

The `Write` methods now need to handle the two states.

```csharp
// Requires a default value of T to return if the state is false
    public T Write(T defaultValue)
        => HasValue ? Value : defaultValue;

// Applies one of two functions based on state to produce TOutstates.
    public TOut Write<TOut>(Func<T, TOut> hasValue, Func<TOut> hasNoValue)
        => this.HasValue
            ? hasValue.Invoke(Value)
            : hasNoValue.Invoke();

// Executes one of two actions based on state.  hasNoValue is optional  
    public void Write(Action<T> hasValue, Action? hasNoValue = null)
    {
        if (HasValue)
        {
            hasValue.Invoke(Value);
            return;
        }

        hasNoValue?.Invoke();
    }
```

The factory class:

```csharp
public static class NullT
{
    public static Null<T> Read<T>(Func<T> input)
        => Null<T>.Read(input.Invoke());

    public static Null<T> Read<T>(T value)
        => Null<T>.Read(value);

    public static Null<T> NoValue<T>()
        => new Null<T>();
}
```

And `Map`.

```csharp
    public Null<TOut> Map<TOut>(Func<T, TOut> func)
        => HasValue ? Null<TOut>.Read(func.Invoke(Value)) : new Null<TOut>();
}
```

Now to the try problem.

```csharp
    public Null<TOut> TryMap<TOut>(Func<T, TOut> func)
    {
        try
        {
            return this.HasValue
                ? Null<TOut>.Read(func.Invoke(Value))
                : new Null<TOut>();
        }
        catch
        {
            return new Null<TOut>();
        }
    }
```

We can now refactor the console app:

```csharp
NullT.Read(Console.ReadLine)
    .TryMap(double.Parse!)
    .Write(Console.WriteLine);
```

## Chaining Functors

Adding more steps to the process is simple and demonstrates the power of `Map`:

```csharp
NullT.Read(Console.ReadLine)
    .TryMap(double.Parse!)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(Console.WriteLine);
```

Next, *Monads*!.