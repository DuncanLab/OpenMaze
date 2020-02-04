using data;
using UnityEngine;
using wallSystem;
using Debug = UnityEngine.Debug;

using DS = data.DataSingleton;
using L = main.Loader;

public class GenerateEnclosureFromFile : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        var m = L.Get().CurrTrial.trialData.Map;
        var y = m.TopLeft[1];

        // Goes through each map and initializes it based on stuff.
        foreach (var row in m.Map)
        {
            var x = m.TopLeft[0];

            foreach (var col in row.ToCharArray())
            {
                if (col == 'w')
                {
                    var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    obj.GetComponent<Renderer>().sharedMaterial.color = Data.GetColour(m.Color);
                    obj.transform.localScale = new Vector3(m.TileWidth, L.Get().CurrTrial.enclosure.WallHeight, m.TileWidth);
                    obj.transform.position = new Vector3(x, L.Get().CurrTrial.enclosure.WallHeight * 0.5f, y);
                }
                else if (col == 's')
                {
                    Debug.Log(x + " " + y);
                    GameObject.Find("Participant").GetComponent<PlayerController>().ExternalStart(x, y, true);
                }
                else if (col != '0')
                {
                    // TODO: This code should be shared with PickupGenerator.cs - this class might also just be deadcode.
                    var val = col - '0';
                    var goalItem = DS.GetData().Goals[val - 1];

                    GameObject prefab;
                    GameObject obj;
                    var spriteName = "";

                    if (goalItem.Type.ToLower().Equals("3d"))
                    {
                        prefab = (GameObject)Resources.Load("3D_Objects/" + goalItem.Object, typeof(GameObject));
                        obj = Instantiate(prefab);
                        obj.AddComponent<RotateBlock>();
                        obj.GetComponent<Renderer>().material.color = Data.GetColour(goalItem.Color);
                    }
                    else
                    {
                        // Load the "2D" prefab here, so we have the required components
                        prefab = (GameObject)Resources.Load("3D_Objects/" + goalItem.Type.ToUpper(), typeof(GameObject));
                        obj = Instantiate(prefab);
                        spriteName = goalItem.Object;
                    }

                    obj.transform.localScale = goalItem.ScaleVector;
                    obj.transform.position = new Vector3(x, 0.5f, y);

                    if (!string.IsNullOrEmpty(spriteName))
                    {
                        var pic = Img2Sprite.LoadNewSprite(DataSingleton.GetData().SpritesPath + spriteName);
                        obj.GetComponent<SpriteRenderer>().sprite = pic;
                    }
                }

                x += m.TileWidth;
            }

            y -= m.TileWidth;
        }
    }
}
