using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace data
{
	/// <summary>
	/// The old data file present in the previous branch.
	/// This is now the KING of the experiment system.
	/// </summary>
	[Serializable]
	public class Data
	{
		public int OutputTimesPerSecond;
		//This is the object (defined below) which contains all data available for the main player
		public Character CharacterData;
	
		//This is a LIST of the different types of pickup items that are defiend below
		public List<PickupItem> PickupItems;
	
		//This is a list of the pillar objects
		public List<Pillar> Pillars;

		
		//This list contains all pre-defined trials.
		public List<Trial> TrialData;

		//This is the wall height
		public float WallHeight;

		//Array of blocks
		public List<BlockData> BlockList;

		//Order of blocks
		public List<int> BlockOrder;
		
		//Each given block
		[Serializable]
		public class BlockData
		{
			public string EndGoal; //percentage ___SPACE___ number. This is very arbitrary.
			public string EndFunction; //The function name (if not present, we assume its always true)

			
			public string BlockName; //Name (outputed at the end of the Block)
			public string Notes; //Notes about the given block
			public int Replacement; //Integer value representing replacement
			public List<RandomData>	 RandomTrialType; //Array that contains all the possible random values
			public List<int> TrialOrder; //Trial order (-1 means random)
		}
		[Serializable]
		public class RandomData
		{
			public List<int> RandomGroup;
		}
		
		/// <summary>
		/// A sample trial.
		/// </summary>
		[Serializable]
		public class Trial
		{
			public int TwoDimensional; //Set to true iff trial is two dimensional
			public string FileLocation; //Is not null iff FileLocation exists (Image for 1D trials)
			public int EnvironmentType; //This is the environment type referenced.
			public int Sides; //Number of sides present in the trial.
			public string Color; //HEX color of the walls
			public int Radius; //Radius of the walls
			public int PickupType; //Pickup type associated with the block
			public int TimeAllotted; //Allotted amount of time
			public string PillarColor; //Color of the pillars
			public int RandomLoc; //Whether or not the pickup has a random loaction
			public int PickupVisible; //Visibility of the pickup
			public string Header; //Note outputted out of the trial.
			public MazeData Map;
			public List<int> ActivePickups;
			public List<int> InactivePickups;
			public int Quota;
		}


		//This is the given PickupItem. Note that in the JSON, this is contained with an ARRAY
		[Serializable]
		public class PickupItem
		{
			public int Count; //The number of Pickup Items to generate
			//In general this will always be one unless necessary to implement in the future.
			public string Tag; //The name of the pickup item
			public string Color; //The colour in Hex without #
			public string SoundLocation; //The file path of the sound
			public string PythonFile; //The python file that will generate the position
			public float Size; //The size of the object
			public string PrefabName; //The name of the prefab object
			public string ImageLoc;
		}


		[Serializable]	
		public class Character
		{
			public float CamRotation; //This is the rotation of the initial pan of the field
			public float Height; //The height of the camera
			public float MovementSpeed; //The movespeed of the character
			public float RotationSpeed; //The rotation speed of the character
			public int TimeToRotate; //How long the delay can last in the rotation
			public string OutputFile; //The output file of the character's movements during an experiment
			public Point CharacterStartPos; //The start position of the character (usually 0, 0)
			public float CharacterBound;
			public float PickupRotationSpeed;

			
			public float DistancePickup;
		}


		[Serializable]
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

		//This is essentially a pillar object.
		//Fields are pretty obvious.
		[Serializable]
		public class Pillar
		{
			public float X;
			public float Y;
			public float Radius;
			public float Height;
		}
	
		//=========================== END OF JSON FIELDS ==========================================
		
		[Serializable]
		public class MazeData
		{
			public List<float> TopLeft;
			public float TileWidth;
			public List<string> Map;
			public string Color;
		}

		//This function converts the hex value to Colour.
		public static Color GetColour(string hex)
		{
			float[] l = { 0, 0, 0 };
			for (var i = 0; i < 6; i += 2)
			{
				float decValue = int.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
				l[i / 2] = decValue / 255;
			}
			return new Color(l[0], l[1], l[2]);

		}
	}
}