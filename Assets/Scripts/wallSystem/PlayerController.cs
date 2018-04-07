using System;
using System.Collections.Generic;
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
		private int localQuota;
		
		private void Start()
		{
			Random.InitState(DateTime.Now.Millisecond);

			_currDelay = 0;
			_iniRotation = Random.Range (0, 360);
			
			transform.Rotate (new Vector3 (0, _iniRotation, 0));
			_controller = GetComponent<CharacterController>();
			_gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();
			Cam.transform.Rotate (0, 0, 0);
			_waitTime = E.Get().CurrTrial.Value.TimeToRotate;
			_reset = false;
			localQuota = E.Get().CurrTrial.Value.Quota;

		}

		//Start the character. //If init from maze, this allows "s" to determine the start position
		public void ExternalStart(float pickX, float pickY, bool maze = false)
		{
			TrialProgress.GetCurrTrial().TrialProgress.TargetX = pickX;
			TrialProgress.GetCurrTrial().TrialProgress.TargetY = pickY;			
			if (E.Get().CurrTrial.Value.RandomLoc == 1)
			{
				var i = 0;
				while (i++ < 100)
				{
					var v = Random.insideUnitCircle * E.Get().CurrTrial.Value.Radius * 0.9f;
					var mag = Vector3.Distance(v, new Vector2(pickX, pickY));
					if (mag > DS.GetData().CharacterData.DistancePickup)
					{
						transform.position = new Vector3(v.x,DS.GetData().CharacterData.Height, v.y);
						break;
					}

				}
				Debug.LogError("This is exploed.");
				transform.position = new Vector3(0,DS.GetData().CharacterData.Height, 0);

			}
			else
			{
				var p = E.Get().CurrTrial.Value.CharacterStartPos;
				if (maze)
					p = new List<float>() {pickX, pickY};
				transform.position = new Vector3(p[0], 0.5f, p[1]);
				var v = Cam.transform.position;
				v.y = DS.GetData().CharacterData.Height;
				Cam.transform.position = v;
			}
		}
		

		//This is the collision system.
		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Pickup")) return;

			
			GetComponent<AudioSource> ().PlayOneShot (other.gameObject.GetComponent<PickupSound>().Sound, 10);
			Destroy (other.gameObject);
			if (--localQuota > 0) return;
			E.Get().CurrTrial.Notify();

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
	
			 //This first block is for the initial rotation of the character
			if (_currDelay < _waitTime)
			{
				var angle = 360f * _currDelay / _waitTime + _iniRotation - transform.rotation.eulerAngles.y;
				transform.Rotate(new Vector3(0, angle, 0));
			}
			//We need to not continue if there is audio playing, so we just pause here.
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
				//This section rotates the camera (potentiall up 15 degrees), basically deprecated code.
				if (!_reset)
				{
					Cam.transform.Rotate (0, 0, 0);
					_reset = true;
					TrialProgress.GetCurrTrial().ResetTime();
				}
				
				//Move the character.
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
