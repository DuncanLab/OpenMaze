using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



//This class is contains the central data object for all classes.
//This is implemented using a singleton design pattern.
public class DataSingleton{

	//Our singleton instance
	private static Data _data;

	public static Data GetData(){
		return _data;
	}

	public static void SetData(Data data){
		DataSingleton._data = data;
	}

	//This function loads the file from a defacto loaction as shown below
	public static void Load(){
		string file = System.IO.File.ReadAllText("Assets/InputFiles~/input.json");
		_data = JsonUtility.FromJson<Data>(file);
	}

	//This function saves the current configuration into the text file.
	public static void Save(){
		string data = JsonUtility.ToJson (DataSingleton._data);
		System.IO.File.WriteAllText ("Assets/InputFiles~/input.json", data);
	}
}
