using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Ui
{
    /// <summary>
    /// Функции смены сцен
    /// </summary>
    public class ButtonChangeScene : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}