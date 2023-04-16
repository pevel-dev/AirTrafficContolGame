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

    void Start()
    {
        foreach (var airplane in AirplaneController.Airplanes)
        {
            radarPoints[airplane.id] = new VisualizeAirplane(CreateNewRadarPoint(airplane));
        }
    }

    void Update()
    {
        var presentIds = new HashSet<int>();

        foreach (var airplane in AirplaneController.Airplanes)
        {
            presentIds.Add(airplane.id);
            if (radarPoints.TryGetValue(airplane.id, out var point))
                point.PointAirplane.transform.localPosition = airplane.currentPosition;
            else
                radarPoints[airplane.id] = new VisualizeAirplane(CreateNewRadarPoint(airplane));
        }

        //RemoveOldPoints(presentIds);        
    }

    private GameObject CreateNewRadarPoint(Airplane airplane)
    {
        var newPoint = Instantiate(prefabPoint, me.transform);
        newPoint.transform.localPosition = me.transform.TransformPoint(airplane.currentPosition);
        return newPoint;
    }
    
    

    // private void RemoveOldPoints(HashSet<int> presentIds)
    // {
    //     foreach (var key in radarPoints.Keys.Where(key => !presentIds.Contains(key)))
    //     {
    //         Destroy(radarPoints[key]);
    //         radarPoints.Remove(key);
    //     }
    // }
}