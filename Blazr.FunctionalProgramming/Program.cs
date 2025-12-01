using Blazr.Containors;
using Blazr.FunctionalProgramming;
using Blazr.Manganese;

//var inputContainer = Containor<string?>.Read(Console.ReadLine);

//Containor<string?>.Read(Console.ReadLine)
//    .Write<string?>(Console.WriteLine);

//NullableContainor.Read(Console.ReadLine)
//ConsoleReader.ReadLine()
//  .Bind(TryParseToDouble)
//  .Map(Math.Sqrt)
//  .Map(value => Math.Round(value, 2))
//  .Write(
//      hasValue: value => $"Success: The transformed value is: {value}",
//      hasNoValue: () => "The input value could not be parsed.")
//  .Write(Console.WriteLine);

//ConsoleReader.ReadInput()
//  .Bind(TryParseAsDouble)
//  .Map(Math.Sqrt)
//  .Map(value => Math.Round(value, 2))
//  .Write(
//      hasValue: value => $"Success: The transformed value is: {value}",
//      hasNoValue: () => "The input value could not be parsed.",
//      hasException: ex => $"An error occurred: {ex.Message}")
//  .Write(Console.WriteLine);


await DataProvider.GetDataAsync()
  .MapAsync(Math.Sqrt)
  .MapAsync(value => Math.Round(value, 2))
  .WriteAsync(
      hasValue: value => $"Success: The transformed value is: {value}",
      hasNoValue: () => "The input value could not be parsed.",
      hasException: ex => $"An error occurred: {ex.Message}")
  .WriteAsync(Console.WriteLine);


(await DataProvider.GetDataAsync())
  .Map(Math.Sqrt)
  .Map(value => Math.Round(value, 2))
  .Write(
      hasValue: value => $"Success: The transformed value is: {value}",
      hasNoValue: () => "The input value could not be parsed.",
      hasException: ex => $"An error occurred: {ex.Message}")
  .Write(Console.WriteLine);

//NullableContainer.Read(Console.ReadLine)
//    .TryMap(value => double.Parse(value ?? string.Empty))
//    .Write(Console.WriteLine);

static Null<double> TryParseToDouble(string? value)
{
    if (double.TryParse(value, out double result))
        return NullT.Read(result);

    return NullT.NoValue<double>();
}

static Return<double> TryParseAsDouble(string? value)
{
    if (double.TryParse(value, out double result))
        return ReturnT.Read(result);

    return Return<double>.Failure();
}

static async Task<Return<double>> TryParseAsDoubleAsync(string? value)
{
    await Task.Delay(3000);
    if (double.TryParse(value, out double result))
        return ReturnT.Read(result);

    return Return<double>.Failure();
}

public static class ConsoleReader
{
    public static Null<string?> ReadLine()
        => NullT.Read(Console.ReadLine());

    public static Return<string?> ReadInput()
       => Return<string?>.Read(Console.ReadLine());
}

public static class Extensions
{
    extension(string @this)
    {
        public void Write(Action<string> writer)
            => writer.Invoke(@this);
    }

    extension(Task<string> @this)
    {
        public async Task WriteAsync(Action<string> writer)
            => writer.Invoke(await @this);
    }
}
