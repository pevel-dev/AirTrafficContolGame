using System.Collections;
using System.Collections.Generic;
using Source.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int StartHeals;
    public string GameOverSceneName;
    public static int Heal { get; private set; } = 1;
    public static int Points { get; private set; }

    // Update is called once per frame

    public void Awake()
    {
        GameController.Heal = StartHeals;
    }
    void Update()
    {
        Debug.Log(GameController.Heal);
        if (GameController.Heal < 0)
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
    }
}
