using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
	public enum BulletOwner
	{
		EnemyBullet,
		PlayerBullet
	}

	public List<Transform> _shoootPoints = new List<Transform>();
	public GameObject _projectilePrefab;

	public BulletOwner _bulletOwner;

	public float _projectileSpeed;
	public float _projectileDamage;
	public bool _alwaysShoot;
	public float _shootInterval;

	public bool _shooting;

	List<GameObject> _projectilePool;

	float _timeFromLastShot;

	void Start()
	{
		if(_projectilePrefab == null)
			gameObject.SetActive(false);

		SetupPool();
	}

	void Update()
	{
		_timeFromLastShot += Time.deltaTime;

		if((_alwaysShoot || _shooting) && _timeFromLastShot >= _shootInterval)
		{
			Shoot();
		}
	}

	void SetupPool()
	{
		var poolSize = _shoootPoints.Count * (int)Mathf.Round((1f / _shootInterval));

		_projectilePool = new List<GameObject>(poolSize);

		for(int i = 0; i < poolSize; i++)
		{
			SpawnProjectile();
		}
	}

	void Shoot()
	{
		foreach(var point in _shoootPoints)
		{
			var freeProj = _projectilePool.Find(x => !x.activeInHierarchy);

			freeProj = freeProj ?? SpawnProjectile();

			freeProj.transform.position = point.position;
			freeProj.transform.rotation = point.rotation;

			var projComponent = freeProj.GetComponent<Projectile>();
			projComponent._speed = _projectileSpeed;

			freeProj.SetActive(true);
		}

		_timeFromLastShot = 0f;
	}

	GameObject SpawnProjectile()
	{
		var obj = Instantiate(_projectilePrefab);
		//obj.transform.parent = transform;
		obj.SetActive(false);
		obj.gameObject.tag = _bulletOwner.ToString();
		obj.GetComponent<Projectile>()._damage = _projectileDamage;
		_projectilePool.Add(obj);

		return obj;
	}

	void OnDestroy()
	{
		foreach(var obj in _projectilePool)
		{
			if(obj == null)
				continue;
			
			var proj = obj.GetComponent<Projectile>();

			if(proj != null)
			{
				proj._dieOnHit = true;
			}
		}
	}
}
