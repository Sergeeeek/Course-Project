﻿using UnityEngine;

public class Health : MonoBehaviour
{
	public float _maxHealth = 100f;
	public float _health;
	public GameObject _deathEffect;
	public bool _regenHealth;
	public float _regenAmountPerSecond;
	public float _timeBeforeRegen;

	float _lastDamageTime;

    void Start()
    {
        _health = _maxHealth;
    }

	void Update()
	{
		if (_regenHealth)
		{
			if (Time.time - _lastDamageTime > _timeBeforeRegen)
			{
				_health = Mathf.Min(_health + _regenAmountPerSecond * Time.deltaTime, _maxHealth);
			}
		}
	}

	public void UpdateHealth(float health)
	{
		_health += health;

		if (health < 0)
		{
			_lastDamageTime = Time.time;
		}

		if(_health <= 0f)
		{
			var obj = Instantiate(_deathEffect);
			obj.transform.position = transform.position;
			obj.transform.rotation = transform.rotation;
			Destroy(gameObject);
		}
	}
}