using System;
using Source.Models;
using TMPro;
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

        [SerializeField] [Header("Объекты жизней")]
        private GameObject[] healsObjects;

        [SerializeField] [Header("Спрайт пустого сердечка")]
        private Sprite emptyHeal;

        [SerializeField] [Header("Спрайт полного сердечка")]
        private Sprite fullHeal;

        [SerializeField] [Header("Объект текста для монет")]
        private TMP_Text moneyText;

        [SerializeField] [Header("Объект текста для очков")]
        private TMP_Text pointText;


        private MoneyController _moneyController;
        private AirplanesController _airplanesController;
        private int _heal;
        private int _points;
        private int _money;

        public void AddPoints(AirplaneTypes airplaneType)
        {
            _points += (int)airplaneType;
            UpdateText();
        }

        public void AddHeals()
        {
            if (_heal < 5)
            {
                healsObjects[_heal].GetComponent<SpriteRenderer>().sprite = fullHeal;
                _heal++;
            }
        }

        public void EndGame()
        {
            _heal = -1;
        }

        public void AirplaneKilled()
        {
            _airplanesController.KilledAirplane();
            _heal--;
            if (_heal >= 0)
                healsObjects[_heal].GetComponent<SpriteRenderer>().sprite = emptyHeal;
        }

        public void CollectedMoney()
        {
            _money++;
            _moneyController.KilledMoney();
            UpdateText();
        }

        public void AirplaneDown()
        {
            _airplanesController.KilledAirplane();
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

            UpdateText();
        }

        private void Update()
        {
            if (_heal <= 0)
            {
                SaveAllToDisk();
                SceneManager.LoadScene(gameOverSceneName);
            }
        }

        private void SaveAllToDisk()
        {
            PlayerPrefs.SetInt("lastResult", _points);
            PlayerPrefs.SetInt("money", _money);
            PlayerPrefs.Save();
        }

        private void UpdateText()
        {
            moneyText.text = _money.ToString();
            pointText.text = _points.ToString();
        }
        
    }
}