using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject _gameOverScreen;
    public Camera _gameCamera;

	GameObject _player;
	int _score;

	void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player");

		if(_player == null || _gameCamera == null)
		{
			Debug.LogError("Игрок или камера не найдены");
			gameObject.SetActive(false);
		}

        Camera.SetupCurrent(_gameCamera);
	}

	void FixedUpdate()
	{
        // Игрок ещё жив
        if(_player == null)
        {
            GameOver();
        }
	}

    void GameOver()
    {
        if (_gameOverScreen == null)
            return;

        _gameOverScreen.SetActive(true);
    }

	public void Retry()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadMainMenu()
	{
		SceneManager.LoadScene("mainmenu");
	}
}
