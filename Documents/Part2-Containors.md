# C# Containors

In at the shallow end.

Containers are our main *Functional Programming* building block.

They don't have a universal name.  You will see them called *Wrapper*, *Kind*, *Higher Order Kind*, *Higher Order Objects*, *Lifted Objects*, *Elevated Objects*,... to name but a few.  Authors switch between them, eve within articles,  and cause much confusion.

A container is a wrapper you place around an object.  It's generic.  I've invented a term for them - *Containors*.  I'll use it from now on.
 
You are almost certainly using containers already.  `IEnumerable<T>` and `Task<T>` are good examples.

The purpose of a *containor* is to provide generic functionality regardless of the type.  Like `IEnumerable<T>` provides LINQ functionality to any object collection.

It can be expressed like this:

```csharp
Containor<T>
```

*FP* objects are immutable, so:

```csharp
public readonly record struct Containor<T> {...}
//or
public record Containor<T> {...}
```

The choice depends on the functionality required.  In general use a `struct` unless...

The basic `Containor` object:

```csharp
public readonly record struct Containor<T>
{
    private readonly T Value;

    public Containor(T value)
        => this.Value = value;
}
```

Which does absolutely nothing and hides it's internal value.

I'm a fan of the *Read/Write* paradigm, so I add the basic I/O to *Containor* like this:

```csharp
public static Containor<T> Read(T value)
    => new Containor<T>(value);

public T Write()
    => this.Value;
```

And then this

```csharp
public static Containor<T> Read(Func<T> func)
    => new Containor<T>(func.Invoke());

public void Write<TOut>(Action<T> action)
    => action.Invoke(this.Value);
```

This is real *FP* paradigm code: treating functions as *First Class Citizens*. Both methods are higher-order functions: they accept functions as arguments.

`Read` can receive any method matching the `Func<T>` pattern.  `Console.ReadLine()` for example.

This new I/O lets me write this simple console app:

```csharp
Containor<string?>
    .Read(Console.ReadLine)
    .Write<string?>(Console.WriteLine);
```

`Console.WriteLine(string)` matches the `Write` pattern.  `T` must be qualified for `Write` because the compiler doesn't know which `Console.WriteLine` overload to use.

A simple factory for `Containor<T>` simplifies the constructors:

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

In the next article, I'll show how to add some functionality to the `Containor`.