using System.Collections.Generic;
using UnityEngine;

namespace Source.Models
{
    public class PathLine
    {
        public LineRenderer LineRenderer { get; }

        public PathLine(LineRenderer lineRendererRenderer, Color color, float width)
        {
            LineRenderer = lineRendererRenderer;
            LineRenderer.startWidth = width;
            LineRenderer.endWidth = width;
            LineRenderer.startColor = color;
            LineRenderer.endColor = color;
            LineRenderer.enabled = true;
        }

        public void UpdatePosition(List<Vector3> points)
        {
            LineRenderer.positionCount = points.Count;
            for (var i = 0; i < points.Count; i++)
                points[i] = new Vector3(points[i].x, points[i].y, -10);

            LineRenderer.SetPositions(points.ToArray());
        }
    }
}