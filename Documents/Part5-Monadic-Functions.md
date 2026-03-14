# C# The Monad Pattern

We are now ready to address the *Monad Enigma*.

In coding a *Monadic Function* is a function with the following pattern:

```csharp
Monad<TOut> Bind(Func<T, Monad<TOut>> func)
```

It's standard practice (but not compulsory) to call the method `Bind`. 

In the single state `Container` object context it can be coded like this:

```csharp
public static class ContainerFunctionalExtensions
{
    extension<T>(Container<T> @this)
    {
        public Container<TResult> Bind<TResult>(Func<T, Container<TResult>> func)
            => func.Invoke(@this.Value);

        //,, more methods
    }
}
```

We can now write a new parsing method that returns a `Container<double>`:

```csharp
Container<double> TryParseToDouble(string? value)
    => double.TryParse(value, out double result)
        ? result.Containerize
        : (0d).Containerize;
```

And refactor our app:

```csharp
Console.ReadLine()
    .Containerize
    .Bind(TryParseToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(Console.Write);
```

That's it.  `Container<T>` is now a *Monad*: it implements the *Monadic Function* pattern.  The pattern unwraps a lot of powerful coding options, that you will learn later.

There is however a major issue with the whole app.  `Container` has a single state: it has no mechanism for elegantly handling failure.  You can see workarounds in the code.

The next article introduces you to the two state `Result` Monad.

[Part 6 - ResultT](Part6-ResultT.md)
[Part 4 - Functors](Part4-Functors.md)
