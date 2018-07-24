using System.Security.Cryptography;
using data;
using UnityEngine;
using UnityEngine.UI;

using E = main.Loader;
using DS = data.DataSingleton;
//This script is the Generate (GenerateWall) script
//This is essentially the god class, the backbone of the project.
namespace wallSystem
{
	public class GenerateGenerateWall : MonoBehaviour {
		//-------------------------- Fields -------------------------------------
		public GameObject Create; //This is the prefab of the GenerateWall object.
		public GameObject GenerateMazeFromFile;
		public Camera Cam;		  //This is the main camera of the player.
		public Text Timer;		  //This exists as the timer text.
		public GameObject Player;

        public int currBlockId; // The id of the current block being tested.

		//current generate wall object that exists. This is intrinsically different from 
		//the Create object as that is a prefab while this is the instance.
		private GameObject _currCreate; 
	
		//This is the current running timestamp that is outputted to the outputfile.
		private float _timestamp;
	
		// Use this for initialization
		private void Start () {
			Create.transform.position = Vector3.zero;
			_currCreate = Instantiate(E.Get().CurrTrial.Value.Sides == 0 ?  GenerateMazeFromFile : Create);
		}

        private void Update()
        {

            currBlockId = E.Get().CurrTrial.BlockID;

            if (DS.GetData().BlockList[currBlockId].ShowCount)
            {
                Timer.text = "Objects found: " + E.Get().CurrTrial.TrialProgress.NumSuccess;
            }
        }
    }
}
