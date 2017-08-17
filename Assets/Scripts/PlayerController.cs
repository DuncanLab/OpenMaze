using System;
using UnityEngine;
using System.IO;
using DS = DataSingleton;
using Random = UnityEngine.Random;
using E = ExperimentManager;
//This class is the primary player script.
//This allows us to move around essentially.
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

	private void Start()
	{
		_currDelay = 0;
		E.Get().CatchEvent(E.State.Waiting);
	    _iniRotation = Random.Range (0, 360);
		transform.Rotate (new Vector3 (0, _iniRotation, 0));
	    _controller = GetComponent<CharacterController>();
        _gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();
		Cam.transform.Rotate (-DS.GetData().CharacterData.CamRotation, 0, 0);
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

//	    GetComponent<AudioSource> ().PlayOneShot (_gen.GetWaveSrc (), 1);
//	    Destroy (other.gameObject);

		LogData(true);
		GetComponent<AudioSource>().PlayOneShot(_gen.GetWaveSrc(), 1);
	    E.Get().CatchEvent(E.State.PlayingSound);

    }

	private void LateUpdate(){
		if (E.Get().St != E.State.Waiting)
			LogData(false);

	}

	private void Update()
    {

	    if (E.Get().St == E.State.PlayingSound)
	    {
		    if (!GetComponent<AudioSource>().isPlaying)
		    {
			    E.Get().CatchEvent(E.State.DonePlaying);
		    }

	    }
	    else if (E.Get().St == E.State.Waiting)
	    {
		    var waitTime = DS.GetData().CharacterData.TimeToRotate;
		    //In each update loop, we have we begin by checking if they have indeed been
		    //by the appropriate amount of time.
		    if (_currDelay > waitTime)
		    {
			    if (E.Get().St == E.State.Moving) return;
			    E.Get().CatchEvent(E.State.Moving);
			    _gen.Timer.text = "";
			    Cam.transform.Rotate(DS.GetData().CharacterData.CamRotation, 0, 0);
		    }
		    else
		    {

			    float angle = 360f * _currDelay / waitTime + _iniRotation - transform.rotation.eulerAngles.y;

			    transform.Rotate(new Vector3(0, angle, 0));
			    _gen.Timer.text = "";
		    }
	    }
	    else //E.State should be MOVING here.
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

		    _currDelay += Time.deltaTime;

	    }
    }


}
