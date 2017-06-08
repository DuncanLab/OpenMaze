using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupGenerator : MonoBehaviour {
    List<GameObject> destroy;
    public GameObject pickup;
    private GenData genType;
    public Text goalText;
    private float RandomNormalValue(float mu, float sd)
    {
        float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        return v1 * s * sd + mu;
    }

    class GenData
    {
        public string GenType { get; set; }
        public Color Color { get; set; }
        public int type;       
    }


    private static GenData[] arr = {
        new GenData(){
            GenType = "PickupFood",
            Color = Color.yellow,
            type = 0
        },
        new GenData(){
            GenType = "PickupMoney",
            Color = Color.green,
            type = 1

        },
        new GenData(){
            GenType = "PickupWater",
            Color = Color.blue,
            type = 2
        }
    };

	// Use this for initialization
	void Start () {
        destroy = new List<GameObject>();
        genType = arr[(int)Random.Range(0, 3)];
        goalText = GameObject.Find("Goal").GetComponent<Text>();
        goalText.text = genType.GenType.Substring(6);
        float val1 = genType.type == 0 ? -1f : 1f;
        float val2 = genType.type == 1 ? -1f : 1f;

        for (int i = 0; i < 50; i++)
        {
            GameObject obj = Instantiate(pickup);
            float x = RandomNormalValue(transform.position.x * val1, 1);
            float y = RandomNormalValue(transform.position.z * val2, 1);
            obj.transform.position = new Vector3(x, 0.5f, y);
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            obj.tag = genType.GenType;
            obj.GetComponent<Renderer>().material.color = genType.Color;
            destroy.Add(obj);
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject obj in destroy)
        {
            Destroy(obj);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
