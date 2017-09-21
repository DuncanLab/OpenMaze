using UnityEngine;

namespace twoDSystem
{
	public class LineDrawer : MonoBehaviour {
		private void Start()
		{
			var points = WallPointContainer.GetPoints();
			foreach (var i in points)
			{
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.GetComponent<Renderer>().material.color = Color.black;
				cube.transform.position = new Vector3(i.x, 0, i.z);
				cube.transform.Rotate(new Vector3(0, i.y, 0));
				cube.transform.localScale = new Vector3(WallPointContainer.Length, 1f, 1);
			}

			foreach (var i in WallPointContainer.Pillars)
			{
				GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				cylinder.GetComponent<Renderer>().material.color = Color.red;
				cylinder.transform.position = new Vector3(i.x, 0, i.z);
				cylinder.transform.localScale = new Vector3(i.y, 1f, i.y);


			}

		}
	}
}
