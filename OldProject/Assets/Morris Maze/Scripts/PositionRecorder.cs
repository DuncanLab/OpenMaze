using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PositionRecorder : MonoBehaviour {

	string FILE_NAME = "positionFile.csv";
    int time = 0;
    public int interval = 1;
	float init_x;
	float init_z;
	float init_time;
    // Use this for initialization
    void Start () {
		init_x = transform.position.x;
		init_z = transform.position.z;
		init_time = Time.time;
		StreamWriter sw = File.AppendText(FILE_NAME);
		sw.WriteLine ("time(sec),x,z");
		sw.Close ();
    }
   
    // Update is called once per frame
    void Update () {
	   
	    if (time == interval)
        {
            float x = transform.position.x;
            float z = transform.position.z;
//            float fx = transform.forward.x;
//            float fz = transform.forward.z;
            StreamWriter sw = File.AppendText(FILE_NAME);
			sw.WriteLine ((Time.time - init_time) + "," + (x - init_x) + "," + (z - init_z));
//            sw.WriteLine ("my x position is " + x + " my z position is " + z);
//            sw.WriteLine ("I'm facing " + transform.forward);
            sw.Close ();
//            Debug.Log ("write to file");
            time = 0;
        }
	    else
            time ++;
	}
}
