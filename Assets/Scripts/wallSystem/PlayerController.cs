using System;
using data;
using main;
using trial;
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

		}


		public void ExternalStart(float pickX, float pickY)
		{
			TrialProgress.GetCurrTrial().TrialProgress.TargetX = pickX;
			TrialProgress.GetCurrTrial().TrialProgress.TargetY = pickY;			
			if (E.Get().CurrTrial.Value.RandomLoc == 1)
			{
				while (true)
				{
					var v = Random.insideUnitCircle * E.Get().CurrTrial.Value.Radius * DS.GetData().CharacterData.CharacterBound;
					var mag = Vector3.Distance(v, new Vector2(pickX, pickY));
					if (mag > DS.GetData().CharacterData.DistancePickup)
					{
						transform.position = new Vector3(v.x,DS.GetData().CharacterData.Height, v.y);
						break;
					}

				}
			}
			else
			{
				var p = DS.GetData().CharacterData.CharacterStartPos;
				transform.position = new Vector3(p.X, DS.GetData().CharacterData.Height, p.Y);
			}
		}
		

		//This is the collision system.
		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Pickup")) return;
			E.Get().CurrTrial.Notify();
			GetComponent<AudioSource> ().PlayOneShot (_gen.GetWaveSrc (), 1);
			Destroy (other.gameObject);
			_playingSound = true;
			E.LogData(
				TrialProgress.GetCurrTrial().TrialProgress, 
				TrialProgress.GetCurrTrial().GetRunningTime(),
				transform,
				1
			);

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
	
			 
			if (_currDelay < _waitTime)
			{
				var angle = 360f * _currDelay / _waitTime + _iniRotation - transform.rotation.eulerAngles.y;
				transform.Rotate(new Vector3(0, angle, 0));
			}
			else if (_playingSound)
			{
				if (!GetComponent<AudioSource>().isPlaying)
				{
					//We finish it here
					E.Get().CurrTrial.Progress();
				}
			}
			else
			{
				if (!_reset)
				{
					Cam.transform.Rotate (DS.GetData().CharacterData.CamRotation, 0, 0);
					_reset = true;
					TrialProgress.GetCurrTrial().ResetTime();
				}
				ComputeMovement();
				E.LogData(
					TrialProgress.GetCurrTrial().TrialProgress, 
					TrialProgress.GetCurrTrial().GetRunningTime(),
					transform
				);
			}
			_currDelay += Time.deltaTime;
		}
		

	}
}
