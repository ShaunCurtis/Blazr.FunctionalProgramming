# C# The Functor Pattern

> A *Functor* is a higher order method that takes a `T => TOut` function as it's input.  

In the *Container* context, the *Functor* applies a function with the pattern `Func<T, TOut>` to it's internal `T` value and returns a new *Container* instance of `TOut`.  

This *Functor* is normally called `Map`.

```csharp
public Container<TOut> Map(Func<T, TOut> func);
```

Many of the functions we use or write fit this pattern. Examples are `Double.Parse` and `Math.Sqrt(double)`.  Note that `T` and `TOut` may be the same type.  

The `Map` implementation in the `Container` context is implemented as an extension method in the *ContainerFunctionalExtensions* static class :

```csharp
public static class ContainerFunctionalExtensions
{
    extension<T>(Container<T> @this)
    {
        public Container<TResult> Map<TResult>(Func<T, TResult> func)
            => Container<TResult>.Read(func.Invoke(@this.Value));

        //.. other extensions
    }
}
```

The console app using double.Parse now looks like this:

```csharp
Console.ReadLine()
    .Containerize
    .Map(value => double.Parse(value ?? string.Empty))
    .Write(Console.Write);
```

Note the lambda expression to handle nullables.

We can also now *chain* computations together like this:

```csharp
Console.ReadLine()
    .Containerize
    .Map(value => double.Parse(value ?? string.Empty))
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(Console.Write);
```

Next, *Monads*!.

[Part 5 - Monadic Functions](Part5-Monadic-Functions.md)
[Part 3 - Containers](Part3-Containers.md)
