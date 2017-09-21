using System.IO;
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

		private void LogData()
		{
			var str = "TwoD, x, " + transform.position.x + ", y, " + transform.position.z + ", time, " + E.Get().RunningTime;
			E.LogData(str);	
		}



		// Update is called once per frame
		private void Update () {
			LogData();

			if (Input.GetKey(KeyCode.Space))
			{
				
				
				E.Get().Progress();
			}
			
		
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
				GetComponent<CharacterController>().Move(trans);
			}
			
			
			
			transform.Rotate(0, 0, -rotation);


		}
	}


}
