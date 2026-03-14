# C# Containers

Containers are a fundimental building block in the C# *Functional Programming* toolkit: a coding pattern.

You will see them given all sorts of names: *Wrapper*, *Kind*, *Higher Order Kind*, *Higher Order Objects*, *Lifted Objects*, *Elevated Objects*,.... to quote but a few.  Most authors devote no little to explaining what they are: assume you already know.

So lets define a **Container**.

> A **Container** abstracts a value into a generic object. 

It can be expressed like this:

```csharp
Container<T>
```

And in C# defined like this:

```csharp
public record Container<T>(T Value)
```

Recognise this pattern?  `IEnumerable<T>` and `Task<T>` are *Containers*.  They provide specific functionality to the value they contain.

Note: *FP* objects are immutable, so a *Container* is defined as a `record`.

At this point a decision is required .  Hide or expose `Value`?  

It depends on the functionality required.  Is `Value` visible or hidden?  If it's visible, then the *Container* is just a wrapper around the value.  If it's hidden, then the *Container* is an abstraction over the value.  The *Container* can provide additional functionality to the value, but the value itself is not directly accessible.

My preference is to hide the constructor and expose `Value` as a read only property.  `Value` is accessible, but you need to use a static constructor.


```csharp
public record Container<T>
{
    public T Value { get; private init; }

    private Container(T value)
       => this.Value = value;

    public static Container<T> Read(T value)
        => new Container<T>(value);
}
```

This may seem debatable in this simple example, but in more complex *FP* objects, the constructor may be doing more than just setting a value.  It may be doing some validation or transformation on the value.  In that case, it's better to hide the constructor and expose a static method to create the object.

We can write this simple console app:

```csharp
Console.WriteLine(Console.ReadLine().Containerize.Value);
```

So far, just normal *OOP* code.

## Higher Order Functions

> Higher order functions are functions that take other functions as arguments or return functions as results.

They're a fundimental building block in the *FP* toolkit.  They allow us to abstract over behavior and create reusable code.

Adding some *FP* style I/O to `Container`:

```csharp
public static class Container
{
    public static Container<T> Read<T>(Func<T> func)
        => Container<T>.Read(func.Invoke());
//...
}
```

`Read` can receive any method matching the `Func<T>` pattern.  So we can write:

```csharp
Container.Read(Console.ReadLine);
```

And:

```csharp
public static class ContainerFunctionalExtensions
{
    extension<T>(Container<T> @this)
    {
        public void Write(Action<T> action)
            => action.Invoke(@this.Value);

        //... other methods
    }
}
```

So we can write:

```csharp
Container.Read(Console.ReadLine)
    .Write(Console.Write);
```

These methods add a new dimension to the I/O. They accept functions in the form of *delegates* as arguments. `Func` and `Action` are two generic *delegates* built into C#: examples of the *FP* paradigm creeping into the language.

`Read` can receive any method matching the `Func<T>` pattern, and `Write` any method matching the `Action<T>` pattern.

Functions are first class citizens.  They are *values* you can pass around and store in variables.  In *FP* functions and values are interchangeable.

`Container.Read(Console.ReadLine)` is a little clumsy, so we can write an extension method.  However, in this case we can't: we're dealing with a static class `Console`.  Instead, we can write a static method in a new static class:

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

For general purposes, the latest C# lets us write a generic general extension method:

```csharp
public static class Container
{
    //... other methods
    extension<T>(T @this)
    {
        public Container<T> Containerize
            => Container<T>.Read(@this);
    }
}
```

Which allows us to write:

```csharp
Console.ReadLine()
    .Containerize
    .Write(Console.Write);
```

That's it for this article.

Next, I'll show how to add more *FP* functionality to the `Containor`.

[Part 2 - FP Fundimentals](Part2-Fundimentals.md)
[Part 4 - Functors](Part4-Functors.md)


