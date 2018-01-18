using System;
using System.Collections.Generic;
using System.IO;
using data;
using trial;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DS = data.DataSingleton;
using C = data.Constants;

namespace main
{

	/// <inheritdoc />
	/// <summary>
	/// Main entry point of the app as well as the game object that stays alive for all scenes.
	/// </summary>
	public class Loader : MonoBehaviour
	{
		public static Loader Get()
		{
			return GameObject.Find("Loader").GetComponent<Loader>();
		}
		public InputField[] Fields;

		public AbstractTrial CurrTrial;

		
		
		private void Start () {
			DontDestroyOnLoad(this);
			var inputFile = EditorUtility.OpenFilePanel("Choose Input File", "", "");
			
			
			
			DS.Load (inputFile);
			Directory.CreateDirectory(C.OutputDirectory);
			CurrTrial = new FieldTrial(Fields);
		}


		private void Update()
		{
			CurrTrial.Update(Time.deltaTime);
		}



		public static void LogFirst()
		{
			using (var writer = new StreamWriter ("Assets/OutputFiles~/" + DS.GetData ().CharacterData.OutputFile, false))
			{
				writer.Write (
					"Trial Number, Time (seconds), X, Y, Angle, Environment Type, Sides, TargetFound, PickupType, " +
					"TargetX, TargetY, LastX, LastY BlockID, TrialID, Subject, Delay, 2D, Visible, UpArrow, DownArrow, LeftArrow, RightArrow, Space, " 
				+ "\n");
				writer.Flush ();
				writer.Close();
			}	
		}

		public static void LogData(TrialProgress s, float timestamp, Transform t, int targetFound = 0)
		{
			using (var writer = new StreamWriter ("Assets/OutputFiles~/" + DS.GetData ().CharacterData.OutputFile, true))
			{
				writer.Write (s + "\n");
				writer.Flush ();
				writer.Close();
			}	
		}

	}
}
