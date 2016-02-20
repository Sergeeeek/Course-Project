using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
	public List<Transform> _shoootPoints = new List<Transform>();
	public GameObject _projectilePrefab;

	public float _projectileSpeed;
	public bool _alwaysShoot;
	public float _shootInterval;

	public bool _shooting;

	List<GameObject> _projectilePool;

	void Start()
	{
		if(_projectilePrefab == null)
			gameObject.SetActive(false);

		SetupPool();

		StartCoroutine(ShootCoroutine());
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

	IEnumerator ShootCoroutine()
	{
		while(true)
		{
			if(_alwaysShoot || _shooting)
			{
				Shoot();
			}

			yield return new WaitForSeconds(_shootInterval);
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
	}

	GameObject SpawnProjectile()
	{
		var obj = Instantiate(_projectilePrefab);
		//obj.transform.parent = transform;
		obj.SetActive(false);
		obj.gameObject.tag = gameObject.tag;
		_projectilePool.Add(obj);

		return obj;
	}
}
