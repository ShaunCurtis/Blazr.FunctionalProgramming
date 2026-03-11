# C# The Monad Pattern

We are now ready to address the *Monad Enigma*.

In coding a *Monadic Function* is a function with the following pattern:

```csharp
Monad<TOut> Bind(Func<T, Monad<TOut>> func)
```

Note the standard practicein calling the method `Bind`.

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

We can write a new parsing method:

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

That's it.  `Container<T>` is a *Monad*: it implements the *Monadic Function* pattern.  The pattern unwraps a lot of powerful coding options, that you will learn later.

However, `Container` is a single state container and has no mechanism for handling failure.  To do that we need to mov to the `Result` Monad.  Read on. 