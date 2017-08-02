
using System.Collections.Generic;
using UnityEngine;

///This is the data object which is entirely serializeable
//Fields should be edited here.
//This also serves as the documentation for the input.json file.
[System.Serializable]
public class Data
{
	public bool DeveloperMode; //This is a field that represents the developer mode. Most commands will work only in developer mode
	public string ExperimentFile; //This is the csv file that contains the experiment data.
	public string OutputFolderName; //This is the folder where screenshots will be sent to
	public bool OnCorner; //This represents if the rotation of the walls is based "on corner" or "on side". 
						  //Essentially only relevant for screen shots
	public bool ShowText;
	//This is the object (defined below) which contains all data available for the main player
	public Character CharacterData;
	
	//This is the object (defined below) which contains all data available for the walls
	public Wall WallData;
	
	//This is a LIST of the different types of pickup items that are defiend below
	public List<PickupItem> PickupItems;
	
	//This represents most fields regarding to the Walls
	//The colour of the walls will be a spectrum with the start colour being at MinNum and end colour being at MaxNum
	//The remaining walls are linear interpolations of the in between colours
	//The wall is setup essentially with each side tangent on the outside of the circle
	[System.Serializable]
	public class Wall 
	{
		
		public int Sides; //This is the number of sides
		public string StartColour; //This is the beginning colour in HEX without the #, Example RED = FF0000
		public string EndColour; //This is the ending colour in HEX without the #, Example RED = FF0000
		public int Radius; //This is the radius of the inner circle
		public int InitialAngle; //The initial angle with respect to standard 0
		public int MinNumWalls; //The minimum number of walls
		public int MaxNumWalls; //The max number of walls
		public int WallStep; //The number of walls increased/decreased by key 2 or 1 repectively
		public int WallHeight; //The height of the wall
	}


	//This is the given PickupItem. Note that in the JSON, this is contained with an ARRAY
	[System.Serializable]
	public class PickupItem
	{
		public int Count; //The number of Pickup Items to generate
							//In general this will always be one unless necessary to implement in the future.

		public string Tag; //The name of the pickup item
		public string Color; //The colour in Hex without #
		public string SoundLocation; //The file path of the sound
		public bool Visible; //Visibility
		public string PythonFile; //The python file that will generate the position
		public float Size; //The size of the object

	}


	[System.Serializable]	
	public class Character
	{
		public float CamRotation; //This is the rotation of the initial pan of the field
		public float Height; //The height of the camera
		public float MovementSpeed; //The movespeed of the character
		public float RotationSpeed; //The rotation speed of the character
		public int Delay; //How long the delay can last in the rotation
		public string OutputFile; //The output file of the character's movements during an experiment
		public Point CharacterStartPos; //The start position of the character (usually 0, 0)

	}


	[System.Serializable]
	public class Point //A serializable 2d point class.
	{
		public float X; //x value
		public float Y; //y value

		public Vector3 GetVector3()
		{
			return new Vector3(X, 0.2f, Y);
		}

		public override string ToString(){
			return "(" + X + ", " + Y + ")";
		}

	}

	//=========================== END OF JSON FIELDS ==========================================


	
	//This function converts the hex value to Colour.
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