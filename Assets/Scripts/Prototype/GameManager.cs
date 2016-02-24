using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject _gameOverScreen;

	GameObject _player;
	int _score;

	void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player");

		if(_player == null)
		{
			Debug.LogError("Игрок не найден");
			gameObject.SetActive(false);
		}
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
}
