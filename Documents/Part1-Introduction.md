# Functional Programming in C#

This series of articles is about applying *Functional Programming* principles to C#.

Let's start with a fact.

> You won't switch from an *OOP* to a *FP* programming paradigm in a day or a month.  If you continue using C# you will never switch totally.

This begs a very relevant question.  Why start?  What's the point?

I'll leave you to be the final judge once you've read and worked with thi set of articles.

What I will say is it helped me solve several fundimental code problems in a more concise and elegant way.  It made me think a lot more about *state* and immutability.  My code now is less buggy, and better tested.  And there's a lot less of it!

Before we jump into the detail, it worth mentioning the *Monad Enigma*.

To quote *Douglas Crockford*:

> The monadic curse is that once someone learns what monads are and how to use them, they lose the ability to explain them to other people.

If you're reading this, you've probably already tried some of the other articles.  This one isn't top of the search list.  Most quickly become incomprehensible.  There are a few gems of information out there, but they're not on page 1 of the search engine results, or are buried away in a mass of otherwise incomprehensible alpha numeric characters.

The problem is, at some point on page one, they throw you in a the deep end.

Here's a classic piece of information you don't need to know:

> A Monad is just a Monoid in the Category of EndofFunctors.

And there's that word - *Manad* - that they all try and explain before talking about the basics.  Like trying to explain complex language functionality without first covering the *Type* system.


So before we begin, a very quick look at some *FP* fundamental principles: 

1. Functions are pure. 

1. Functions cab be Higher-Order.

1. Variables are Immutable.

1. Iteration uses Recursion. 

1. Referential Transparency.

That's also it with the M word for a while.  

To the shallow end.

## FP support in C#

C# was conceived as an imperative language.  While some functional programming concepts have been added over time, some of the fundimentals aren't baked into the language.  There are specific implementations, such as `IEnumerable<T>`, but these are basically "hacked in".

So how do we add it?

Two ways:

1. Create generic wrappers - containers - like `IEnumerable<T>`.
2. Add `Extension` methods to existing objects.

I'll demonstrate both.

## Containers

Containers are a fundimental building block of Functional Programming.  They don't have a universal name, so you can get petty confused between articles and even within the same article as author's use their own names and switch between names. *Wrapper*, *Kind*, *Higher Order Kind*,  *Higher Order Objects*, *Lifted Objects*, *Elevated Objects*,... to name but a few.

A container is a wrapper you can place around any object.  It's generic.  I've invented a term for them - *Containors*.  I'll use it from now on.

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

And then the first real *FP* code:

```csharp
public static Containor<T> Read(Func<T> func)
    => new Containor<T>(func.Invoke());

public void Write<TOut>(Action<T> action)
    => action.Invoke(this.Value);
```

This is real *FP* paradigm code: treating functions are *First Class Citizens*. Both methods are higher-order functions: they accept functions as arguments.

`Read` can receive any method matching the `Func<T>` pattern.  `Console.ReadLine()` is a good example.

This new I/O lets me write this simple console app:

```csharp
Containor<string?>
    .Read(Console.ReadLine)
    .Write<string?>(Console.WriteLine);
```

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

`Console.WriteLine(string)` matches the `Write` pattern.  `T` must be qualified for `Write` because the compiler doesn't know which version of `Console.WriteLine` to use.
