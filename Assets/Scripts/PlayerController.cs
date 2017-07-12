using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DS = DataSingleton;
using L = Loader;

//This class is the primary player script.
//This allows us to move around essentially.
public class PlayerController : MonoBehaviour {
    
	public Camera cam;


    //This is essentially an enumeration to choose between two possible states.
    public enum State
    {
        WAITING,
        MOVING
    }

    //This is the current state
	//This essentially contrals the c
    private State state;

    private GenerateGenerateWall gen;
    

    //This is the current running time of the game
    private float runningTime;

    //And this counter keeps track of how long he delay has until it's over
    private float currDelay;
    
    //The stream writer that writes data out to an output file.
	private string outDir;



    //This is the character controller system used for collision
    private CharacterController controller;

    //The initial move direction is static zero.
	private Vector3 moveDirection = Vector3.zero;

	private float iniRotation;
	private float finalRotation;
	private float currRotation;
    void Start()
    {
		runningTime = 0;
		iniRotation = Random.Range (0, 360);
		transform.Rotate (new Vector3 (0, iniRotation, 0));
		finalRotation = iniRotation + 360;
		currRotation = iniRotation;
        controller = GetComponent<CharacterController>();
        state = State.WAITING;
        gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();
		System.IO.Directory.CreateDirectory ("Assets\\OutputFiles~");
		cam.transform.Rotate (-DS.GetData().CharacterData.CamRotation, 0, 0);
    }

    private void LogData(bool collided)
    {
		if (L.experimentMode) {
			using (var writer = new StreamWriter ("Assets\\OutputFiles~\\" + DS.GetData ().CharacterData.OutputFile, true)) {
				string line = (Loader.experimentIndex + 1) + ", "
				              + runningTime + ", "
				              + transform.position.x + ", "
				              + transform.position.z + ", "
				              + collided + ", "
				              + transform.rotation.eulerAngles.y + "\n";
				writer.Write (line);
				writer.Flush ();
			}
		}
    }

    //This is the collision system.
    void OnTriggerEnter(Collider other)
    {
        //Here this finds the given pickup
        if (other.gameObject.CompareTag("Pickup") && this.state == State.MOVING)
        {
            //We log the data out to the console

			GetComponent<AudioSource> ().PlayOneShot (gen.GetWaveSrc (), 1);
			Destroy (other.gameObject);

			if (Loader.experimentMode) {
				LogData(true);
				GetComponent<AudioSource>().PlayOneShot(gen.GetWaveSrc(), 1);
				Loader.progressExperiment ();
				Destroy (this);

			} else {
				gen.NewZoneRandom ();
				//We reset the position
				transform.position = new Vector3(0, 0.2f, 0);

				//As well as the rotation
				transform.rotation = Quaternion.identity ;
				transform.Rotate(new Vector3(0, 90, 0));	

			}
			state = State.WAITING;
			currDelay = 0;


        }

    }

	void LateUpdate(){
		if (state != State.WAITING)
			LogData(false);

	}

    void Update()
    {
        int waitTime = DS.GetData().CharacterData.Delay;
        //In each update loop, we have we begin by checking if they have indeed been
        //by the appropriate amount of time.
        if (currDelay > waitTime)
        {
            if (state != State.MOVING)
            {
                state = State.MOVING;
                gen.timer.text = "";
				cam.transform.Rotate (DS.GetData().CharacterData.CamRotation, 0, 0);

            }
        } else
        {
			if (state != State.MOVING) {
				float angle = 360f * currDelay / waitTime + iniRotation - transform.rotation.eulerAngles.y;


				transform.Rotate (new Vector3 (0, angle, 0));
				gen.timer.text = "";

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
        moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= DS.GetData().CharacterData.MovementSpeed;

        //Here is the movement system
        if (state == State.MOVING)
        {
            //we move iff rotation is 0
            if (Mathf.Abs(rotation) == 0)
                controller.Move(moveDirection * Time.deltaTime);

            transform.Rotate(0, rotation, 0);
        }

        //And we increment both delay systems
        runningTime += Time.deltaTime;
        currDelay += Time.deltaTime;
        //And we log the current data.


    }


}
