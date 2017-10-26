using System;
using data;

namespace main
{
    public class Functions
    {
        public static bool CheckFoodThresholdPercentage(Data.BlockData blockData)
        {
            var arr = blockData.EndGoal.Split(' ');

            var percentage = Convert.ToDouble(arr[0]);

            var n = Convert.ToInt32(arr[1]);
            if (n > BlockState.GetBlockLength()) return false;

            return BlockState.GetNumberItemsFound(n) / (double) n >= percentage;


        }
    }
}