using System.Collections.Generic;
using data;
using UnityEditor;
using UnityEngine;

namespace twoDSystem
{
    public class WallPointContainer
    {

        private static List<Vector3> _points;

        public static float Length;
        public static void Reset()
        {
            if (_points == null) _points = new List<Vector3>();
            _points.Clear();
            Length = 0;
        }
    
        public static void Add(Data.Point p, float angle)
        {
            Vector3 v = p.GetVector3();
            v.y = angle;
            _points.Add(v);
        }


        public static List<Vector3> GetPoints()
        {
            return _points;

        }

    }
}
