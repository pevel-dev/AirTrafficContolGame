using System.Collections.Generic;
using UnityEngine;

namespace Source.Controllers
{
    public class VisualizeAirplane
    {
        public GameObject PointAirplane;
        public LineRenderer LineRenderer;

        public VisualizeAirplane(LineRenderer lineRendererRenderer)
        {
            LineRenderer = lineRendererRenderer;
            LineRenderer.startWidth = 3f;
            LineRenderer.endWidth = 3f;
            LineRenderer.enabled = true;
            LineRenderer.startColor = Color.blue;
            LineRenderer.endColor = Color.blue;
        }

        public void UpdatePosition(List<Vector3> points)
        {
            LineRenderer.positionCount = points.Count;
            for (var i = 0; i < points.Count; i++)
            {
                points[i] = new Vector3(points[i].x, points[i].y, -10);
            }

            LineRenderer.SetPositions(points.ToArray());
        }
    }
}