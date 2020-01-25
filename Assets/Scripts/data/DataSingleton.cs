using UnityEngine;

//This class is contains the central data object for all classes.
//This is implemented using a singleton design pattern.
namespace data
{
    public class DataSingleton
    {
        //Our singleton instance
        private static Data _data;

        public static Data GetData()
        {
            return _data;
        }

        //This function loads the file from a defacto location as shown below
        public static void Load(string fileName)
        {
            var file = System.IO.File.ReadAllText(fileName);

            var temp = JsonUtility.FromJson<Data>(file);
            _data = temp;
        }
    }
}
