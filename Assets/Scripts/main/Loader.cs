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

		public static void LogData(string s, bool append = true)
		{
			using (var writer = new StreamWriter ("Assets/OutputFiles~/" + DS.GetData ().CharacterData.OutputFile, append))
			{
				writer.Write (s + "\n");
				writer.Flush ();
				writer.Close();
			}	
		}

		public void LogData(Transform transform, bool collided = false)
		{
			//Ignore this for now.
		}
	}
}
