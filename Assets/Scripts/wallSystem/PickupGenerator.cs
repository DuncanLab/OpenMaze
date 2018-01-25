using System.Collections.Generic;
using System.Diagnostics;
using data;
using UnityEngine;
using UnityEngine.UI;
using DS = data.DataSingleton;
using E = main.Loader;

namespace wallSystem
{
	public class PickupGenerator : MonoBehaviour {
		private List<GameObject> _destroy;
		

		private static Data.Point ReadFromExternal(string inputFile){
			var p = new Process
			{
				StartInfo = new ProcessStartInfo("python",
					"Assets/InputFiles~/" + inputFile)
				{
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};
			p.Start ();
			p.StandardInput.Write(JsonUtility.ToJson(E.Get().CurrTrial.Value) +"\n");

			p.WaitForExit ();
			var line = p.StandardOutput.ReadLine();

			
			while (!p.StandardError.EndOfStream) {
				var outputLine = p.StandardError.ReadLine ();
				UnityEngine.Debug.LogError(outputLine);

			}

			if (line == null)
			{
				UnityEngine.Debug.LogError("PYTHON FILE ERROR!");
				return new Data.Point{X = 5, Y = 5};
			}
		
			var arr = line.Split (',');
			return new Data.Point
			{
				X = float.Parse(arr[0]),
				Y = float.Parse(arr[1])
			};

		}

		// Use this for initialization
		private void Start () {	
		
			var gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();

			_destroy = new List<GameObject>(); //This initializes the food object destroy list

			var goalText = GameObject.Find("Goal").GetComponent<Text>();
			goalText.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 30);

			var val = E.Get().CurrTrial.Value.PickupType;
			if (val == 0)
			{
				goalText.text = E.Get().CurrTrial.Value.Header ?? "Explore";
				goalText.color = Color.white;
				return;
			} 
			var item = DS.GetData ().PickupItems [Mathf.Abs(val) - 1];
			gen.SetWaveSrc (item.SoundLocation);
			
        
			//Here is the text to determine the type of food that exists here

			//And this section sets the text.
			goalText.text = E.Get().CurrTrial.Value.Header ?? item.Tag;
			goalText.color = Data.GetColour(item.Color);

			if (val < 0)
				return;
			
		
		
			var p =  ReadFromExternal (item.PythonFile);
			GameObject.Find("FirstPerson").GetComponent<PlayerController>().ExternalStart(p.X, p.Y);
			
			var prefab = (GameObject)Resources.Load("prefabs/" + item.PrefabName, typeof(GameObject));
			prefab.AddComponent<RotateBlock>();
			
			
			
			
			var obj = Instantiate (prefab);
	
			obj.transform.localScale = new Vector3 (1, 1, 1) * item.Size;
			obj.transform.position = new Vector3 (p.X, prefab.GetComponent<Renderer>().bounds.size.y/2, p.Y);

			var color = Data.GetColour (item.Color);
			obj.GetComponent<Renderer>().material.color = color;
			obj.GetComponent<Renderer>().enabled = E.Get().CurrTrial.Value.PickupVisible == 1;
		

			_destroy.Add (obj);
	
		}

		//And here we destroy all the food.
		private void OnDestroy()
		{
			foreach (var t in _destroy)
			{
				if (t != null) Destroy(t);
			}
		}
    
	}
}