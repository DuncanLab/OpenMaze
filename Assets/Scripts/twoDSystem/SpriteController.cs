using System;
using System.IO;
using System.Runtime.InteropServices;
using trial;
using UnityEngine;
using UnityEngine.SceneManagement;
using wallSystem;
using DS = data.DataSingleton;
using G = wallSystem.GenerateWall;
using E = main.Loader;
using C = data.Constants;
namespace twoDSystem
{
	public class SpriteController : MonoBehaviour
	{
		// Update is called once per frame
		private void Update () {
			//This calculates the current amount of rotation frame rate independent
			var rotation = Input.GetAxis("Horizontal") * DS.GetData().CharacterData.RotationSpeed * Time.deltaTime;

			//This calculates the forward speed frame rate independent
			var moveDirection = new Vector3(0, Input.GetAxis("Vertical"), 0);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= DS.GetData().CharacterData.MovementSpeed;

			//Here is the movement system
			const double tolerance = 0.0001;

			//we move iff rotation is 0
			if (Math.Abs(Mathf.Abs(rotation)) < tolerance)
				GetComponent<CharacterController>().Move(moveDirection * Time.deltaTime);

			transform.Rotate(0, 0, -rotation);
		}
	}


}
