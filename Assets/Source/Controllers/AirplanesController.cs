using System;
using System.Collections.Generic;
using System.Diagnostics;
using Source.Models;
using UnityEngine;
using Random = System.Random;

namespace Source.Controllers
{
    public class AirplanesController : MonoBehaviour
    {
        public GameObject prefabAirplane;
        public GameObject canvasScreen;
        private readonly Random _random = new();
        public int coolDown = 2;
        public int airplaneLimit = 5;

        public static int AirplaneCount { get; set; }

        private readonly Stopwatch _timeFromLastPlane = new();

        private void Awake()
        {
            _timeFromLastPlane.Start();
        }

        void Update()
        {
            if (AirplaneCount < airplaneLimit && _timeFromLastPlane.Elapsed > TimeSpan.FromSeconds(coolDown))
            {
                NewRandomPlane();
                AirplaneCount++;
                _timeFromLastPlane.Restart();
            }
        }

        public void NewRandomPlane()
        {
            var path = GetRandomStartEnd();
            var airplane = Instantiate(prefabAirplane, path[0], Quaternion.identity).GetComponent<Airplane>();
            airplane.parentPrefabPoints = canvasScreen;
            airplane.transform.SetParent(canvasScreen.transform);
            airplane.LoadPath(path);
        }


        private List<Vector3> GetRandomStartEnd()
        {
            var randoms = new Vector3[]
            {
                new(10, _random.Next(10, 1070)),
                new(1910, _random.Next(10, 1070)),
                new(_random.Next(10, 1910), 10),
                new(_random.Next(10, 1910), 1070),
            };
            return new List<Vector3>
            {
                randoms[_random.Next(0, 3)],
                new(_random.Next(0, 1920), _random.Next(0, 1080)),
            };
        }
    }
}