using System.Runtime.CompilerServices;

namespace main
{
    
    public class BlockState
    {

        private static double  _blockTime;

        private static int _numberItemsFound;

        public static double GetCurrentBlockTime()
        {
            return _blockTime;
        }

        public static void Update()
        {
            _blockTime++;
        }
        
        public static void Reset()
        {
            _blockTime = 0;
            _numberItemsFound = 0;
        }
        
        public static int GetNumberItemsFound()
        {
            return _numberItemsFound;
        }
        

        
        
        
        
        public static void Found()
        {
            _numberItemsFound++;
        }
        
        
    }
    
    public class Functions
    {
           
    }
}