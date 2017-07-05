using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is for when the food is visible.
public class RotateBlock : MonoBehaviour {

    public string type;

    // Update is called once per frame
    void Update () {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
	}
}
