using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace Source.Controllers
{
    public class SmokeMenu : MonoBehaviour
    {
        [SerializeField] [Header("Объект дыма")]
        private GameObject smoke;

        [SerializeField] [Header("Анимация появления/исчезновения дыма")]
        private AnimationCurve animationCurve;

        [SerializeField] [Header("Скорость анимации")]
        private float animationSpeed = 0.3f;

        [SerializeField] [Header("Тексты, которые нужно анимировать")]
        private GameObject[] textToFade;

        private SpriteRenderer _spriteRenderer;
        private List<TextMeshProUGUI> _texts = new ();


        public void NextScene(string sceneName)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut(sceneName));
        }

        void Start()
        {
            _spriteRenderer = smoke.GetComponent<SpriteRenderer>();
            foreach (var obj in textToFade)
                _texts.Add(obj.GetComponent<TextMeshProUGUI>());
            StartCoroutine(FadeIn());
        }
        
        IEnumerator FadeOut(string sceneName)
        {
            var t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime * animationSpeed;
                var a = animationCurve.Evaluate(t);
                _spriteRenderer.color = new Color (1f, 1f, 1f, a);
                foreach (var text in _texts)
                    text.color = new Color (1f, 1f, 1f, a);
                yield return 0;
            }

            SceneManager.LoadScene(sceneName);
        }
        
        IEnumerator FadeIn()
        {
            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * animationSpeed;
                var a = animationCurve.Evaluate(t);
                _spriteRenderer.color = new Color (1f, 1f, 1f, a);
                foreach (var text in _texts)
                    text.color = new Color (1f, 1f, 1f, a);
                yield return 0;
            }
        }
    }
}

