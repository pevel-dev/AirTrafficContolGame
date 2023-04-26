using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Source.Models;
using UnityEngine;

public class AirScheme : MonoBehaviour
{
    public GameObject prefabPoint;
    public GameObject me;
    public Dictionary<int, VisualizeAirplane> radarPoints = new();

    void Update()
    {
        foreach (var airplane in AirplaneController.Airplanes)
        {
            if (radarPoints.TryGetValue(airplane.id, out var point))
                DrawAirplanesOnScheme(point, airplane);
            else
                radarPoints[airplane.id] = BuildVisualizeAirplane(airplane);
        }
    }

    private static void DrawAirplanesOnScheme(VisualizeAirplane point, Airplane airplane)
    {
        point.PointAirplane.transform.position = airplane.currentPosition;
        var path = airplane.path.ToList();
        path.Insert(0, airplane.nextPoint);
        path.Insert(0, airplane.currentPosition);
        

        point.UpdatePosition(path);
    }

    private VisualizeAirplane BuildVisualizeAirplane(Airplane airplane)
    {
        var newPoint = Instantiate(prefabPoint, me.transform);
        newPoint.transform.position = airplane.currentPosition;
        return new VisualizeAirplane(newPoint,
            newPoint.AddComponent<LineRenderer>());
    }
}