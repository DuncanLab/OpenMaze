using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using DS = DataSingleton;

public class PickupGenerator : MonoBehaviour {
    private List<GameObject> _destroy;
    public GameObject Pickup;
    private Text _goalText;


	private static List<Data.Point> ReadFromExternal(string inputFile, string pickupType){
		var p = new Process
		{
			StartInfo = new ProcessStartInfo("python",
				"Assets/InputFiles~/" + inputFile + " " + pickupType + " " + Loader.experimentIndex)
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			}
		};

		p.Start ();
		p.WaitForExit ();
		var outputs = new List<Data.Point> ();
		while (!p.StandardOutput.EndOfStream) {
			var line = p.StandardOutput.ReadLine();
			var point = new Data.Point();
			if (line != null)
			{
				var arr = line.Split (',');
				point.X = float.Parse (arr [0]);
				point.Y = float.Parse (arr [1]);
			}
			outputs.Add (point);
		}

		while (!p.StandardError.EndOfStream) {
			var line = p.StandardError.ReadLine ();
			print (line);

		}


		return outputs;
	}

	// Use this for initialization
	private void Start () {	
		
        var gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();

        _destroy = new List<GameObject>(); //This initializes the food object destroy list

        //SETUP SEED SYSTEM HERE (probably initialize this with number of walls).
		Random.InitState(DS.GetData().WallData.Sides);

		Data.PickupItem item;
		if (!Loader.experimentMode) {
			item = DS.GetData ().PickupItems [Random.Range (0, DS.GetData ().PickupItems.Count)];
			gen.SetWaveSrc (item.SoundLocation);
		} else {
			item = DS.GetData ().PickupItems [Loader.experiment [Loader.experimentIndex] [2]];
			gen.SetWaveSrc (item.SoundLocation);

		}
        
        //Here is the text to determine the type of food that exists here
        _goalText = GameObject.Find("Goal").GetComponent<Text>();

        //And this section sets the text.
        _goalText.text = item.Tag;

		_goalText.color = Data.GetColour(item.Color);

		var p = ReadFromExternal (item.PythonFile, item.Tag);

		if (p.Count == 0) {
			print ("INVALID PYTHON INPUT FILE " + item.PythonFile);
			print ("PLEASE MAKE SURE THAT IT IS CONTAINED WITHIN THE INPUTFILES FOLDER AND IS SPELLED CORRECTLY");
			return;
		}



		//This for loop generates the pickup files.
        for (var i = 0; i < item.Count; i++)
        {
			var obj = Instantiate (Pickup);
        
			obj.transform.position = new Vector3 (p[i].X, 0.5f, p[i].Y);
			obj.transform.localScale = new Vector3 (1, 1, 1) * item.Size;
			var color = Data.GetColour (item.Color);
			obj.GetComponent<Renderer> ().material.color = color;
			if (!item.Visible)
				obj.GetComponent<Renderer>().enabled = false;
			

			_destroy.Add (obj);
        }
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
