using System.Collections;
using System.Collections.Generic;
using Source.Models;
using UnityEngine;
using Random = System.Random;

public class AirplaneController : MonoBehaviour
{
    public static List<Airplane> Airplanes = new();
    public int index = 0;
    public Random rnd = new();

    void Update()   
    {
        foreach (var a in Airplanes)
        {
            a.Update();
        }
    }

    public void NewRandomPlane()
    {
        var countPaths = rnd.Next(0, 10);
        var path = new List<Vector3>();
        for (var i = 0; i < countPaths; i++)
        {
            path.Add(new Vector3(rnd.Next(0, 1920), rnd.Next(0, 1080)));
        }
        Airplanes.Add(new Airplane(index, 0, 100, 0.1f, path));
        index++;
    }
}