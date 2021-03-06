﻿using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GunOwner _owner;

	// Префаб создаваемый при попадании пули
	public GameObject _hitEffect;
	// Урон от пули
	public float _damage;
	// Скорость пули
	public float _speed;
	// если true, то когда пуля с чем-нибудь столкнётся или вылетит за экран, то её объект будет 
	// уничтожен, а не деактивирован
	public bool _dieOnHit;

    public float _shakeDuration = 0.2f;
    public float _shakeStrength = 5f;

    Camera _camera;
    CameraShake _shakeCamera;

    void Start()
    {
        _camera = FindObjectOfType<GameManager>()._gameCamera;
        _shakeCamera = _camera.GetComponent<CameraShake>();
    }

	void Update()
	{
		// Получаем координаты viewport'а из координат мира этой пули
		// Координаты viewport'а начинаются в левом верхнем углу экрана (0,0) и заканчиваются в нижнем правом (1,1)
		var viewPortPos = _camera.WorldToViewportPoint(transform.position);

		if(viewPortPos.x > 1f || viewPortPos.x < 0f || viewPortPos.y > 1f || viewPortPos.y < 0f) // если за пределами экрана
		{
			DisableOrDestroy();
			return;
		}

		transform.Translate(Vector3.up * _speed * Time.deltaTime); // Translate двигает объект относительно локальных координат
	}

	// Функция вызывается когда что-либо объект попадает в триггер, в данном случае корабли в игре являются триггерами
	void OnTriggerEnter2D(Collider2D other)
	{
		var health = other.gameObject.GetComponent<Health>(); // Получаем скрипт Health триггера (корабля)
		if(health == null) // если у объекта нет компонента Health
			return;

		// Если эта пуля принадлежит игроку и триггер - враг или если эта пуля принадлежит врагу и триггер - игрок, то
		if((_owner == GunOwner.Player && other.gameObject.layer == LayerMask.NameToLayer("Enemies")) || (_owner == GunOwner.Enemy && other.gameObject.layer == LayerMask.NameToLayer("Player")))
		{
            _shakeCamera.Shake(_shakeDuration, _shakeStrength);

			health.ChangeHealth(-_damage); // наносим урон
			if(_hitEffect != null) // если есть префаб эввекта попадания
				Instantiate(_hitEffect, transform.position, Quaternion.identity); // то создаём этот эффект в месте попадания

			DisableOrDestroy(); // Уничтожаем или отключаем объект этой пули
		}
	}

	void DisableOrDestroy()
	{
		if(_dieOnHit)
		{
			Destroy(gameObject);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}
