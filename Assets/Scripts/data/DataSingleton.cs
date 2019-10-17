using System.Collections.Generic;
using UnityEngine;
using static data.Data;

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
            //_data = temp;

            _data = ConvertMazeListToDictionary(temp);
        }

        private static Data ConvertMazeListToDictionary(Data tempData)
        {
            tempData.MazesDictionary = new Dictionary<string, Maze>();

            foreach (var maze in tempData.Mazes)
            {
                if (!string.IsNullOrEmpty(maze.MazeName))
                {
                    tempData.MazesDictionary.Add(maze.MazeName, maze);
                }
            }

            return tempData;
        }
    }
}
