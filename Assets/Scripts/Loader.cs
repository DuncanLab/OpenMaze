using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = DataSingleton;
using UnityEngine.UI;	
using System.IO;



//We will convert the initial loader into what is known as the level controller. 
//In order to properly set this up, there must exist a level config.
//We must also construct a level config builder.
//TODO: Create an experiment manager singleton in order to not make everything ew.
public class Loader : MonoBehaviour {

	//This enum represents the current state of experiment progression.
	public enum ExperimentProgression{
		Beginning,
		Ended,
		InWaiting,
		InExperiment
	}

	//This represents the source of the experiment ending
	public enum ExperimentEndSrc
	{
		Never, //This implies that the experiment hasn't started yet
		External, //This implies that the experiment was ended OUTSIDE the class.
		Internal //This implies the experiment was ended internally.
	}

	public static ExperimentEndSrc experimentEndSrc = ExperimentEndSrc.Never;

	public static ExperimentProgression ep;

	public static bool experimentMode = false;
	public static List<List<int>> experiment;

	public static bool src = false;

	private static float runningTime;
	public static int experimentIndex;


	//Since we have a csv file of entirely numbers, instead of using a data object, we will instead use
	//a matrix.



	// Use this for initialization
	void Start () {
		DS.Load ();
			
		experiment = new List<List<int>> ();
		using(var reader = new StreamReader("Assets/InputFiles~/" +DS.GetData().ExperimentFile))
		{
			
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				var values = line.Split(',');
				List<int> arr = new List<int> ();
				foreach (var i in values){
					arr.Add (int.Parse(i));
				}
				experiment.Add (arr);
			}
		}



		DontDestroyOnLoad (this);
		SceneManager.LoadScene (2);
		runningTime = 0;
		experimentIndex = 0;
		experimentMode = false;
		ep = ExperimentProgression.Beginning;
	}
	
	// Update is called once per frame
	void Update () {

		checkExperimentStatus ();


		if (Input.GetKey(KeyCode.N) && !experimentMode && ep == ExperimentProgression.Beginning) {
			runExperiment (true);
		}

		if (Input.GetKey (KeyCode.Alpha0) && DS.GetData().DeveloperMode) {
			SceneManager.LoadScene (2);
			experimentMode = false;
			ep = ExperimentProgression.Beginning;
			experimentIndex = 0;
		}


		if (DS.GetData ().DeveloperMode && !experimentMode) { //Make sure this is in developer mode and not experiment mode..
			if (Input.GetKey (KeyCode.K)) {
				SceneManager.LoadScene (0);

			} else if (Input.GetKey (KeyCode.L)) {
				SceneManager.LoadScene (1);
			}
		}


		runningTime += Time.deltaTime;
	}


	void runExperiment (bool begin = false)
	{

		if (begin) {
			experimentIndex = 0;
			experimentMode = true;
			using (var writer = new StreamWriter ("Assets\\OutputFiles~\\" + DS.GetData ().CharacterData.OutputFile, false)) {
				writer.WriteLine ("Trial Number, time (seconds), x, y, target, angle");
			}
		}
		runningTime = 0;

		DS.GetData ().WallData.Sides = experiment[experimentIndex][3];
		DS.Save ();
		ep = ExperimentProgression.InWaiting;
		SceneManager.LoadScene (2);

	}

	public static void progressExperiment(ExperimentEndSrc end = ExperimentEndSrc.External){
		ep = ExperimentProgression.InWaiting;
		SceneManager.LoadScene (2);

		experimentEndSrc = end;

		experimentIndex++;
		if (experimentIndex >= experiment.Count) {
			ep = ExperimentProgression.Ended;
			experimentMode = false;
			return;
		}
		runningTime = 0;
		DS.GetData ().WallData.Sides = experiment[experimentIndex][3];
		DS.Save ();
	}


	void checkExperimentStatus ()
	{

		if (experimentMode) {
			if (ep == ExperimentProgression.InWaiting) {

				if (runningTime > experiment [experimentIndex] [4]) {
					ep = ExperimentProgression.InExperiment;
					SceneManager.LoadScene (experiment [experimentIndex] [0]);
					runningTime = 0;

				}
		
			} else {
				if (runningTime > experiment [experimentIndex] [1] + DS.GetData().CharacterData.Delay) {
					progressExperiment (ExperimentEndSrc.Internal);

				}

			}
		}
	}


}
