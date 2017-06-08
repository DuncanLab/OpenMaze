using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBlock : MonoBehaviour {
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
	}
}
