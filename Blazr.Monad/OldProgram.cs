//using Blazr.ResultMonad;
//using System;

//var input = Console.ReadLine();
//return;

//if (double.TryParse(input, out double result))
//{
//    double sqrt = Math.Sqrt(result);
//    double rounded = Math.Round(sqrt, 2);
//    Console.WriteLine($"Success: The transformed value is: {rounded}");
//}
//else
//    Console.WriteLine("The input value could not be parsed.");


//Console.ReadLine()
//    .Then(ConvertToDouble)
//    .Then(Square)
//    .Then(RoundToTwoDecimalPlaces)
//    .Then(WriteToConsole);

//Result<double> StringToDouble(string? input)
//{
//    try
//    {
//        if (double.TryParse(input, out double value))
//            return new SuccessResult<double>(value);
//        else
//            return new FailureResult<double>(ResultException.Null);
//    }
//    catch 
//    {
//        return new FailureResult<double>(ResultException.Null);
//    }
//}
////double ConvertToDouble(string? input) => double.TryParse(input, out double value) ? value : throw new ArgumentNullException();
//double Square(double value) => Math.Sqrt(value);
//double RoundToTwoDecimalPlaces(double value) => Math.Round(value, 2);
////void WriteToConsole(double value) => Console.WriteLine($"Success: The transformed value is: {value}");


////Container.Read(Console.ReadLine)
////    .Then(ConvertToDouble)
////    .Then(Square)
////    .Then(RoundToTwoDecimalPlaces)
////    .Write(WriteToConsole);


////Result.Read(Console.ReadLine)
////    .Then(value => double.Parse(value!))
////    .Then(Square)
////    .Then(RoundToTwoDecimalPlaces)
////    .Write(
////        success: value => Console.WriteLine($"Success: The transformed value is: {value}"),
////        failure: () => Console.WriteLine($"Failure: The value provided was not a number."));


////Result.Read(Console.ReadLine)
////    .Map(value => double.Parse(value!))
////    .Map(Square)
////    .Map(RoundToTwoDecimalPlaces)
////    .Match(
////        success: value => Console.WriteLine($"Success: The transformed value is: {value}"),
////        failure: () => Console.WriteLine($"Failure: The value provided was not a number."));


////Result.Read(Console.ReadLine)
////    .Bind(StringToDouble)
////    .Map(Square)
////    .Map(RoundToTwoDecimalPlaces)
////    .Match(
////        success: value => Console.WriteLine($"Success: The transformed value is: {value}"),
////        failure: () => Console.WriteLine($"Failure: The value provided was not a number."));


//Console.ReadLine()
//    .ToResult()
//    .Bind(StringToDouble)
//    .Map(Square)
//    .Map(RoundToTwoDecimalPlaces)
//    .Match(
//        success: value => Console.WriteLine($"Success: The transformed value is: {value}"),
//        failure: exception => Console.WriteLine($"Failure: {exception.Message}."));

//internal static class MyExt
//{
//    extension<T> (Result<T> @this)
//    {
//        internal void WriteToConsole(Func<T> func)
//        {
//            Console.Write(@this switch
//            {
//                SuccessResult<T> s => $"Success: The transformed value is: {s.Value}",
//                FailureResult<T> f => $"Failure: {f.Exception.Message}.",
//                _ => "Failure: The value provided was not a number."
//            });
//        }
//    }
//} 

//public static class ThenExtensions
//{
//    extension(string? @this)
//    {
//        public double Then(Func<string?, double> then) => then.Invoke(@this);
//    }

//    extension(double @this)
//    {
//        public double Then(Func<double, double> then) => then.Invoke(@this);

//        public void Then(Action<double> then) => then.Invoke(@this);
//    }
//}

