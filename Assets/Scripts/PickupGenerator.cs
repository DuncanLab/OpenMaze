using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using DS = DataSingleton;
public class PickupGenerator : MonoBehaviour {
    List<GameObject> destroy;
    public GameObject pickup;
    public Text goalText;

    //This function supposedly generates a random normal number with given mu sd
    //This is done using the Marsaglia Polar Method
    //https://en.wikipedia.org/wiki/Marsaglia_polar_method
    

	List<Data.Point> ReadFromExternal(string InputFile, int numValues){
		Process p = new Process();
		p.StartInfo = new ProcessStartInfo ("python", "Assets/InputFiles/" + InputFile + " " + numValues);
		p.StartInfo.RedirectStandardOutput = true; 
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.CreateNoWindow = true;
		p.Start ();
		p.WaitForExit ();
		List<Data.Point> outputs = new List<Data.Point> ();
		while (!p.StandardOutput.EndOfStream) {
			string line = p.StandardOutput.ReadLine();
			Data.Point point = new Data.Point();
			string[] arr = line.Split (new char[]{','});
			point.x = float.Parse (arr [0]);
			point.y = float.Parse (arr [1]);
			outputs.Add (point);
		}

		return outputs;
	}

	// Use this for initialization
	void Start () {	

        GenerateGenerateWall gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();

        destroy = new List<GameObject>(); //This initializes the food object destroy list

        //SETUP SEED SYSTEM HERE (probably initialize this with number of walls).
		Random.InitState(DS.GetData().WallData.Sides);



		Data.PickupItem pickup = DS.GetData().PickupItems[(int)Random.Range(0, DS.GetData().PickupItems.Count)];
        gen.SetWaveSrc(pickup.SoundLocation);
        
        //Here is the text to determine the type of food that exists here
        goalText = GameObject.Find("Goal").GetComponent<Text>();

        //And this section sets the text.
        goalText.text = pickup.Tag;

		List<Data.Point> p = ReadFromExternal (pickup.PythonFile, pickup.Count);

		if (p.Count == 0) {
			print ("INVALID PYTHON INPUT FILE " + pickup.PythonFile);
			print ("PLEASE MAKE SURE THAT IT IS CONTAINED WITHIN THE INPUTFILES FOLDER AND IS SPELLED CORRECTLY");
			return;
		}


		if (!pickup.Visible)
			return;

		//This for loop generates the pickup files.
        for (int i = 0; i < pickup.Count; i++)
        {
			GameObject obj = Instantiate (this.pickup);
        
			obj.transform.position = new Vector3 (p[i].x, 0.5f, p[i].y);
			obj.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
			Color color = Data.GetColour (pickup.Color);
			obj.GetComponent<Renderer> ().material.color = color;


			destroy.Add (obj);
        }
    }

    //And here we destroy all the food.
    private void OnDestroy()
    {
        for (int i = 0; i < destroy.Count; i++)
        {
            if (destroy[i] != null) Destroy(destroy[i]);
        }
    }
    

}
