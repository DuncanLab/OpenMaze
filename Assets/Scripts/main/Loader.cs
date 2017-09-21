using System.IO;
using System.Reflection;
using data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

		public InputField[] TextBoxes;
		private bool _inputDone;
		
		
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
			_inputDone = false;
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
				} else if (CurrTrial.Value.TwoDimensional == 1)
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
			if (_inputDone)
			{
				HandleInput();
				if (CheckTimeOut())
				{

					Progress();
				}
				RunningTime += Time.deltaTime;
			}
			else
			{
				if (Input.GetKeyDown(KeyCode.Return))
				{
					LogData("", false);
					foreach (var textBox in TextBoxes)
					{
						var arr = textBox.transform.GetComponentsInChildren<Text>();
						LogData(arr[0].text + ": " + arr[1].text);
						
					}					
					_inputDone = true;
					SceneManager.LoadScene(1);
				}
			}
		}


		private bool CheckTimeOut()
		{
			return CurrTrial.Value.TimeAllotted > 0 
			       && RunningTime > CurrTrial.Value.TimeAllotted;
		}
		
		
		private void HandleInput()
		{
			if (CurrTrial.Value.TimeAllotted > 0) return;

			if (Input.GetKeyDown(KeyCode.Space) )
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
