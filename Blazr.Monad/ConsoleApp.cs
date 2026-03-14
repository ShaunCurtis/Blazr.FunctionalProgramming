/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Monad;

public static class ConsoleApp
{
    public static void BasicApp()
    {
        var input = Console.ReadLine();

        if (input is null)
            Console.WriteLine("You must enter a value.");

        if (double.TryParse(input, out double value))
        {
            value = Math.Sqrt(value);
            value = double.Round(value, 2);

            Console.WriteLine($"The square root of {input} is {value}.");
        }
        else
        {
            Console.WriteLine("The value entered was not a number.");
        }
    }

    public static Result<double> ParseInputToDouble(string? Value)
        => double.TryParse(Value, out double value)
            ? value.ToResultT
            : ResultT.Fail<double>("The value entered was not a number.");

    public static void Container_V1()
    {
        Console.WriteLine(Console.ReadLine().Containerize.Value);
    }

    public static void Container_V2()
    {
        Console.ReadLine()
            .Containerize
            .Map(value => double.Parse(value ?? string.Empty))
            .Map(Math.Sqrt)
            .Map(value => Math.Round(value, 2))
            .Write(Console.Write);

    }

    public static void Container_V3()
    {
        Console.ReadLine()
            .Containerize
            .Bind(TryParseToDouble)
            .Map(Math.Sqrt)
            .Map(value => Math.Round(value, 2))
            .Write(Console.Write);
    }

    public static Container<double> TryParseToDouble(string? value)
        => double.TryParse(value, out double result)
            ? result.Containerize
            : (0d).Containerize;

    public static void Result_V1()
    {
        Console.WriteLine(
            Console.ReadLine()
            .ToResultT
            .Bind(ConsoleApp.ParseInputToDouble)
            .Map(Math.Sqrt)
            .Map(value => Math.Round(value, 2))
            .Write(
                success: value => $"The result is: {value}",
                failure: exception => $"An error occured: {exception.Message}")
            );
    }

    public static async Task AsyncResult_V1()
    {
        Console.WriteLine(
            await DataProvider.GetDataAsync()
              .MapAsync(Math.Sqrt)
              .MapAsync(value => Math.Round(value, 2))
                .WriteAsync(
                    success: value => $"The result is: {value}",
                    failure: exception => $"An error occured: {exception.Message}")
        );
    }


    public static async Task AsyncResult_V2()
    {
        var result = await DataProvider.GetDataAsync();

        Console.Write(
            result
              .Map(Math.Sqrt)
              .Map(value => Math.Round(value, 2))
              .Write(
                  success: value => $"The result is: {value}",
                  failure: exception => $"An error occured: {exception.Message}")
      );
    }

}
public static class DataProvider
{
    public static async Task<Result<double>> GetDataAsync()
    {
        await Task.Delay(2000); // Simulate async data fetching
        return ResultT.Read((double)Random.Shared.Next(2, 100));
    }
}



