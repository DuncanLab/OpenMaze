using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

//This class is contains the central data object for all classes.
//This is implemented using a singleton design pattern.
namespace data
{
    public class DataSingleton
    {
        //Our singleton instance
        private static Data _data = new Data();

        public static Data GetData()
        {
            return _data;
        }

        //This function loads the file from a defacto location as shown below
        public static void Load(string fileName)
        {
            var file = File.ReadAllText(fileName);
            try
            {
                _data = JsonConvert.DeserializeObject<Data>(file);
            }
            catch (JsonReaderException e)
            {
                Debug.LogError(e);
                Debug.LogError("INVALID JSON FILE");
                Application.Quit();
            }
        }
    }
}
