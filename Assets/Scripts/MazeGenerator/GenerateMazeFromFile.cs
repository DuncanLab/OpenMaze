using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using data;
using UnityEditor;
using UnityEngine;
using wallSystem;
using Debug = UnityEngine.Debug;
using DS = data.DataSingleton;
using L = main.Loader;
public class GenerateMazeFromFile : MonoBehaviour {

		
	// Use this for initialization
	private void Start ()
	{
		var m = L.Get().CurrTrial.Value.Map;
		var y = m.TopLeft[1];
		

		foreach (var row in m.Map)
		{
			var x = m.TopLeft[0];
			
			foreach (var col in row.ToCharArray())
			{
				if (col == 'w')
				{
					var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
					obj.GetComponent<Renderer>().sharedMaterial.color = Data.GetColour(m.Color);
					obj.transform.localScale = new Vector3(m.TileWidth, DS.GetData().WallHeight, m.TileWidth);
					obj.transform.position = new Vector3(x, DS.GetData().WallHeight * 0.5f, y);
					
				}

				else if (col != '0')
				{
					var val = col - '0'; 
					var item = DS.GetData ().PickupItems [val - 1];
					var prefab = (GameObject)Resources.Load("prefabs/" + item.PrefabName, typeof(GameObject));
					
					var obj = Instantiate (prefab);
					if (!item.PrefabName.Equals("2DImageDisplayer"))
						obj.AddComponent<RotateBlock>();
			
					obj.transform.localScale *= item.Size;
					obj.transform.position = new Vector3 (x, 0.5f, y);
			
				}
				
				x += m.TileWidth;

			}

			y -= m.TileWidth;

		}
		
		
	}
}
