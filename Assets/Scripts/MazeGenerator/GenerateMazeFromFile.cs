using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using data;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GenerateMazeFromFile : MonoBehaviour {

	[Serializable]
	private class MazeData
	{
		public List<float> TopLeft;
		public float TileWidth;
		public List<string> Map;
	}
	
	// Use this for initialization
	private void Start ()
	{
		var file = System.IO.File.ReadAllText(Constants.InputDirectory + "maze_test.json");
		var m = JsonUtility.FromJson<MazeData>(file);
		var y = m.TopLeft[1];
		
		var phone = Resources.Load<GameObject>("Prefabs/phone");
		phone.transform.position = new Vector3(0, 1, 0);
		Instantiate(phone);

		foreach (var row in m.Map)
		{
			var x = m.TopLeft[0];
			
			foreach (var col in row.ToCharArray())
			{
				if (col == '1')
				{
					var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
					obj.GetComponent<Renderer>().sharedMaterial.color = Color.red;
					obj.transform.localScale = new Vector3(m.TileWidth, m.TileWidth, m.TileWidth);
					obj.transform.position = new Vector3(x, 0.5f, y);
					
				}

				x += m.TileWidth;

			}

			y -= m.TileWidth;

		}
		
		
	}
}
