
using System.Collections.Generic;
using UnityEngine;

public class WallPointContainer
{

    private static List<Vector3> _points;


    public static void Reset()
    {
        if (_points == null) _points = new List<Vector3>();
        _points.Clear();
    }
    
    public static void Add(Data.Point p)
    {        
        
        
        
        _points.Add(p.GetVector3());
    }


    public static List<Vector3> GetPoints()
    {
        return _points;

    }

}
