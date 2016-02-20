using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float _speed;

	void Update()
	{
		var viewPortPos = Camera.main.WorldToViewportPoint(transform.position);

		if(viewPortPos.x > 1f || viewPortPos.x < 0f || viewPortPos.y > 1f || viewPortPos.y < 0f)
		{
			gameObject.SetActive(false);
			return;
		}

		transform.Translate(Vector3.up * _speed * Time.deltaTime); // Translate двигает объект относительно локальных координат
	}
}
