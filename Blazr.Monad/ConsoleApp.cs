using Blazr.Manganese;
using System;
using System.Collections.Generic;
using System.Text;

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
    {
        if (double.TryParse(Value, out double value))
        {
            value = Math.Sqrt(value);
            value = double.Round(value, 2);
            return ResultT.Read(value);
        }
        else
        {
            return ResultT.Fail<double>("The value entered was not a number.");
        }
    }

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
}



