using System.Collections;
using System.Collections.Generic;
using Source.Models;
using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public static List<Airplane> Airplanes = new();
    public int index = 0;

    void Update()   
    {
        foreach (var a in Airplanes)
        {
            a.Update();
        }
    }

    public void NewRandomPlane()
    {
        var path = new Vector3[] { new(0, 0, 5), new(100, 100, 5), new(200, 200, 5)};
        Airplanes.Add(new Airplane(index, 0, 100, 0.5f, path));
        index++;
    }
}