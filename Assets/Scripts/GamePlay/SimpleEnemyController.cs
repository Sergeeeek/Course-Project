using UnityEngine;

public class SimpleEnemyController : MonoBehaviour
{
	public float _speed;

	void Update()
	{
		transform.Translate(Vector3.left * _speed * Time.deltaTime);
	}
}
