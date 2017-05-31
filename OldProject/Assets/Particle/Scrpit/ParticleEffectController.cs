using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectController : MonoBehaviour {
//	private MeshRenderer mr;
//	private Animator animator;
	private GameObject spell;
	private bool control;
	private float offTime;
	public float duration = 2.0f;
	// Use this for initialization

	void Start () {
		int i = 0;
		while (i < 2) {
			if(this.gameObject.transform.GetChild(i).gameObject.CompareTag("Magic")) {
				spell = this.gameObject.transform.GetChild(i).gameObject;
//				mr = spell.GetComponent<MeshRenderer> ();
//				animator = spell.GetComponent<Animator> ();
			}
			i++;
		}
	}
	
	// Update is called once per frame
	void Update () { 
		if (control) {
			if (Time.time > offTime) {
				spell.SetActive (false);
				control = false;
			}
		}
	}

	public void PickedUp () {
		control = true;
		offTime = Time.time + duration;
		spell.SetActive (true);
	}
}
