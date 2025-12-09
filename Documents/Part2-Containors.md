# C# Containors

Containers are a pattern in the C# *Functional Programming* toolkit.

They get given all sorts of names: *Wrapper*, *Kind*, *Higher Order Kind*, *Higher Order Objects*, *Lifted Objects*, *Elevated Objects*,.... to quote but a few.  Most authors assume you already know about it, devote too little explaining what they are.

This article does't make that mistake.  I give it a name you can remember:  **The Containor Pattern**, and the attention it deserves.

A *containor* can be expressed like this:

```csharp
Containor<T>
```

It's a generic super-object that wraps around another object.
 
You use them already:  `IEnumerable<T>` and `Task<T>` are *Containors*.

The purpose of the *containor* is to provide some generic functionality regardless of the type of `T`.  `IEnumerable<T>` provides LINQ functionality regardless of `T`'s type.

*FP* objects are immutable, so a *Containor* can be defined like this:

```csharp
public readonly record struct Containor<T> {...}
//or
public record Containor<T> {...}
```

The choice depends on the functionality required.  My rule is `struct` by default.

The skeleton `Containor` object:

```csharp
public readonly record struct Containor<T>
{
    private readonly T Value;

    public Containor(T value)
        => this.Value = value;
}
```

Hide or expose `Value` depending on the functionality required..

Next we need to provide some I/O functionality.  I like the *Read/Write* paradigm, so I add basic I/O like this:

```csharp
public static Containor<T> Read(T value)
    => new Containor<T>(value);

public T Write()
    => this.Value;
```

So far, just normal *OOP* code.  

Now some *FP* style I/O:


```csharp
public static Containor<T> Read(Func<T> func)
    => new Containor<T>(func.Invoke());

public void Write<TOut>(Action<T> action)
    => action.Invoke(this.Value);
```

These two methods add a new dimension to the I/O. They accept functions as arguments.  `Read` can receive any method matching the `Func<T>` pattern.

We can write:

```csharp
Containor<string?>.Read(Console.ReadLine)
```
Which reads a line from the console and wraps it in a `Containor<string?>`.  We pass the `Console.ReadLine` method as a delegate to the `Read` method. Functions become as first class citizens.

With this new I/O, we can write this simple console app:

```csharp
Containor<string?>
    .Read(Console.ReadLine)
    .Write<string?>(Console.WriteLine);
```

`Console.WriteLine(string)` matches the `Write` pattern.  `T` is qualified in `Write` because the compiler doesn't know which `Console.WriteLine` overload to use.

A `Containor<T>` factory class simplifies the constructors:

```csharp
public static class Containor
{
    public static Containor<T> Read<T>(T value)
        => Containor<T>.Read(value);

    public static Containor<T> Read<T>(Func<T> func)
        => Containor<T>.Read(func);
}
```

And the console app becomes:

```csharp
Containor
    .Read(Console.ReadLine)
    .Write<string?>(Console.WriteLine);
```


That's it for this article.

Next, I'll show how to add some real *FP* functionality to the `Containor`.

