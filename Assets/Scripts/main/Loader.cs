using System.Collections.Generic;
using System.IO;
using data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DS = data.DataSingleton;
using C = data.Constants;
using BS = main.BlockState;

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

		public LinkedListNode<Block> Blocks;
		public LinkedListNode<Data.Trial> CurrTrial;
		public float RunningTime;

		public class LinkedListNode<T>
		{
			public T Value;
			public LinkedListNode<T> Next;

			
		}	
		
		
		private void Start () {
			DontDestroyOnLoad(this);
			DS.Load ();
			Directory.CreateDirectory(C.OutputDirectory);
			
		
			
			Blocks = new LinkedListNode<Block>();
			CurrTrial = new LinkedListNode<Data.Trial>();
			var temp = Blocks;
			
			
			var cnt = 0;
			foreach (var i in DS.GetData().BlockOrder)
			{
				temp.Value = new Block(DS.GetData().BlockList[i]);
				if (cnt++ != DS.GetData().BlockOrder.Count - 1)
					temp.Next = new LinkedListNode<Block>();
				temp = temp.Next;
			}

			Blocks.Value.Log();
			CurrTrial.Value = Blocks.Value.Peek();
			_inputDone = false;
		}

		public void Progress()
		{
			if (Blocks == null)
			{
				return;
			}
			RunningTime = 0;
	
			if (Blocks.Value.Progress() == null)
			{
				Blocks = Blocks.Next;
				if (Blocks == null) return;
				BlockState.Reset();
				Blocks.Value.Log();

			}

				
				
				
			CurrTrial.Value = Blocks.Value.Peek();

			
			if (CurrTrial.Value.FileLocation != null)
			{
				SceneManager.LoadScene(C.LoadingScreen);
			} else if (CurrTrial.Value.TwoDimensional == 1)
			{
				SceneManager.LoadScene(CurrTrial.Value.EnvironmentType + 4);
			}
			else
			{
					
				foreach (var prop in typeof(Data.Trial).GetFields())
				{
					var s = prop.Name + ", " + prop.GetValue(CurrTrial.Value);
					LogData(s);
				}
				SceneManager.LoadScene(CurrTrial.Value.EnvironmentType + 2);
			}
			
		}
		
		private void LateUpdate()
		{
			if (_inputDone) //This is the beginning text field
			{
				HandleInput();
				if (CheckTimeOut())
				{
					if (CurrTrial.Value.Color != null)
						BlockState.Failed();
						
					Progress();
				}
				RunningTime += Time.deltaTime;
				BS.Update(Time.deltaTime);
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
					Blocks.Value.Log(); 

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

			if (Input.GetKeyDown(KeyCode.Return) )
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
