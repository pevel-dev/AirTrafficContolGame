using System;
using System.Collections.Generic;
using System.Diagnostics;
using Source.Models;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Source.Controllers
{
    public class AirplanesController : MonoBehaviour
    {
        [SerializeField] [Header("Префабы самолетов")]
        private GameObject[] prefabAirplane;

        [SerializeField] [Header("Префабы обычных самолетов")]
        private GameObject[] prefabNormalAirplane;

        /// <summary>
        /// Указываются правые границы инетрвалов для каждого объекта.
        /// см. алгоритм генерации
        /// </summary>
        [SerializeField] [Header("Шансы спавна самолетов")]
        private int[] randomWeights;

        [SerializeField] [Header("Ссылка на экран")]
        private GameObject canvasScreen;

        [SerializeField] [Header("Задержка между спавном самолетов в секундах")]
        private int coolDown;

        [SerializeField] [Header("Начальный лимит спавна самолетов")]
        private int startAirplaneLimit;

        [SerializeField] [Header("Лимит спавна самолетов")]
        private AnimationCurve airplaneLimitCurve;

        [SerializeField] [Header("Размер спавна по X")]
        private int spawnX;

        [SerializeField] [Header("Размер спавна по Y")]
        private int spawnY;

        [SerializeField] [Header("Объект gameconroller")]
        private GameObject gameControllerSource;

        private static int _airplaneCount;
        private readonly Stopwatch _timeFromLastPlane = new();
        private readonly Random _random = new();
        private int _currentAirplaneLimit;
        private float _currentTime;
        private int _currentCheckedAirplane;

        private void Awake()
        {
            _currentTime = 0;
            _timeFromLastPlane.Start();
            _airplaneCount = 0;
            _currentCheckedAirplane = 0;
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            _currentAirplaneLimit = (int)(startAirplaneLimit * airplaneLimitCurve.Evaluate(_currentTime));
            if (_airplaneCount < _currentAirplaneLimit && _timeFromLastPlane.Elapsed > TimeSpan.FromSeconds(coolDown))
            {
                NewRandomPlane();
                _airplaneCount++;
                _timeFromLastPlane.Restart();
            }
        }


        private void NewRandomPlane()
        {
            var path = GetRandomStartEnd();

            var randomPlaneValue = RandomAirplanePrefabIndexWithWeight();
            var currentPrefab = randomPlaneValue == 0
                ? prefabNormalAirplane[_currentCheckedAirplane]
                : prefabAirplane[randomPlaneValue];

            var airplane =
                Instantiate(currentPrefab, path[0], Quaternion.identity)
                    .GetComponent<Airplane>();
            airplane.parentPrefabPoints = canvasScreen;
            airplane.transform.SetParent(canvasScreen.transform);
            airplane.InitializeAirplane(path[1], gameControllerSource.GetComponent<GameController>());
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

        private int RandomAirplanePrefabIndexWithWeight()
        {
            var randomNum = _random.Next(0, 100);
            for (var i = 0; i < randomWeights.Length; i++)
            {
                if (i == 0)
                {
                    if (randomNum <= randomWeights[i])
                        return i;
                }
                else
                {
                    if (randomWeights[i - 1] < randomNum && randomNum <= randomWeights[i])
                        return i;
                }
            }

            return 0;
        }

        public void KilledAirplane()
        {
            _airplaneCount--;
        }

        private int FindCurrentAirplane()
        {
            return PlayerPrefs.GetInt("equip", 0);
        }
    }
}