# C# The Monad Pattern

We are now ready to address the *Monad Enigma*.

In coding a *Monadic Function* is a function with the following pattern:

```csharp
Containor<TOut> Bind(Func<T, Containor<TOut>> func)
```
In the single state `Containor` object context it can be coded like this:

```csharp
public Containor<TOut> Bind<TOut>(Func<T, Containor<TOut>> func)
    => func.Invoke(this.Value);
```
In the two state `NullableContainor` object context it can be coded like this:

```csharp
public NullableContainor<TOut> Bind<TOut>(Func<T, NullableContainor<TOut>> func)
    => HasValue 
        ? func.Invoke(Value) 
        : new NullableContainor<TOut> { HasValue = false };
```

In the previous article I addressed the `double.Parse` issue with `TryMap.`  This works, but it generates costly exceptions.  There's `double.TryParse` that's more efficient, but a horrible bit of code, spouting results at both ends.  One solution is to wrap it in a *Manadic Function* like this:

```csharp
static NullableContainor<double> TryParseToDouble(string? value)
{
    if (double.TryParse(value, out double result))
        return NullableContainor.Read(result);

    return NullableContainor.NoValue<double>();
}
```

This now handles the nullable input string and wraps the `TryParse` in a more elegant solution.

The console app:

```csharp
NullableContainor.Read(Console.ReadLine)
    .Bind(TryParseToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(Console.WriteLine);
```
