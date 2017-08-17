using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Director;
using DS = DataSingleton;
using G = GenerateWall;

public class SpriteController : MonoBehaviour
{


	
	// Update is called once per frame
	private void Update () {
		
		//This calculates the current amount of rotation frame rate independent
		float rotation = Input.GetAxis("Horizontal") * DS.GetData().CharacterData.RotationSpeed * Time.deltaTime;


		float currAngle = transform.rotation.eulerAngles.y;

		var a1 = G.Sin(currAngle);
		var a2 = G.Cos(currAngle);
		
		
		Vector3 trans = new Vector3(a1, 0, a2);
		trans = trans.normalized * DS.GetData().CharacterData.MovementSpeed * Time.deltaTime;
		
		//Here is the movement system
		const double tolerance = 0.0001;
		if (Mathf.Abs(rotation) < tolerance && Input.GetKey(KeyCode.UpArrow))
		{
			transform.position += trans;
		}

		transform.Rotate(0, 0, -rotation);
	

	}
}
