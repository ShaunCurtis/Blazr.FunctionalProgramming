using Blazr.Manganese;
using Blazr.Monad;
using System;

Console.WriteLine( 
    Console.ReadLine()
    .ToResultT
    .Bind(ConsoleApp.ParseInputToDouble)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .Write(
        success: value => $"The result is: {value}",
        failure: exception =>  $"An error occured: {exception.Message}")
    );