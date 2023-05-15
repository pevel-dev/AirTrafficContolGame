using System;
using System.Collections;
using System.Collections.Generic;
using Source.Controllers;
using UnityEngine;
using Random = System.Random;

namespace Source.Models
{
    public class Cloud : MonoBehaviour
    {
        [SerializeField]
        [Header("Кривая появления облака")]
        private AnimationCurve startCurve;
        [SerializeField]
        [Header("Кривая удаления облака")]
        private AnimationCurve endCurve;
        private SpriteRenderer _sprite;
        private float _secondsLeft;
        private readonly Random _random = new();
        private bool _fadeOutCalled;
        private int _speed;

        private static readonly Vector3[] Movements =
        {
            Vector3.back, Vector3.up, Vector3.left, Vector3.right
        };
        
        void Awake()
        {
            _secondsLeft = _random.Next(10, 20);
            _sprite = GetComponent<SpriteRenderer>();
            StartCoroutine(FadeIn());
            _speed = _random.Next(13, 20);
        }

        IEnumerator FadeIn ()
        {
            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime* 0.3f;
                var a = startCurve.Evaluate(t);
                _sprite.color = new Color (1f, 1f, 1f, a);
                yield return 0;
            }
        }
        
        IEnumerator FadeOut()
        {
            var t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime* 0.3f;
                var a = endCurve.Evaluate(t);
                _sprite.color = new Color (1f, 1f, 1f, a);
                yield return 0;
            }

            CloudsController.CloudsCount--;
            Destroy(gameObject);
        }
    
        void Update()
        {
            _secondsLeft -= Time.deltaTime;
            if (_secondsLeft < 0 && _fadeOutCalled is false)
            {
                _fadeOutCalled = true;
                StartCoroutine(FadeOut());
            }
            transform.Translate(Movements[_random.Next(0, Movements.Length)] * (Time.deltaTime * _speed));
           
        }
    }
}


