# C# Containors

Containers are a coding pattern, a powerful *Functional Programming* building block.

They deserve a category to themselves, so let me introduce the **Containor Pattern**.

A containor can be expressed like this:

```csharp
Containor<T>
```

Without a name, they're not univerally regarded as a *coding pattern*.  The are called: *Wrapper*, *Kind*, *Higher Order Kind*, *Higher Order Objects*, *Lifted Objects*, *Elevated Objects*,.... to quote just a few.  Authors fail to devote enough time to them, assume you already know about them, and often switch naming conventions mid-article.

They are generic super-objects that wrap other objects.  **Containor** is a term I inverted to describe them.  I'll use it from now on.
 
You use containers already:  `IEnumerable<T>` and `Task<T>` are *Containors*.

The purpose of a *containor* is to provide some generic functionality regardless of the type.  `IEnumerable<T>` provides LINQ functionality regardless of `T`'s type.

In *FP* objects are immutable, so we can define a *Containor* like this:

```csharp
public readonly record struct Containor<T> {...}
//or
public record Containor<T> {...}
```

The choice depends on the functionality required.  My rule is: `struct` by default.

The skeleton `Containor` object:

```csharp
public readonly record struct Containor<T>
{
    private readonly T Value;

    public Containor(T value)
        => this.Value = value;
}
```

Whether you hide or expose `Value` depends on the functionality you want to provide and the context in which you're writing the code.

Next we need to provide some I/O functionality.

I'm a fan of the *Read/Write* paradigm, so I add basic I/O like this:

```csharp
public static Containor<T> Read(T value)
    => new Containor<T>(value);

public T Write()
    => this.Value;
```

So far, normal *OOP* code.  But now some *FP* style I/O:


```csharp
public static Containor<T> Read(Func<T> func)
    => new Containor<T>(func.Invoke());

public void Write<TOut>(Action<T> action)
    => action.Invoke(this.Value);
```

These two methods add a new dimension to the I/O. They accept functions as arguments. 

`Read` can receive any method matching the `Func<T>` pattern.

We can write:

```csharp
Containor<string?>.Read(Console.ReadLine)
```
Which reads a line from the console and wraps it in a `Containor<string?>`.  We're passing the `Console.ReadLine` method as a delegate to the `Read` method. Treating functions as first class citizens.

This new I/O lets us write this simple console app:

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

In the next, I'll show how to add some real *FP* functionality to the `Containor`.

