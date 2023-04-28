﻿using System.Collections.Generic;
using Source.Models;
using UnityEngine;
using Random = System.Random;

namespace Source.Controllers
{
    public class AirplanesController : MonoBehaviour
    {
        public GameObject prefabAirplane;
        public GameObject canvasScreen;
        public static List<Airplane> Airplanes = new();
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
            var path = GetRandomStartEnd();
            var airplane = Instantiate(prefabAirplane, path[0], Quaternion.identity).GetComponent<Airplane>();
            airplane.parentPrefabPoints = canvasScreen;
            airplane.transform.SetParent(canvasScreen.transform);
            airplane.LoadPath(path);
            Airplanes.Add(airplane);
        }

        private List<Vector3> GetRandomStartEnd()
        {
            return new List<Vector3>
            {
                new(30, rnd.Next(0, 1080)),
                new(rnd.Next(0, 1920), rnd.Next(0,1080)),
                new(rnd.Next(0, 1920), rnd.Next(0,1080)),
                new(rnd.Next(100, 750), rnd.Next(0, 1080))
            };
        }
    }
}