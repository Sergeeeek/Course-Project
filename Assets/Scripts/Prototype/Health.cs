using UnityEngine;

public class Health : MonoBehaviour
{
	public float _health = 100f;
	public GameObject _deathEffect;

	public void UpdateHealth(float health)
	{
		_health += health;

		if(_health <= 0f)
		{
			var obj = Instantiate(_deathEffect);
			obj.transform.position = transform.position;
			obj.transform.rotation = transform.rotation;
			Destroy(gameObject);
		}
	}
}
