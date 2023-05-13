using System;
using Source.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Controllers
{
    public class GameController : MonoBehaviour
    {
        public int StartHeals;
        public string GameOverSceneName;
        public static int Heal { get; private set; } = 1;
        public static int Points { get; private set; }

        public GameObject AirplanesControllerSource;
        private static AirplanesController _airplanesController;


        public void Awake()
        {
            _airplanesController = AirplanesControllerSource.GetComponent<AirplanesController>();
            Heal = StartHeals;
        }

        void Update()
        {
            Debug.Log(_airplanesController.airplaneLimit);
            if (Heal < 0)
            {
                SceneManager.LoadScene(GameOverSceneName);
            }
        }

        public static void RemoveHeal()
        {
            Heal--;
        }

        public static void AddPoints(AirplaneTypes airplaneType)
        {
            Points += (int)airplaneType;
            _airplanesController.airplaneLimit = (int)Math.Log(Points);
        }
    }
}