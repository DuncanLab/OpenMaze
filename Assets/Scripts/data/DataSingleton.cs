using UnityEngine;
using C = data.Constants;

//This class is contains the central data object for all classes.
//This is implemented using a singleton design pattern.
namespace data
{
	public class DataSingleton{

		//Our singleton instance
		private static Data _data;
		private static Data.InstructionXml _instructions;

		public static Data GetData(){
			return _data;
		}

		public static Data.InstructionXml GetInstructions()
		{
			return _instructions;
		}
	
		//This function loads the file from a defacto loaction as shown below
		public static void Load(){
			string file = System.IO.File.ReadAllText(C.InputFileSrcPath);
			_data = JsonUtility.FromJson<Data>(file);
			_instructions = Data.ParseXml();
		}

	}
}
