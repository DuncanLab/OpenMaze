using System;
using data;
using main;
using UnityEngine;
using UnityEngine.UI;
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

		private bool _playingSound;
		
		private bool _reset;

		private bool _log;
		
		private void Start()
		{
			Random.InitState(DateTime.Now.Millisecond);

			_currDelay = 0;
			_iniRotation = Random.Range (0, 360);
			transform.Rotate (new Vector3 (0, _iniRotation, 0));
			_controller = GetComponent<CharacterController>();
			_gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();
			Cam.transform.Rotate (-DS.GetData().CharacterData.CamRotation, 0, 0);
			_waitTime = DS.GetData().CharacterData.TimeToRotate;
			_reset = false;
			_log = false;

		}


		public void ExternalStart()
		{
			if (E.Get().CurrTrial.Value.RandomLoc == 1)
			{
				while (true)
				{
					var v = Random.insideUnitCircle * E.Get().CurrTrial.Value.Radius * DS.GetData().CharacterData.CharacterBound;
					var mag = v - new Vector2(PickupGenerator.P.X, PickupGenerator.P.Y);
					if (mag.magnitude > DS.GetData().CharacterData.DistancePickup)
					{
						transform.position = new Vector3(v.x, transform.position.y, v.y);
						break;
					}

				}
			}
			else
			{
				var p = DS.GetData().CharacterData.CharacterStartPos;
				transform.position = new Vector3(p.X, transform.position.y, p.Y);
			}
		}
		
		private void LogData(bool collided)
		{
			var v = E.Get().CurrTrial.Value;
			var line = E.Get().CurrTrial.Value.Note + ", "
			              + E.Get().RunningTime + ", "
			              + transform.position.x + ", "
			              + transform.position.z + ", "
			              + transform.rotation.eulerAngles.y + ", "
			              + v.EnvironmentType + ", "
			              + v.Sides + ", "
			              + (collided ? 1 : 0) + ", "
			              + v.PickupType + ", "
			              + PickupGenerator.P.X + ", "
			              + PickupGenerator.P.Y;
			if (_log)
				E.LogData(line);
		}

		//This is the collision system.
		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Pickup")) return;
			BlockState.Found();
			var text = GameObject.Find("CountDown").GetComponent<Text>();
			text.text = "Found: " + BlockState.GetNumberItemsFound();
			GetComponent<AudioSource> ().PlayOneShot (_gen.GetWaveSrc (), 1);
			Destroy (other.gameObject);
			LogData(true);
			_playingSound = true;
			_log = false;

		}

		private void LateUpdate(){
			LogData(false);

		}

		private void ComputeMovement()
		{
			//This calculates the current amount of rotation frame rate independent
			var rotation = Input.GetAxis("Horizontal") * DS.GetData().CharacterData.RotationSpeed * Time.deltaTime;


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

			if (_playingSound)
			{
				if (!GetComponent<AudioSource>().isPlaying)
				{
					E.Get().Progress();
				}
			} 
			else if (_currDelay < _waitTime)
			{
				var angle = 360f * _currDelay / _waitTime + _iniRotation - transform.rotation.eulerAngles.y;
				transform.Rotate(new Vector3(0, angle, 0));
			} 
			else
			{
				if (!_reset)
				{
					E.Get().RunningTime = 0;
					Cam.transform.Rotate (DS.GetData().CharacterData.CamRotation, 0, 0);
					_reset = true;
					_log = true;
					E.LogData("Trial Number, time (seconds), x, y, angle,  " +
					          "EnvironmentType, Sides, targetFound, pickupType, targetX, targetY");
				}
				ComputeMovement();
			}
			_currDelay += Time.deltaTime;

		}
		

	}
}
