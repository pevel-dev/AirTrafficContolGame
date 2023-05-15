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
    public class MoneyController : MonoBehaviour
    {
        [SerializeField] [Header("Префаб монетки")]
        private GameObject prefabMoney;
        

        [SerializeField] [Header("Задержка между спавном монеток в секундах")]
        private int coolDown;

        [SerializeField] [Header("Начальный лимит спавна монеток")]
        private int startMoneyLimit;
        
        [SerializeField] [Header("Лимит спавна монеток")]
        private AnimationCurve moneyLimitCurve;

        [SerializeField] [Header("Размер спавна по X")]
        private int spawnX;

        [SerializeField] [Header("Размер спавна по Y")]
        private int spawnY;

        private int _moneyCount;
        private readonly Stopwatch _timeFromLastPlane = new();
        private readonly Random _random = new();
        private int _currentAirplaneLimit;
        private float _currentTime;

        private void Awake()
        {
            _currentTime = 0;   
            _timeFromLastPlane.Start();
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            _currentAirplaneLimit = (int)(startMoneyLimit * moneyLimitCurve.Evaluate(_currentTime));
            if (_moneyCount < _currentAirplaneLimit && _timeFromLastPlane.Elapsed > TimeSpan.FromSeconds(coolDown))
            {
                NewRandomPlane();
                _moneyCount++;
                _timeFromLastPlane.Restart();
            }
        }

        private void NewRandomPlane()
        {
            Instantiate(prefabMoney, GetRandomPoint(), Quaternion.identity);
        }

        private Vector3 GetRandomPoint() 
            => new(_random.Next(0, spawnX), _random.Next(0, spawnY), 0);

        public void KilledMoney()
        {
            _moneyCount--;
        }
    }
}