using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float _speed;
	public Vector3 _direction;

	void Update()
	{
		if(_direction.magnitude >= 1f)
		{
			_direction.Normalize();
		}
		
		transform.Translate(_direction * _speed * Time.deltaTime);
	}
}
