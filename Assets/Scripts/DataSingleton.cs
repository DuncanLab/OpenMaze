using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This class is contains the central data object for all classes.
//This is implemented using a singleton design pattern.
public class DataSingleton{

	//Our singleton instance
	private static Data data;

	public static Data GetData(){
		return data;
	}

	public static void SetData(Data data){
		DataSingleton.data = data;
	}

	//This function loads the file from a defacto loaction as shown below
	public static void Load(){
		string file = System.IO.File.ReadAllText("Assets/InputFiles~/input.json");
		data = JsonUtility.FromJson<Data>(file);
	}

	//This function saves the current configuration into the text file.
	public static void Save(){
		string data = JsonUtility.ToJson (DataSingleton.data);
		System.IO.File.WriteAllText ("Assets/InputFiles~/input.json", data);
	}
}

//This is the data object which is entirely serializeable
//Fields should be edited here.
[System.Serializable]
public class Data
{
	public bool DeveloperMode;
	public string OutputFolderName;
	public bool OnCorner;

    [System.Serializable]
    public class Wall
    {
        public int Sides;
        public string StartColour;
        public string EndColour;
        public int Radius;
        public int InitialAngle;
        public int MinNumWalls;
        public int MaxNumWalls;
        public int WallStep;
        public int WallHeight;
    }
    [System.Serializable]
    public class Point
    {
        public float x;
        public float y;

        public Vector3 GetVector3()
        {
            return new Vector3(x, 0.2f, y);
        }

		public override string ToString(){
			return "(" + x + ", " + y + ")";
		}

    }

    [System.Serializable]
    public class PickupItem
    {
        public int Count;
        public string Tag;
        public string Color;
        public string SoundLocation;
        public bool Visible;
        public string PythonFile;
		public float Size;

    }


    [System.Serializable]	
    public class Character
    {
		public float CamRotation;
		public float Height;
        public float MovementSpeed;
        public float RotationSpeed;
        public int Delay;
        public string OutputFile;
        public Point CharacterStartPos;

    }
    

    public Character CharacterData;
    public Wall WallData;
    public List<PickupItem> PickupItems;
    //This function converts the hex value to floats.
    public static Color GetColour(string hex)
    {
        float[] l = { 0, 0, 0 };
        for (int i = 0; i < 6; i += 2)
		{
            float decValue = int.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
            l[i / 2] = decValue / 255;
        }
        return new Color(l[0], l[1], l[2]);

    }
}