# C# Functional Programming Fundimentals

To get on in *FP* you need to understand some key concepts:

1. Immutability
1. Functions
1. Containers

I'll cover the first two here and deal with the last in the next article.

## Immutablity 

[Almost] everything in *FP* is immutable.  Get used to *record* and *readonly record struct*.

```csharp
public class WeatherForecast
{
    public Guid Id { get; set; } = Guid.NewGuid();
	public DateOnly Date {get; set;}
	public decimal TemperatureC {get; set;}
    public string Summary { get; set; } = string.Empty;
}
```

becomes:

```csharp
public record WeatherForecast
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public DateOnly Date {get; init;}
	public decimal TemperatureC {get; init;}
    public string Summary { get; init; } = string.Empty;
}
```

or even:

```csharp
public record WeatherForecast (
    Guid Id,
    DateOnly Date ,
    decimal TemperatureC,
    string Summary
    );
```

You mutate in code like this:

```csharp
var newRecord = record with {Summary = "Updated"};
```

Or with an interactive UI using what I call a *Mutor*:

```csharp
public class WeatherForecastMutor
{
    public WeatherForecast BaseRecord { get; private set; }
    public DateOnly Date { get; set; }
    public decimal TemperatureC { get; set; }
    public string Summary { get; set; } = string.Empty;
    public bool IsNew { get; private init; }

    public WeatherForecastMutor(WeatherForecast record )
    {
        this.BaseRecord = record;
        this.Set();
    }
    public WeatherForecastMutor()
    {
        this.BaseRecord = new(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), 0, "[Not Set]");
        this.Set();
        this.IsNew = true;
    }

    private void Set()
    {
        this.Date = this.BaseRecord.Date;
        this.Summary = this.BaseRecord.Summary;
        this.TemperatureC = this.BaseRecord.TemperatureC;    
    }

    public WeatherForecast Record
        => new WeatherForecast(
            Id: this.BaseRecord.Id,
            Date: this.Date,
            TemperatureC: this.TemperatureC,
            Summary: this.Summary
            );

    public bool IsDirty
        => this.Record != this.BaseRecord;
}
```

This also tracks state.

## Functions

Functions in *FP* are *values*.  You can assign them to variables, arguments, and pass them around just like normal objects.

C# handles this concept with *Delegates*, and provides `Func` and `Action` as ready made ones in the language.

Let's look at some code to see this in action.

Here we define a simple method as a Llambda expression:

```csharp
Func<double, double> doubler = value => value + value;
```

and then use it like this:

```csharp
var x = doubler(2);
```

We can also assign existing methods:

```csharp
Func<string, double> parser = Double.Parse;
Func<double, double> squarer = Math.Sqrt;
```

### Function Composition

We can take the code above and compose it into a single function:

```csharp
Func<string, double> doitall = x => squarer(doubler(parser(x)));
```
And then use it like this:

```csharp
Console.WriteLine(doitall(Console.ReadLine()!));
```

or even:

```csharp
Action readAndWrite = () => Console.WriteLine(doitall(Console.ReadLine()!));

readAndWrite();
```

Next, *Containers*.

