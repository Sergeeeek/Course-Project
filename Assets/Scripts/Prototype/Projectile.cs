using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject _hitEffect;
	public float _damage;
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

	void OnTriggerEnter2D(Collider2D other)
	{
		var health = other.gameObject.GetComponent<Health>();
		if(health == null)
			return;

		if((gameObject.tag == "PlayerBullet" && other.gameObject.tag == "Enemy") || (gameObject.tag == "EnemyBullet" && other.gameObject.tag == "Player"))
		{
			health.UpdateHealth(-_damage);
			gameObject.SetActive(false);
		}
	}
}
