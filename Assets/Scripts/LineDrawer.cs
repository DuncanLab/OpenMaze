using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour {
	private void Start()
	{
		var points = WallPointContainer.GetPoints();
		points.Add(points[0]);	
		
		var lr = GetComponent<LineRenderer>();
		lr.startColor = Color.black;
		lr.endColor = Color.black;
		lr.positionCount = points.Count;
		
		Debug.Log(points.ToArray().Length);
		
		lr.SetPositions(points.ToArray());

		
		
	}
}
