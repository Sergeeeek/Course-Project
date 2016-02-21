using UnityEngine;

public class GameManager : MonoBehaviour
{
	GameObject _player;
	int _score;

	void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player");

		if(_player == null)
			gameObject.SetActive(false);
	}

	void Update()
	{
		
	}
}
