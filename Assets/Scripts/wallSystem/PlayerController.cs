using System;
using System.Collections.Generic;
using trial;
using UnityEngine;
using UnityEngine.UI;
using DS = data.DataSingleton;
using E = main.Loader;
using Random = UnityEngine.Random;
//This class is the primary player script.
//This allows us to move around essentially.
namespace wallSystem
{
    public class PlayerController : MonoBehaviour
    {
        public Camera Cam;
        private GenerateGenerateWall _gen;

        //The stream writer that writes data out to an output file.
        private readonly string _outDir;

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
            try
            {
                var goalText = GameObject.Find("Goal").GetComponent<Text>();
                goalText.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 40);

                //And this section sets the text.
                goalText.text = E.Get().CurrTrial.Value.Header;
                goalText.color = Color.white;


            }
            catch (NullReferenceException e)
            {
                Debug.LogWarning("Goal object not set: running an instructional trial");
            }

            Random.InitState(DateTime.Now.Millisecond);

            _currDelay = 0;

            // Choose a random starting angle if the value is not set in config
            if (E.Get().CurrTrial.Value.StartFacing == -1)
            {
                _iniRotation = Random.Range(0, 360);
            }
            else
            {
                _iniRotation = E.Get().CurrTrial.Value.StartFacing;
            }

            transform.Rotate(0, _iniRotation, 0, Space.World);

            try
            {
                _controller = GetComponent<CharacterController>();
                _gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();
                Cam.transform.Rotate(0, 0, 0);
            }
            catch (NullReferenceException e)
            {
                Debug.LogWarning("Can't set controller object: running an instructional trial");
            }
            _waitTime = E.Get().CurrTrial.Value.Rotate;
            _reset = false;
            localQuota = E.Get().CurrTrial.Value.Quota;

        }

        //Start the character. //If init from maze, this allows "s" to determine the start position
        public void ExternalStart(float pickX, float pickY, bool maze = false)
        {
            TrialProgress.GetCurrTrial().TrialProgress.TargetX = pickX;
            TrialProgress.GetCurrTrial().TrialProgress.TargetY = pickY;

            // No start pos specified so make it random.
            if (E.Get().CurrTrial.Value.StartPosition.Count == 0)
            {
                // Try to randomly place the character, checking for proximity
                // to the pickup location
                var i = 0;
                while (i++ < 100)
                {
                    var CurrentTrialRadius = DS.GetData().MazesDictionary[E.Get().CurrTrial.TrialProgress.CurrentMazeName].Radius;
                    var v = Random.insideUnitCircle * CurrentTrialRadius * 0.9f;
                    var mag = Vector3.Distance(v, new Vector2(pickX, pickY));
                    if (mag > DS.GetData().CharacterData.DistancePickup)
                    {
                        transform.position = new Vector3(v.x, 0.5f, v.y);
                        var camPos = Cam.transform.position;
                        camPos.y = DS.GetData().CharacterData.Height;
                        Cam.transform.position = camPos;
                        return;
                    }
                }
                Debug.LogError("Could not randomly place player. Probably due to" +
                               " a pick up location setting");
            }
            else
            {
                var p = E.Get().CurrTrial.Value.StartPosition;
                if (maze)
                    p = new List<float>() { pickX, pickY };
                transform.position = new Vector3(p[0], 0.5f, p[1]);
                var camPos = Cam.transform.position;
                camPos.y = DS.GetData().CharacterData.Height;
                Cam.transform.position = camPos;
            }
        }

        // This is the collision system.
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Pickup")) return;

            GetComponent<AudioSource>().PlayOneShot(other.gameObject.GetComponent<PickupSound>().Sound, 10);
            Destroy(other.gameObject);

            // Tally the number collected per current block
            int BlockID = TrialProgress.GetCurrTrial().BlockID;
            TrialProgress.GetCurrTrial().TrialProgress.NumCollectedPerBlock[BlockID]++;

            TrialProgress.GetCurrTrial().NumCollected++;
            E.LogData(
                TrialProgress.GetCurrTrial().TrialProgress,
                TrialProgress.GetCurrTrial().TrialStartTime,
                transform,
                1
            );

            if (--localQuota > 0) return;

            E.Get().CurrTrial.Notify();

            _playingSound = true;
            E.LogData(
                TrialProgress.GetCurrTrial().TrialProgress,
                TrialProgress.GetCurrTrial().TrialStartTime,
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
            // This first block is for the initial rotation of the character
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
                    Cam.transform.Rotate(0, 0, 0);
                    _reset = true;
                    TrialProgress.GetCurrTrial().ResetTime();
                }

                //Move the character.
                try
                {
                    ComputeMovement();
                }
                catch (MissingComponentException e)
                {
                    Debug.LogWarning("Skipping movement calc: instructional trial");
                }

                E.LogData(TrialProgress.GetCurrTrial().TrialProgress, TrialProgress.GetCurrTrial().TrialStartTime, transform);
            }
            _currDelay += Time.deltaTime;
        }


    }
}
