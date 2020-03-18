using Newtonsoft.Json;

// This class contains the central data object for all classes.
namespace data
{
    public class DataSingleton
    {
        // Our singleton instance
        private static Data _data;

        public static Data GetData()
        {
            return _data;
        }

        // Load the configuration json with all the experiment settings
        public static void Load(string fileName)
        {
            string configFileJson = System.IO.File.ReadAllText(fileName);

            _data = JsonConvert.DeserializeObject<Data>(configFileJson);
        }
    }
}
