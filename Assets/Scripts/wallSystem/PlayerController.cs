using System;
using System.IO;
using states;
using UnityEngine;
using DS = data.DataSingleton;
using Random = UnityEngine.Random;
using E = main.Loader;
using C = data.Constants;
//This class is the primary player script.
//This allows us to move around essentially.
namespace wallSystem
{
	public class PlayerController : MonoBehaviour {
   
		public Camera Cam;
		public static int ObjectsFound;
		private GenerateGenerateWall _gen;
    
    
		//The stream writer that writes data out to an output file.
		private string _outDir;

		//This is the character controller system used for collision
		private CharacterController _controller;

		//The initial move direction is static zero.
		private Vector3 _moveDirection = Vector3.zero;

		private float _currDelay;
	
		private float _iniRotation;

		private float _waitTime;
		
		private void Start()
		{
			_currDelay = 0;
			_iniRotation = Random.Range (0, 360);
			transform.Rotate (new Vector3 (0, _iniRotation, 0));
			_controller = GetComponent<CharacterController>();
			_gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();
			Cam.transform.Rotate (-DS.GetData().CharacterData.CamRotation, 0, 0);
			_waitTime = DS.GetData().CharacterData.TimeToRotate;
			ObjectsFound = 0;
		}

		private void LogData(bool collided)
		{
			using (var writer = new StreamWriter ("Assets\\OutputFiles~\\" + DS.GetData ().CharacterData.OutputFile, true)) {
				string line = E.Get().CurrTrial.Index + ", "
				              + E.Get().RunningTime+ ", "
				              + transform.position.x + ", "
				              + transform.position.z + ", "
				              + collided + ", "
				              + transform.rotation.eulerAngles.y + "\n";
				writer.Write (line);
				writer.Flush ();
				writer.Close();
			}
		}

		//This is the collision system.
		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Pickup")) return;
			ObjectsFound++;
			GetComponent<AudioSource> ().PlayOneShot (_gen.GetWaveSrc (), 1);
			Destroy (other.gameObject);
			LogData(true);
			
			E.Get().CatchEvent(new states.Event(State.PlayingSound));

		}

		private void LateUpdate(){
			LogData(false);

		}

		private void ComputeMovement()
		{
			//This calculates the current amount of rotation frame rate independent
			float rotation = Input.GetAxis("Horizontal") * DS.GetData().CharacterData.RotationSpeed * Time.deltaTime;


			//This calculates the forward speed frame rate independent
			_moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
			_moveDirection = transform.TransformDirection(_moveDirection);
			_moveDirection *= DS.GetData().CharacterData.MovementSpeed;

			//Here is the movement system
			const double tolerance = 0.0001;

			//we move iff rotation is 0
			if (Math.Abs(Mathf.Abs(rotation)) < tolerance)
				_controller.Move(_moveDirection * Time.deltaTime);

			transform.Rotate(0, rotation, 0);

		}
		
		private void Update()
		{

			if (E.Get().CurrentState == State.PlayingSound)
			{
				if (!GetComponent<AudioSource>().isPlaying)
				{
					if (E.Get().Progress())
					{

							
						E.Get().CatchEvent(
							new TransitionEvent(
								C.LoadingScreen,
								E.Get().CurrTrial.PickupType > 0 ? State.Won : State.TwoDim

							));
						
					}
					return;
				}
			}
			else if (_currDelay < _waitTime)
			{
				float angle = 360f * _currDelay / _waitTime + _iniRotation - transform.rotation.eulerAngles.y;
				transform.Rotate(new Vector3(0, angle, 0));
				if (_waitTime - _currDelay < 1 / 30f)
				{
					Cam.transform.Rotate(DS.GetData().CharacterData.CamRotation, 0, 0);
					E.Get().RunningTime = 0;
				}
			}
			else
			{
				ComputeMovement();
			}
			_currDelay += Time.deltaTime;

		}
		

	}
}
