using System.IO;
using System.Reflection;
using data;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = data.DataSingleton;
using C = data.Constants;

namespace main
{

	/// <summary>
	/// Main entry point of the app as well as the game object that stays alive for all scenes.
	/// </summary>
	public class Loader : MonoBehaviour
	{
		public static Loader Get()
		{
			return GameObject.Find("Loader").GetComponent<Loader>();
		}
		
		
		public LinkedListNode CurrTrial;
		public float RunningTime;

		public class LinkedListNode
		{
			public Data.Trial Value;
			public LinkedListNode Next;
		}		
		private void Start () {
			DontDestroyOnLoad(this);
			DS.Load ();
			Directory.CreateDirectory(C.OutputDirectory);

			CurrTrial = new LinkedListNode();

			var temp = CurrTrial;
			int cnt = 0;
			foreach (var i in DS.GetData().TrialOrder)
			{
				temp.Value = DS.GetData().TrialData[i];
				if (cnt++ != DS.GetData().TrialOrder.Count - 1)
					temp.Next = new LinkedListNode();
				temp = temp.Next;
			}
			LogData("", false);
			SceneManager.LoadScene(C.LoadingScreen);
		}

		public void Progress()
		{
			if (CurrTrial.Next != null)
			{
				RunningTime = 0;
				CurrTrial = CurrTrial.Next;
				if (CurrTrial.Value.FileLocation != null)
				{
					SceneManager.LoadScene(C.LoadingScreen);
				} else if (CurrTrial.Value.TwoDimensional)
				{
					SceneManager.LoadScene(CurrTrial.Value.EnvironmentType + 4);
				}
				else
				{
					
					foreach (FieldInfo prop in typeof(Data.Trial).GetFields())
					{
						var s = prop.Name + ", " + prop.GetValue(CurrTrial.Value);
						LogData(s);
					}
					SceneManager.LoadScene(CurrTrial.Value.EnvironmentType + 2);
				}

			}
		}
		
		private void LateUpdate()
		{
			HandleInput();
			if (CheckTimeOut())
			{
				if (CurrTrial.Value.TwoDimensional)
				{
					LogData("TwoD, x, NA, y, NA, time, NA");
				}
				Progress();
			}
			RunningTime += Time.deltaTime;
		}
		
		
		public bool CheckTimeOut()
		{
			return CurrTrial.Value.TimeAllotted > 0 
			       && RunningTime > CurrTrial.Value.TimeAllotted;
		}
		
		
		private void HandleInput()
		{
			if (CurrTrial.Value.TimeAllotted > 0) return;
			if (Input.GetKey(KeyCode.Space))
			{
				Progress();
			}			
		}

		public static void LogData(string data, bool append = true)
		{
			using (var writer = new StreamWriter ("Assets/OutputFiles~/" + DS.GetData ().CharacterData.OutputFile, append))
			{
				writer.Write (data + "\n");
				writer.Flush ();
				writer.Close();
			}	
		}
		
	}
}
