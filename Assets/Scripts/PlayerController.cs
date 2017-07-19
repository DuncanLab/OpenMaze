using System;
using UnityEngine;
using System.IO;
using DS = DataSingleton;
using L = Loader;
using Random = UnityEngine.Random;

//This class is the primary player script.
//This allows us to move around essentially.
public class PlayerController : MonoBehaviour {
   
	public Camera Cam;


    //This is essentially an enumeration to choose between two possible states.
    public enum State
    {
        Waiting,
        Moving
    }

    //This is the current state
	//This essentially contrals the c
    private State _state;

    private GenerateGenerateWall _gen;
    

    //This is the current running time of the game
    private float _runningTime;

    //And this counter keeps track of how long he delay has until it's over
    private float _currDelay;
    
    //The stream writer that writes data out to an output file.
	private string _outDir;



    //This is the character controller system used for collision
    private CharacterController _controller;

    //The initial move direction is static zero.
	private Vector3 _moveDirection = Vector3.zero;

	private float _iniRotation;

	private void Start()
    {
		_runningTime = 0;
		_iniRotation = Random.Range (0, 360);
		transform.Rotate (new Vector3 (0, _iniRotation, 0));
	    _controller = GetComponent<CharacterController>();
        _state = State.Waiting;
        _gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();
		Directory.CreateDirectory ("Assets\\OutputFiles~");
		Cam.transform.Rotate (-DS.GetData().CharacterData.CamRotation, 0, 0);
    }

    private void LogData(bool collided)
    {
	    if (!L.experimentMode) return;
	    using (var writer = new StreamWriter ("Assets\\OutputFiles~\\" + DS.GetData ().CharacterData.OutputFile, true)) {
		    string line = (Loader.experimentIndex + 1) + ", "
		                  + _runningTime + ", "
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
        //Here this finds the given pickup
	    if (!other.gameObject.CompareTag("Pickup") || _state != State.Moving) return;
	    //We log the data out to the console

	    GetComponent<AudioSource> ().PlayOneShot (_gen.GetWaveSrc (), 1);
	    Destroy (other.gameObject);

	    if (Loader.experimentMode) {
		    LogData(true);
		    GetComponent<AudioSource>().PlayOneShot(_gen.GetWaveSrc(), 1);
		    Loader.progressExperiment ();
		    Destroy (this);

	    } else {
		    _gen.NewZoneRandom ();
		    //We reset the position
		    transform.position = new Vector3(0, 0.2f, 0);

		    //As well as the rotation
		    transform.rotation = Quaternion.identity ;
		    transform.Rotate(new Vector3(0, 90, 0));	

	    }
	    _state = State.Waiting;
	    _currDelay = 0;
    }

	private void LateUpdate(){
		if (_state != State.Waiting)
			LogData(false);

	}

	private void Update()
    {
        int waitTime = DS.GetData().CharacterData.Delay;
        //In each update loop, we have we begin by checking if they have indeed been
        //by the appropriate amount of time.
        if (_currDelay > waitTime)
        {
            if (_state != State.Moving)
            {
                _state = State.Moving;
                _gen.Timer.text = "";
				Cam.transform.Rotate (DS.GetData().CharacterData.CamRotation, 0, 0);

            }
        } else
        {
			if (_state != State.Moving) {
				float angle = 360f * _currDelay / waitTime + _iniRotation - transform.rotation.eulerAngles.y;


				transform.Rotate (new Vector3 (0, angle, 0));
				_gen.Timer.text = "";

			}
            //This section modifies the colors of the text value
//            float val = (waitTime - currDelay);
//            int actValue = (int)(waitTime - currDelay);
//            float diff = val - actValue;
//            gen.timer.text = (actValue + 1) + "";
//            gen.timer.color = new Color(0, 0, 0, diff);
//            
        	
		
		}

        //This calculates the current amount of rotation frame rate independent
        float rotation = Input.GetAxis("Horizontal") * DS.GetData().CharacterData.RotationSpeed * Time.deltaTime;

        
        //This calculates the forward speed frame rate independent
        _moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
        _moveDirection = transform.TransformDirection(_moveDirection);
        _moveDirection *= DS.GetData().CharacterData.MovementSpeed;

        //Here is the movement system
	    const double tolerance = 0.0001;
	    if (_state == State.Moving)
        {
            //we move iff rotation is 0
	        if (Math.Abs(Mathf.Abs(rotation)) < tolerance)
                _controller.Move(_moveDirection * Time.deltaTime);

            transform.Rotate(0, rotation, 0);
        }

        //And we increment both delay systems
        _runningTime += Time.deltaTime;
        _currDelay += Time.deltaTime;
        //And we log the current data.


    }


}
