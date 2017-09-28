using Unity;
using UnityEngine;

namespace main
{   
    //Here is the primary interface that 
    public static class BlockState
    {

        private static double  _blockTime;

        private static int _numberItemsFound;

        public static double GetCurrentBlockTime()
        {
            return _blockTime;
        }

        public static int GetNumberItemsFound()
        {
            return _numberItemsFound;
        }
        
        
        
        
        
        //------------------- These are the game methods. Don't touch these -----------------------------
        
        public static void Update(double deltaTime)
        {
            _blockTime += deltaTime;
        }
        
        public static void Reset()
        {
            _blockTime = 0;
            _numberItemsFound = 0;
        }
        
        
        public static void Found()
        {
            _numberItemsFound++;
        }
        
        
    }
}