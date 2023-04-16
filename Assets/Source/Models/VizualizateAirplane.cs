using System.Collections.Generic;
using UnityEngine;

namespace Source.Models
{
    public class VisualizeAirplane
    {
        public GameObject PointAirplane;
        public List<Vector2> path;

        public VisualizeAirplane(GameObject pointPlane)
        {
            PointAirplane = pointPlane;
            path = new List<Vector2>();
        }
    }
}