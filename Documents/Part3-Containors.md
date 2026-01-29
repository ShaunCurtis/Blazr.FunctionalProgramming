# C# Containors

Containers are a coding pattern: a fundimental building block in the C# *Functional Programming* toolkit.

They get given all sorts of names: *Wrapper*, *Kind*, *Higher Order Kind*, *Higher Order Objects*, *Lifted Objects*, *Elevated Objects*,.... to quote but a few.  Most authors assume you already know all about them, and therefore devote too little explaining what they are.

This article does't make that mistake.  A new name you can remember:  **The Containor Pattern**, and an explanation of what it does.

> A **Containor** abstracts a value into a generic object. 

It can be expressed like this:

```csharp
Containor<T>
```

Recognise this pattern?  `IEnumerable<T>` and `Task<T>` are *Containors*.

`IEnumerable<T>` provides LINQ functionality regardless of `T`'s type, `Task<T>` Provides asynchronous functionality.

*FP* objects are immutable, so a *Containor* is restricted to:

```csharp
public readonly record struct Containor<T> {...}
```
or

```csharp
public record Containor<T> {...}
```

The choice depends on the functionality required.

We can define a skeleton `Containor` object:

```csharp
public readonly record struct Containor<T>
{
    private readonly T Value;

    public Containor(T value)
        => this.Value = value;
}
```

Hide or expose `Value`?  It depends on the functionality required..

Now we need some I/O functionality.  I like the *Read/Write* paradigm, so I add basic I/O like this:

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

These two methods add a new dimension to the I/O. They accept functions in the form of *delegates* as arguments. `Func` and `Action` are two generic *delegates* built into C#: examples of the *FP* paradigm creeping into the language.

`Read` can receive any method matching the `Func<T>` pattern.

We can write:

```csharp
Containor<string?>.Read(Console.ReadLine)
```
Which reads a line from the console and wraps it in a `Containor<string?>`.  We pass the `Console.ReadLine` method as a delegate to the `Read` method. Functions become first class citizens.  They are values: you can pass them around and store them in variables.  In *FP* functions and values are interchangeable.

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

`Containor.Read(Console.ReadLine)` is a little clumsy, so we can write an extension method.  However, in this case we can't: we're dealing with a static class `Console`.  Instead, we can write a static method in a new static class:

```csharp
public static class ConsoleReader
{
       public static Containor<string?> ReadLine()
        => Containor.Read(Console.ReadLine());
}
```

And our app becomes succinct but very clear about what it's doing:

```csharp
ConsoleReader.ReadLine
    .Write<string?>(Console.WriteLine);
```

That's it for this article.

Next, I'll show how to add *FP* functionality to the `Containor`.

