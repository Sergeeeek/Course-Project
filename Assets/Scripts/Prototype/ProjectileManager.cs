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

	void FixedUpdate()
	{
		float horizExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
		float vertExtent = Camera.main.orthographicSize;

		var x = Camera.main.transform.position.x;
		var y = Camera.main.transform.position.y;

		foreach(var proj in _projectilePool)
		{
			if(Mathf.Abs(proj.transform.position.y) > vertExtent + y || Mathf.Abs(proj.transform.position.x) > horizExtent + y)
			{
				proj.SetActive(false);
			}
		}
	}

	void SetupPool()
	{
		var poolSize = _shoootPoints.Count * (int)Mathf.Round((1f / _shootInterval)) * 5;

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
			projComponent._direction = point.up;
			projComponent._speed = _projectileSpeed;

			freeProj.SetActive(true);
		}
	}

	GameObject SpawnProjectile()
	{
		var obj = Instantiate(_projectilePrefab);
		//obj.transform.parent = transform;
		obj.SetActive(false);
		_projectilePool.Add(obj);

		return obj;
	}
}
