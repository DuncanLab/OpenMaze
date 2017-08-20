
//A class containing all of the useful constants to be used throughout the program
namespace data
{
     public class Constants
     {
          public const string InputDirectory = "Assets/InputFiles~/";

          public const string InputFileSrcPath = InputDirectory + "config.json";
          public const string OutputDirectory = "Assets/Outputfiles~";


          //Scene numbers.
          public const int LoadScene = 0;
          public const int Jungle2D = 1;
          public const int CityTerrain = 2;
          public const int JungleTerrain = 3;
          public const int LoadingScreen = 4;
          public const int City2D = 5;

     
          //Here we initialize a private constructor to prevent initialization of this class
          private Constants(){}

     }
}
