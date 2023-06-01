using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Ui
{
    /// <summary>
    /// Функции смены сцен
    /// </summary>
    public class ButtonChangeScene : MonoBehaviour
    {
        public AudioSource soundBoard;

        private void Awake()
        {
            soundBoard = GetComponent<AudioSource>();
        }
        
        public void LoadScene(string sceneName)
        {
            soundBoard.Play();
            SceneManager.LoadScene(sceneName);
        }

        public void ExitGame()
        {
            soundBoard.Play();
            Application.Quit();
        }
    }
}