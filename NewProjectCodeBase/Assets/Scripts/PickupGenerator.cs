using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupGenerator : MonoBehaviour {
    List<GameObject> destroy;
    public GameObject pickup;
    public Text goalText;
    public Data data;

    //This function supposedly generates a random normal number with given mu sd
    //This is done using the Marsaglia Polar Method
    //https://en.wikipedia.org/wiki/Marsaglia_polar_method
    private static float RandomNormalValue(float mu, float sd)
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



	// Use this for initialization
	void Start () {
        GenerateGenerateWall gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();
        data = gen.globalData;

        destroy = new List<GameObject>(); //This initializes the food object destroy list

        //SETUP SEED SYSTEM HERE (probably initialize this with number of walls).
        Random.InitState(data.WallData.Sides);

        Data.PickupItem pickup = data.PickupItems[(int)Random.Range(0, data.PickupItems.Count)];
        gen.SetWaveSrc(pickup.SoundLocation);
        
        //Here is the text to determine the type of food that exists here
        goalText = GameObject.Find("Goal").GetComponent<Text>();

        //And this section sets the text.
        goalText.text = pickup.Tag;



        //And we spawn 50 targets. (ADD THIS AS A DYNAMIC FIELD).
        for (int i = 0; i < pickup.Count; i++)
        {
            GameObject obj = Instantiate(this.pickup);
            
            //This is the random value with mu centred at x, z
            float x = RandomNormalValue(
                pickup.Distribution.parameters[0],
                pickup.Distribution.parameters[1]) + pickup.GeneratorPos.x;
            float y = RandomNormalValue(
                pickup.Distribution.parameters[0],
                pickup.Distribution.parameters[1]) + pickup.GeneratorPos.y;


            obj.transform.position = new Vector3(x, 0.5f, y);
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            obj.GetComponent<Renderer>().material.color = Data.GetColour(pickup.Color);
            destroy.Add(obj);
        }
    }

    //And here we destroy all the food.
    private void OnDestroy()
    {
        for (int i = 0; i < destroy.Count; i++)
        {
            if (destroy[i] != null) Destroy(destroy[i]);
        }
    }
    

}
