using System.IO;
using states;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = data.DataSingleton;
using G = wallSystem.GenerateWall;
using E = main.Loader;
using C = data.Constants;
namespace twoDSystem
{
	public class SpriteController : MonoBehaviour
	{
		private bool _ran;

		private void Start()
		{
			_ran = false;
		}
		
		private void LogData()
		{
			using (var writer = new StreamWriter ("Assets\\OutputFiles~\\" + DS.GetData ().CharacterData.OutputFile, true))
			{
				string line = E.Get().CurrTrial.Index + ", "
				              + transform.position.x + ", "
				              + transform.position.z + ", ";
				writer.Write (line);
				writer.Flush ();
				writer.Close();
			}
		}
	
		// Update is called once per frame
		private void Update () {
			
			if (Input.GetKey(KeyCode.Space) && !_ran)
			{
				LogData();
				E.Get().CurrentState = State.WaitFirst;
				E.Get().Progress();
				_ran = true;
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
				transform.position += trans;
			}

			transform.Rotate(0, 0, -rotation);
	

		}
	}


}
