using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerController : MonoBehaviour {

	public GameObject FPSC;
	private FirstPersonController FPSC_Script;

	public Text countText;
	public int totalPickUpCounts;
	public float foundTextShowTime;

	private GameObject objectiveType;
	public GameObject[] objectiveList = new GameObject[3];
	private string objectTag;
	private int objectIndex;

	// private Rigidbody rb;
	private int count;

	public RawImage foundTextObj;
	public RawImage nextMaze;
	public float nextMazeInterval;
	public RawImage nextObjective;
	public float nextObjectiveInterval;

	private float startTime;

	private float foundTextOutTime;
	private float nextMazeFinishedTime;
	private float nextObjectiveFinishedTime;

	public float totalTimeLimitInSeconds;

	private Vector3 initPos ;

	// Use this for initialization
	void Start () {
		// rb = GetComponent<Rigidbody> ();
		initPos = new Vector3 (0, 0.98f, 0);
		startTime = Time.time;
		count = 0;	
		nextMazeFinishedTime = 0;
		nextObjectiveFinishedTime = 0;

		objectIndex = 0;
		objectiveType = objectiveList [0];

		SetCountText ();
		nextMaze.gameObject.SetActive (false);
		nextObjective.gameObject.SetActive (false);
		objectTag = objectiveType.tag;

		FPSC_Script = FPSC.GetComponent<FirstPersonController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (foundTextObj.IsActive ()) {
			if (Time.time > foundTextOutTime) {
				foundTextObj.gameObject.SetActive (false);
				FPSC.transform.position = initPos;
				FPSC_Script.RandomizeViewAngle();
			}
		}
			
		if (nextMaze.IsActive ()) {
			if (Input.GetKey("Fire1")) {
				SceneManager.LoadScene ("Maze1", LoadSceneMode.Additive);
			}
		}

		if (nextObjective.IsActive ()) {
			if (Time.time > nextObjectiveFinishedTime) {
				FPSC.transform.position = initPos;
				objectiveType = objectiveList [objectIndex++];
				nextObjective.gameObject.SetActive (false);
			}
		}

		if (Time.time > startTime + totalTimeLimitInSeconds) {
			GoToNextMaze ();
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		
		if (other.gameObject.CompareTag (objectTag))
		{
			//FPSC.transform.position = initPos;
			other.gameObject.SetActive (false);
			count++;
			if (count == totalPickUpCounts) {
				objectIndex++;
				// Go to next maze
				if (objectIndex >= objectiveList.Length || objectiveList [objectIndex] == null) {
					GoToNextMaze ();
				} 
				// Go to next objectives
				else {
					objectiveType = objectiveList [objectIndex];
					objectTag = objectiveType.tag;
					count = 0;
					SetCountText ();
					GoToNextObjective ();
				}


			} else {
				SetCountText ();
				ParticleEffectController pec = other.transform.parent.gameObject.GetComponent<ParticleEffectController> ();
				pec.PickedUp ();
				ShowFoundText ();
			}
		}


	}

	void SetCountText () {
		if (objectiveType.gameObject.CompareTag("Health")) {
			countText.text = "H: " + count.ToString ();
		} else if (objectiveType.gameObject.CompareTag("Money")) {
			countText.text = "R: " + count.ToString ();
		} 
	}

	void ShowFoundText (){
		foundTextObj.GetComponentInChildren<Text> ().text = "You found one " + objectTag;
		foundTextObj.gameObject.SetActive(true);
		foundTextOutTime = Time.time + foundTextShowTime;
	}

	void GoToNextMaze () {
		nextMaze.gameObject.SetActive (true);
		nextMazeFinishedTime = Time.time + nextMazeInterval;
	}

	void GoToNextObjective () {
		objectiveType = objectiveList [objectIndex];
		nextObjective.GetComponentInChildren<Text> ().text = "Next objective: " + objectTag;
		nextObjective.gameObject.SetActive (true);
		nextObjectiveFinishedTime = Time.time + nextObjectiveInterval;
	}
}
