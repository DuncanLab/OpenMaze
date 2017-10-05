using System.Collections.Generic;
using data;
using UnityEngine;

namespace twoDSystem
{
    public class WallPointContainer
    {

        public static Color WallColor;
        public static Color PillarColor;
        
        private static List<Vector3> _points;

        public static List<Vector3> Pillars;
        
        public static float Length;
        public static void Reset()
        {
            if (_points == null) _points = new List<Vector3>();
            if (Pillars == null) Pillars = new List<Vector3>();
            Pillars.Clear();
            
            _points.Clear();
            Length = 0;
        }
    
        public static void Add(Data.Point p, float angle)
        {
            var v = p.GetVector3();
            v.y = angle;
            _points.Add(v);
        }

        public static void AddPillar(Data.Pillar p)
        {
            var v = new Vector3(p.X, p.Radius, p.Y);
            Pillars.Add(v);
        }
        
        public static List<Vector3> GetPoints()
        {
            return _points;

        }

    }
}
