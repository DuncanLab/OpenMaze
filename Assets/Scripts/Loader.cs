using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = DataSingleton;
using System.IO;
using System.Linq;
using System.Xml;


//We will convert the initial loader into what is known as the level controller. 
//In order to properly set this up, there must exist a level config.
//We must also construct a level config builder.
//TODO: Create an experiment manager singleton in order to not make everything ew.
public class Loader : MonoBehaviour
{
	public static InstructionXml InstructionData;

	//This enum represents the current state of experiment progression.
	public enum ExperimentProgression
	{
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

	public static ExperimentEndSrc EndSrc = ExperimentEndSrc.Never;

	public static ExperimentProgression Ep;

	public static bool ExperimentMode;

	//Since we have a csv file of entirely numbers, instead of using a data object, we will instead use
	//a matrix.
	public static List<List<int>> ExperimentCsv;




	public static bool Src = false;

	private static float _runningTime;
	public static int ExperimentIndex;


	public class InstructionXml
	{

		public object this[string propertyName]
		{
			get { return GetType().GetProperty(propertyName).GetValue(this, null); }
			set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
		}

		public string Instructions { get; set; }
		public string WinMessage { get; set; }
		public string First { get; set; }
		public string LoseMessage { get; set; }
		public string EndMessage { get; set; }
	}




	public static InstructionXml ParseXml()
	{
		
		InstructionXml data = new InstructionXml();

		XmlReaderSettings readerSettings = new XmlReaderSettings {IgnoreComments = true};
		using (XmlReader reader = XmlReader.Create(Constants.InputDirectory + "Instructions.xml", readerSettings))
		{
			XmlDocument xml = new XmlDocument();

			xml.Load(reader);

			for (XmlNode sibling = xml.DocumentElement.FirstChild; sibling != null; sibling = sibling.NextSibling)
			{
				var nodeData = (sibling.FirstChild as XmlCDataSection).Data.Trim().Replace("\t", "");
				
				data[sibling.Name.Trim()] = nodeData;
				
				
			}

		}
		return data;
	}





	private static void _initExperiment()
	{
		ExperimentCsv = new List<List<int>> ();
		using(var reader = new StreamReader("Assets/InputFiles~/" +DS.GetData().ExperimentFile))
		{
			
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				if (line == null) continue;
				var values = line.Split(',');
				var arr = values.Select(int.Parse).ToList();
				ExperimentCsv.Add (arr);
			}
		}

	}

	// Use this for initialization
	private void Start () {
		DS.Load ();
		InstructionData = ParseXml();
		_initExperiment();		

		DontDestroyOnLoad (this);
		SceneManager.LoadScene (2);
		_runningTime = 0;
		ExperimentIndex = 0;
		ExperimentMode = false;
		Ep = ExperimentProgression.Beginning;
	}
	
	// Update is called once per frame
	private void Update () {

		CheckExperimentStatus ();


		if (Input.GetKey(KeyCode.N) && !ExperimentMode && Ep == ExperimentProgression.Beginning) {
			RunExperiment (true);
		}

		if (Input.GetKey (KeyCode.Alpha0) && DS.GetData().DeveloperMode) {
			SceneManager.LoadScene (2);
			ExperimentMode = false;
			EndSrc = ExperimentEndSrc.Never;
			Ep = ExperimentProgression.Beginning;
			ExperimentIndex = 0;
		}


		if (DS.GetData ().DeveloperMode && !ExperimentMode) { //Make sure this is in developer mode and not experiment mode..
			if (Input.GetKey (KeyCode.K)) {
				SceneManager.LoadScene (0);

			} else if (Input.GetKey (KeyCode.L)) {
				SceneManager.LoadScene (1);
			}
		}


		_runningTime += Time.deltaTime;
	}


	private static void RunExperiment (bool begin = false)
	{

		if (begin) {
			ExperimentIndex = 0;
			ExperimentMode = true;
			using (var writer = new StreamWriter ("Assets\\OutputFiles~\\" + DS.GetData ().CharacterData.OutputFile, false)) {
				writer.WriteLine ("Trial Number, time (seconds), x, y, target, angle");
			}
		}
		_runningTime = 0;

		DS.GetData ().WallData.Sides = ExperimentCsv[ExperimentIndex][3];
		DS.Save ();
		Ep = ExperimentProgression.InWaiting;
		SceneManager.LoadScene (2);

	}

	public static void ProgressExperiment(ExperimentEndSrc end = ExperimentEndSrc.External){
		Ep = ExperimentProgression.InWaiting;
		SceneManager.LoadScene (2);

		EndSrc = end;

		ExperimentIndex++;
		if (ExperimentIndex >= ExperimentCsv.Count) {
			Ep = ExperimentProgression.Ended;
			ExperimentMode = false;
			return;
		}
		_runningTime = 0;
		DS.GetData ().WallData.Sides = ExperimentCsv[ExperimentIndex][3];
		DS.Save ();
	}


	public static void CheckExperimentStatus ()
	{
		if (!ExperimentMode) return;
		if (Ep == ExperimentProgression.InWaiting) {
			if (!(_runningTime > ExperimentCsv[ExperimentIndex][4])) return;
			Ep = ExperimentProgression.InExperiment;
			SceneManager.LoadScene (ExperimentCsv [ExperimentIndex] [0]);
			_runningTime = 0;
		} else {
			if (_runningTime > ExperimentCsv [ExperimentIndex] [1] + DS.GetData().CharacterData.Delay) {
				ProgressExperiment (ExperimentEndSrc.Internal);

			}

		}
	}


}
