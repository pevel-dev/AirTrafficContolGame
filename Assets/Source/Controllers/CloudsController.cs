using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Source.Models;
using UnityEngine;
using Random = System.Random;

namespace Source.Controllers
{
    public class CloudsController : MonoBehaviour
    {
        public static int CloudsCount { get; set;  }
        public int maxClouds;
        public GameObject cloudPrefab;
        private readonly Random _random = new();
        public Sprite[] cloudSprites;

        void Awake()
        {
            for (var i = 0; i < maxClouds; i++)
                CreateCloud();
            CloudsCount = maxClouds;
        }

        void Update()
        {
            if (CloudsCount >= maxClouds) return;
            CreateCloud();
            CloudsCount++;
        }

        private void CreateCloud()
        {
            var sprite = cloudSprites[_random.Next(0, cloudSprites.Length)];
            var cloud = Instantiate(cloudPrefab, GetRandomStart(), Quaternion.identity);
            cloud.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    
    
        private Vector3 GetRandomStart()
        {
            return new Vector3(_random.Next(0, 1920), _random.Next(0, 1080));
        }
    }
}
    

