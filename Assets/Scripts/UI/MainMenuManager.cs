using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Объект для функций главного меню
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // Загружает уровень с именем levelName
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    // Выход из игры
    public void ExitGame()
    {
        Application.Quit();
    }
}
