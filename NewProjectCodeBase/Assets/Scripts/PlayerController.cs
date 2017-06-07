using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PlayerController : MonoBehaviour {
    public enum State
    {
        WAITING,
        MOVING
    }
    private State state;

    string path ;

    public static float waitTime = 3f;
    private float runningTime;
    private float currDelay;
    private StreamWriter writer;
    public float speed = 10.0F;
    public float rotationSpeed = 1000.0F;
    CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        state = State.MOVING;
        path = "Assets/OutputFiles/out.txt";
        writer = new StreamWriter(path, false);
        writer.WriteLine("time (seconds), x, y, target");
    }

    private void LogData(bool collided)
    {
        if (state == State.MOVING)
        {
            string line = runningTime + ", " + transform.position.x + ", " + transform.position.z + ", " + collided;
            writer.WriteLine(line);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            LogData(true);

            Destroy(other.gameObject);
            transform.position = new Vector3(0, 0.2f, 0);

            transform.rotation = Quaternion.identity;
            state = State.WAITING;
            currDelay = 0;
        }


    }

    void Update()
    {

        if (currDelay > waitTime) state = State.MOVING;

        

        moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;
        if (state == State.MOVING)
            controller.Move(moveDirection * Time.deltaTime);

        
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        if (state == State.MOVING)
            transform.Rotate(0, rotation, 0);

        runningTime += Time.deltaTime;
        currDelay += Time.deltaTime;

       
        LogData(false);


    }

    void OnDestroy()
    {
        writer.Close();
    }

}
