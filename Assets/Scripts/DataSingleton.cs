using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using C = Constants;

//This class is contains the central data object for all classes.
//This is implemented using a singleton design pattern.
public class DataSingleton{

	//Our singleton instance
	private static Data _data;

	public static Data GetData(){
		return _data;
	}

	public static void SetData(Data data){
		_data = data;
	}

	//This function loads the file from a defacto loaction as shown below
	public static void Load(){
		string file = System.IO.File.ReadAllText(C.InputFileSrcPath);
		_data = JsonUtility.FromJson<Data>(file);
	}

	//This function saves the current configuration into the text file.
	public static void Save(){
		string data = JsonUtility.ToJson (_data);
		System.IO.File.WriteAllText (C.InputFileSrcPath, data);
	}
}
