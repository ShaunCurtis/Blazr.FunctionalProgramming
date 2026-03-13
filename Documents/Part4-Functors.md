# C# The Functor Pattern

> A *Functor* is a higher order method that takes a `T => TOut` function as it's input.  

The *container* applies the *functor* to it's internal value and returns a new *container* instance of `TOut`.  `Map`is the normal function name for a *Functor*.

In the `Container` context it can be defined like this:

```csharp
public Containor<TOut> Map(Func<T, TOut> func);
```

It maps a standard function, such as `Math.Sqrt(double)`.  `T` and `TOut` may be the same type.  Many of the functions we use or write fit this pattern.

The `Map` implementation in the `Container` context:

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

And the console app using double.Parse:

```csharp
Console.ReadLine()
    .Containerize
    .Map(value => double.Parse(value ?? string.Empty))
    .Write(Console.Write);
```

Note the lambda expression to handle nullables.

We can now *chain* computations like this:

```csharp
Console.ReadLine()
    .Containerize
    .Map(value => double.Parse(value ?? string.Empty))
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(Console.Write);
```

Next, *Monads*!.