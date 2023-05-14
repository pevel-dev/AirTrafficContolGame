using System;
using Source.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Source.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] [Header("Начальное количество жизней")]
        private int startHeals;

        [SerializeField] [Header("Название сцены конца игры")]
        private string gameOverSceneName;

        [SerializeField] [Header("Объект контроллера самолетов")]
        private GameObject airplanesControllerSource;

        private static AirplanesController _airplanesController;
        private static int _heal;
        private static int _points;

        public void Awake()
        {
            _airplanesController = airplanesControllerSource.GetComponent<AirplanesController>();
            _heal = startHeals;
        }

        private void Update()
        {
            if (_heal < 0)
            {
                SceneManager.LoadScene(gameOverSceneName);
            }
        }

        public static void AirplaneKilled()
        {
            _airplanesController.KilledAirplane();
            _heal--;
        }

        public static void AddPoints(AirplaneTypes airplaneType)
        {
            _points += (int)airplaneType;
            _airplanesController.CalculateNewLimit(_points);
        }
    }
}