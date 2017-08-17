
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


/// <summary>
/// The old data file present in the previous branch.
/// This is now the KING of the experiment system.
/// </summary>
[Serializable]
public class Data
{
	public string InstructionFile; //This is the csv file that contains the experiment data.
	//This is the object (defined below) which contains all data available for the main player
	public Character CharacterData;
	
	//This is a LIST of the different types of pickup items that are defiend below
	public List<PickupItem> PickupItems;
	
	//This is a list of the pillar objects
	public List<Pillar> Pillars;

	//This list orders the trials to be shown. 
	public List<int> TrialOrder;

	//This list contains all pre-defined trials.
	public List<Trial> TrialData;
	
	/// <summary>
	/// A sample trial.
	/// </summary>
	[Serializable]
	public class Trial
	{
		public int Index;
		public int EnvironmentType;
		public int Sides;
		public string Color;
		public int Radius;
		public int InnitialAngle;
		public int PickupType;
		public int PrewaitTime;
		public int TimeAllotted;
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
		public bool Visible; //Visibility
		public string PythonFile; //The python file that will generate the position
		public float Size; //The size of the object

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
	[System.Serializable]
	public class Pillar
	{
		public float X;
		public float Y;
		public float Radius;
		public float Height;
		public string Color;

	}
	
	//=========================== END OF JSON FIELDS ==========================================


	public class InstructionXml
	{

		public object this[string propertyName]
		{
			get { return GetType().GetProperty(propertyName).GetValue(this, null); }
			set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
		}

		public string Instructions { get; set; }
		public string WinMessage { get; set; }
		public string First { get; set; }
		public string LoseMessage { get; set; }
		public string EndMessage { get; set; }
	}

	public static InstructionXml ParseXml()
	{
		
		InstructionXml data = new InstructionXml();

		XmlReaderSettings readerSettings = new XmlReaderSettings {IgnoreComments = true};
		using (XmlReader reader = XmlReader.Create(Constants.InputDirectory + "Instructions.xml", readerSettings))
		{
			XmlDocument xml = new XmlDocument();

			xml.Load(reader);

			for (XmlNode sibling = xml.DocumentElement.FirstChild; sibling != null; sibling = sibling.NextSibling)
			{
				var nodeData = (sibling.FirstChild as XmlCDataSection).Data.Trim().Replace("\t", "");
				
				data[sibling.Name.Trim()] = nodeData;
				
				
			}

		}
		return data;
	}
	
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