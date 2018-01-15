using System.IO;
using System.Runtime.InteropServices;
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
			E.Get().CurrTrial.LogData(transform);

//			if (Input.GetKeyDown(KeyCode.Space))
//				E.Get().Progress();
//			
			//This calculates the current amount of rotation frame rate independent
			var rotation = Input.GetAxis("Horizontal") * DS.GetData().CharacterData.RotationSpeed * Time.deltaTime;


			var currAngle = transform.rotation.eulerAngles.y;

			var a1 = G.Sin(currAngle);
			var a2 = G.Cos(currAngle);
		
		
			var trans = new Vector3(a1, 0, a2);
			trans = trans.normalized * DS.GetData().CharacterData.MovementSpeed * Time.deltaTime;
		
			
			//Here is the movement system
			const double tolerance = 0.0001;
			if (Mathf.Abs(rotation) < tolerance && (Input.GetKey(KeyCode.UpArrow) ||	 Input.GetKey(KeyCode.DownArrow)))
			{
				if (Input.GetKey(KeyCode.DownArrow)) trans *= -1;
				GetComponent<CharacterController>().Move(trans);
			}
			
			
			
			transform.Rotate(0, 0, -rotation);


		}
	}


}
