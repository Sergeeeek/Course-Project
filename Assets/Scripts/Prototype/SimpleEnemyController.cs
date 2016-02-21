using UnityEngine;

public class SimpleEnemyController : MonoBehaviour
{
	public float _speed;

	void Update()
	{
		transform.Translate(Vector3.up * _speed * Time.deltaTime);
	}
}
