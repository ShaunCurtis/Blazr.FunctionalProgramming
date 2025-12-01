using Blazr.Manganese;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazr.FunctionalProgramming
{
    public static class DataProvider
    {
        public static async Task<Return<double>> GetDataAsync()
        {
            await Task.Delay(2000); // Simulate async data fetching
            return ReturnT.Read((double) Random.Shared.Next(2,100));
        }
    }
}
