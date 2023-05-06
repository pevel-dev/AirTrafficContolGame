using System.Collections.Generic;
using Source.Models;
using UnityEngine;
using Random = System.Random;

namespace Source.Controllers
{
    public class AirplanesController : MonoBehaviour
    {
        public GameObject prefabAirplane;
        public GameObject canvasScreen;
        public static HashSet<Airplane> Airplanes = new();
        public Random rnd = new();

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

            foreach (var airplane1 in Airplanes)
            {
                foreach (var airplane2 in Airplanes)
                {
                    if (airplane1 == airplane2)
                        continue;
                    var diff = airplane1.transform.position - airplane2.transform.position;
                    if (diff.magnitude < 50)
                    {
                        airplane1.Trigger(airplane2);
                        airplane2.Trigger(airplane1);
                    }
                }
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
                new(rnd.Next(0, 1920), rnd.Next(0, 1080)),
                new(rnd.Next(100, 750), rnd.Next(0, 1080))
            };
        }
    }
}