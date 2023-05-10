using System;
using System.Collections.Generic;
using System.Diagnostics;
using Source.Models;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Source.Controllers
{
    public class AirplanesController : MonoBehaviour
    {
        public GameObject prefabAirplane;
        public GameObject canvasScreen;
        public static HashSet<Airplane> Airplanes = new();
        public Random rnd = new();
        public int coolDown = 2;
        
        private Stopwatch _fromNewPlane = new Stopwatch();

        private void Awake()
        {
            _fromNewPlane.Start();
        }

        void Update()
        {
            var needDelete = new HashSet<Airplane>();
            foreach (var a in Airplanes)
            {
                a.Update();
                var res = !a.Alive();
                if (res)
                {
                    needDelete.Add(a);
                }
            }

            foreach (var a in needDelete)
            {
                Airplanes.Remove(a);
                Destroy(a.gameObject);
            }
        
            if (Airplanes.Count < 5 && _fromNewPlane.Elapsed > TimeSpan.FromSeconds(coolDown))
            {
                NewRandomPlane();
                _fromNewPlane.Restart();
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
                new(rnd.Next(0, 1920), rnd.Next(0, 1080)),
            };
        }
    }
}