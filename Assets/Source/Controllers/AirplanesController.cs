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
        [SerializeField] [Header("Префаб самолета")]
        private GameObject prefabAirplane;

        [SerializeField] [Header("Ссылка на экран")]
        private GameObject canvasScreen;

        [SerializeField] [Header("Задержка между спавном самолетов в секундах")]
        private int coolDown;

        [SerializeField] [Header("Начальный лимит спавна самолетов")]
        private int airplaneLimit;

        [SerializeField] [Header("Размер спавна по X")]
        private int spawnX;

        [SerializeField] [Header("Размер спавна по Y")]
        private int spawnY;

        private static int _airplaneCount;
        private readonly Stopwatch _timeFromLastPlane = new();
        private readonly Random _random = new();

        private void Awake()
        {
            _timeFromLastPlane.Start();
        }

        private void Update()
        {
            if (_airplaneCount < airplaneLimit && _timeFromLastPlane.Elapsed > TimeSpan.FromSeconds(coolDown))
            {
                NewRandomPlane();
                _airplaneCount++;
                _timeFromLastPlane.Restart();
            }
        }

        private void NewRandomPlane()
        {
            var path = GetRandomStartEnd();
            var airplane = Instantiate(prefabAirplane, path[0], Quaternion.identity).GetComponent<Airplane>();
            airplane.parentPrefabPoints = canvasScreen;
            airplane.transform.SetParent(canvasScreen.transform);
            airplane.InitializeAirplane(path);
        }

        private List<Vector3> GetRandomStartEnd()
        {
            var randoms = new Vector3[]
            {
                new(0, _random.Next(0, spawnY)),
                new(spawnX, _random.Next(0, spawnY)),
                new(_random.Next(0, spawnX), 0),
                new(_random.Next(0, spawnX), spawnY),
            };
            return new List<Vector3>
            {
                randoms[_random.Next(0, 3)],
                new(_random.Next(0, spawnX), _random.Next(0, spawnY)),
            };
        }

        public void CalculateNewLimit(int points)
        {
            // TODO: Увеличение лимита в зависимости от points
        }

        public void KilledAirplane()
        {
            _airplaneCount--;
        }
    }
}