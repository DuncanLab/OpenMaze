using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour {

	public Text countText;
	public Text foundText;
	public int totalPickUpCounts;
public float foundTextShowTime;

	private Rigidbody rb;
	private int count;
	private float foundTextOutTime;
	private RawImage foundTextObj;
	public RawImage nextMaze;

	private float finishedTime;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		count = 0;	
		SetCountText ();
		foundTextObj = foundText.GetComponentsInParent<RawImage>()[0];
		foundTextObj.gameObject.SetActive(false);
		nextMaze.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (foundText.IsActive ()) {
			if (Time.time > foundTextOutTime)
				foundTextObj.gameObject.SetActive (false);
		}

		if (nextMaze.IsActive ()) {
			if (Time.time >= finishedTime + 5.0f) {
				Application.LoadLevel ("Maze1");
			}
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			other.gameObject.SetActive (false);
			count++;
			if (count == totalPickUpCounts) {
				
				nextMaze.gameObject.SetActive (true);
				float finishedTime = Time.time;

			} else {
				SetCountText ();
				ParticleEffectController pec = other.transform.parent.gameObject.GetComponent<ParticleEffectController> ();
				pec.PickedUp ();
				ShowFoundText ();
			}
		}


	}

	void SetCountText () {
		countText.text = "Count: " + count.ToString () + " out of " + totalPickUpCounts.ToString () + " picked up";
	}

	void ShowFoundText (){
		foundTextObj.gameObject.SetActive(true);
		foundTextOutTime = Time.time + foundTextShowTime;
	}
}
