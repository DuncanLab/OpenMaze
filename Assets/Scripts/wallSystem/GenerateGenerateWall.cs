using UnityEngine;
using UnityEngine.UI;
using DS = data.DataSingleton;
using E = main.Loader;
//This script is the Generate (GenerateWall) script
//This is essentially the god class, the backbone of the project.
namespace wallSystem
{
    public class GenerateGenerateWall : MonoBehaviour
    {
        //-------------------------- Fields -------------------------------------
        public GameObject Create; //This is the prefab of the GenerateWall object.
        public GameObject GenerateMazeFromFile;
        public Camera Cam;        //This is the main camera of the player.
        public Text Timer;        //This exists as the timer text.
        public GameObject Player;

        public int currBlockId; // The id of the current block being tested.
        public int currTrialId; // The id of the current trial being tested.

        //current generate wall object that exists. This is intrinsically different from 
        //the Create object as that is a prefab while this is the instance.
        private GameObject _currCreate;

        //This is the current running timestamp that is outputted to the outputfile.
        private readonly float _timestamp;

        // Use this for initialization
        private void Start()
        {
            Create.transform.position = Vector3.zero;
            _currCreate = Instantiate(E.Get().CurrTrial.maze.Sides == 0 ? GenerateMazeFromFile : Create);
        }

        private void Update()
        {

            currBlockId = E.Get().CurrTrial.BlockID;
            currTrialId = E.Get().CurrTrial.TrialID;

            if (DS.GetData().BlockList[currBlockId].ShowNumSuccessfulTrials)
            {
                Timer.text = "Number of successes: " + E.Get().CurrTrial.TrialProgress.NumSuccess;
            }
            else if (DS.GetData().BlockList[currBlockId].ShowCollectedPerTrial)
            {
                Timer.text = "Goals found this trial: " + E.Get().CurrTrial.NumCollected;
            }
            else if (DS.GetData().TrialData[currTrialId].ShowCollectedPerBlock)
            {
                Timer.text = "Goals found this block: " + E.Get().CurrTrial.TrialProgress.NumCollectedPerBlock[currBlockId];
            }
        }
    }
}
