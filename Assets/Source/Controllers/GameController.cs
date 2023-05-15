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
        
        [SerializeField] [Header("Контроллер монеток")]
        protected GameObject moneyController;

        private static MoneyController _moneyController;
        private static AirplanesController _airplanesController;
        private static int _heal;
        private static int _points;
        private static int _money;

        public static void AddPoints(AirplaneTypes airplaneType)
        {
            _points += (int)airplaneType;
        }

        public static void AddHeals()
        {
            _heal += 2;
        }

        public static void EndGame()
        {
            _heal = -1;
        }

        public static void AirplaneKilled()
        {
            _airplanesController.KilledAirplane();
            _heal--;
        }

        public static void CollectedMoney()
        {
            _money += 1;
            _moneyController.KilledMoney();
        }

        private void Awake()
        {
            _airplanesController = airplanesControllerSource.GetComponent<AirplanesController>();
            _moneyController = moneyController.GetComponent<MoneyController>();
            _heal = startHeals;
            if (PlayerPrefs.HasKey("money"))
                _money = PlayerPrefs.GetInt("money");
            else
            {
                _money = 0;
                PlayerPrefs.SetInt("money", 0);
            }
        }

        private void Update()
        {
            if (_heal < 0)
            {
                SaveAllToDisk();
                SceneManager.LoadScene(gameOverSceneName);
            }
        }

        private void SaveAllToDisk()
        {
            if (PlayerPrefs.HasKey("record"))
            {
                var record = PlayerPrefs.GetInt("record");
                if (_points > record) 
                    PlayerPrefs.SetInt("record", _points);
            }
            else
                PlayerPrefs.SetInt("record", _points);
            
            PlayerPrefs.SetInt("money", _money);
            PlayerPrefs.Save();
        }
    }
}