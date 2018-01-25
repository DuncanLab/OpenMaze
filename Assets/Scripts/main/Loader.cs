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
					"TargetX, TargetY, LastX, LastY, BlockID, TrialID, Subject, Delay, 2D, Visible, UpArrow, DownArrow," +
					" LeftArrow, RightArrow, Space, Session, Note"
				+ "\n");
				writer.Flush ();
				writer.Close();
			}	
		}

		private static float _timer = 0;
		
		public static void LogData(TrialProgress s, float timestamp, Transform t, int targetFound = 0)
		{
			if (_timer > 1f / (DS.GetData().OutputTimesPerSecond == 0 ? 1000 : DS.GetData().OutputTimesPerSecond) || targetFound == 1)
			{
				using (var writer = new StreamWriter("Assets/OutputFiles~/" + DS.GetData().CharacterData.OutputFile, true))
				{

					string str = string.Format(
						"{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, " +
						"{12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}",
						s.TrialNumber, timestamp, t.position.x, t.position.z, t.eulerAngles.y, s.EnvironmentType, s.Sides,
						targetFound, s.PickupType, s.TargetX, s.TargetY, s.LastX, s.LastY, s.BlockID, s.TrialID,
						s.Subject, s.Delay, s.TwoDim, s.Visible, Input.GetKey(KeyCode.UpArrow) ? 1 : 0,
						Input.GetKey(KeyCode.DownArrow) ? 1 : 0, Input.GetKey(KeyCode.LeftArrow) ? 1 : 0,
						Input.GetKey(KeyCode.RightArrow) ? 1 : 0,
						Input.GetKey(KeyCode.Space) ? 1 : 0, s.SessionID, s.Note);
					writer.Write(str + "\n");
					writer.Flush();
					writer.Close();
				}

				_timer = 0;
			}

			_timer += Time.deltaTime;

		}

	}
}
