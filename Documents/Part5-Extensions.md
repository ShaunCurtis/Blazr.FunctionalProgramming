# C# Extension Methods

The console app currently looks like this:

```csharp
NullableContainor.Read(Console.ReadLine)
    .Bind(TryParseToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(Console.WriteLine);
```

It would be nice to tidy up the output using `Write`.

```csharp
NullableContainor.Read(Console.ReadLine)
    .Bind(TryParseToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.")
    .Write(Console.WriteLine);
```

The problem is that last `Write` doesn't work.  The first `Write` outputs a string.  And string doesn't support the *FP* paradigm.

True, but there's nothing stopping this:

```csharp
public static class Extensions
{
    extension(string value)
    {
        public void Write(Action<string> writer)
            => writer.Invoke(value);
    }
}
```

And everything now works.

## Static Helper Classes

```csharp
NullableContainor.Read(Console.ReadLine)
```

is a bit cumbersome.  It can be replaced with a simple static helper.

```csharp
public static class ConsoleReader
{
       public static NullableContainor<string?> ReadLine()
        => NullableContainor.Read(Console.ReadLine());
}
```

and the console app:

```csharp
  ConsoleReader.ReadLine()
    .Bind(TryParseToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.")
    .Write(Console.WriteLine);
```

The most elegate solution would be to add a static extension method to `Console`, but that functionality was pulled from the latest C# release.


