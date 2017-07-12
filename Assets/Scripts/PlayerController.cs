using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DS = DataSingleton;

//This class is the primary player script.
//This allows us to move around essentially.
public class PlayerController : MonoBehaviour {
    
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
    private StreamWriter writer;



    //This is the character controller system used for collision
    private CharacterController controller;

    //The initial move direction is static zero.
    private Vector3 moveDirection = Vector3.zero;


    void Start()
    {
    
		runningTime = 0;
        controller = GetComponent<CharacterController>();
        state = State.WAITING;
        gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();
		System.IO.Directory.CreateDirectory ("Assets\\OutputFiles~");

		writer = new StreamWriter("Assets\\OutputFiles~\\" + DS.GetData().CharacterData.OutputFile, false);


		writer.WriteLine (DS.GetData().EnvironmentType + ", ", DS.GetData().WallData.EndColour + ", " + DS.GetData().WallData.Sides);
        writer.WriteLine("time (seconds), x, y, target, angle");
    }

    private void LogData(bool collided)
    {
      
        string line = runningTime + ", " + transform.position.x + ", " + transform.position.z + ", " + collided + ", " + transform.rotation.eulerAngles.y;
        if (writer != null)
        writer.WriteLine(line);
        
    }

    //This is the collision system.
    void OnTriggerEnter(Collider other)
    {
        //Here this finds the given pickup
        if (other.gameObject.CompareTag("Pickup") && this.state == State.MOVING)
        {
            //We log the data out to the console
            LogData(true);

			if (Loader.experimentMode) {
				Loader.progressExperiment ();
			} else {
				gen.NewZoneRandom ();
				//We reset the position
				transform.position = new Vector3(0, 0.2f, 0);

				//As well as the rotation
				transform.rotation = Quaternion.identity ;
				transform.Rotate(new Vector3(0, 90, 0));

			}


            //And then we begin the waiting game.
            state = State.WAITING;
            currDelay = 0;
            GetComponent<AudioSource>().PlayOneShot(gen.GetWaveSrc(), 1);
            Destroy(other.gameObject);

        }

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
            }
        } else
        {
            //This section modifies the colors of the text value
            float val = (waitTime - currDelay);
            int actValue = (int)(waitTime - currDelay);
            float diff = val - actValue;
            gen.timer.text = (actValue + 1) + "";
            gen.timer.color = new Color(0, 0, 0, diff);
            
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
        LogData(false);


    }

    //Here we flush the stream writer and close it.
    void OnDestroy()
    {
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
        }
    }

}
