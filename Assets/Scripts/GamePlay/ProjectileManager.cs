using System.Collections.Generic; // Нужно что-бы использовать тип List<T>
using UnityEngine; // Для классов MonoBehaviour, GameObject, Transform и т.д.

public class ProjectileManager : MonoBehaviour
{
	// Нужно для определения владельца пули
	public enum BulletOwner 
	{
		EnemyBullet,
		PlayerBullet
	}

	public List<Transform> _shoootPoints = new List<Transform>(); // Список точек откуда будут появлятся пули
	public GameObject _projectilePrefab; // Префаб пуль

	public BulletOwner _bulletOwner; // Владелец всех пуль этого ProjectileManager'а

	public float _projectileSpeed; // Скорость пуль
	public float _projectileDamage; // Урон пуль
	public bool _alwaysShoot; // Если true, то всегда стреляет, создано для врагов
	public float _shootInterval; // Интервал между выстрелами в секундах
	public float _shakeStrength = 5;
	public float _shakeDuration = 0.2f;


	public bool _shooting; // Если true, то стреляет, сделано для изменения значения их других скриптов, например ShipController

	List<GameObject> _projectilePool; // Список пуль

	float _timeFromLastShot; // Время с предыдущего выстрела

	void Start() // Эта функция автоматически вызывается Unity при старте игры
	{
		if(_projectilePrefab == null) // Если префаб пули не установлен в редакторе
			gameObject.SetActive(false); // Отключаем этот объект

		SetupPool(); // Инициализация списка пуль
	}

	void Update() // Эта функция вызывается Unity каждый кадр
	{
		_timeFromLastShot += Time.deltaTime; // Отсчёт времени с предыдущего выстрела, Time.deltaTime = время с предыдущего кадра

		if((_alwaysShoot || _shooting) && _timeFromLastShot >= _shootInterval)
		{
			Shoot();
		}
	}

	void SetupPool()
	{
		var poolSize = _shoootPoints.Count * (int)Mathf.Round((1f / _shootInterval)); // Расчёт начального размера пула

		_projectilePool = new List<GameObject>(poolSize);

		for(int i = 0; i < poolSize; i++)
		{
			SpawnProjectile(); // спауним заранее, позже они будут использованы несколько раз
		}
	}

	void Shoot() // Стрельба
	{
		// Для каждой точки из которой должны появлятся пули
		foreach(var point in _shoootPoints)
		{
			var freeProj = _projectilePool.Find(x => !x.activeInHierarchy); // Находим неактивную пулю в списке

			freeProj = freeProj ?? SpawnProjectile(); // Если такой не нашлось, создаём новую

			freeProj.transform.position = point.position; // Устанавливаем начальную позицию и направление пули
			freeProj.transform.rotation = point.rotation;

			var projComponent = freeProj.GetComponent<Projectile>(); // Находим скрипт управляющий пулей в объекте
			projComponent._speed = _projectileSpeed; // И устанавливаем скорость её движения

			freeProj.SetActive(true); // Активизируем объект
		}

		_timeFromLastShot = 0f; // Теперь время с предыдущего выстрела = 0
	}

	GameObject SpawnProjectile()
	{
		// Создаём объект из префаба
		var obj = Instantiate (_projectilePrefab);
		obj.SetActive (false); // Отключаем его чтобы он не отрисовывался и не обновлялся
		obj.gameObject.tag = _bulletOwner.ToString (); // присваиваем тэг для этой пули
		var projectile = obj.GetComponent<Projectile> ();
		projectile._damage = _projectileDamage; // Устанавливаем урон наносимый этой пулей
		projectile._shakeDuration = _shakeDuration;
		projectile._shakeStrength = _shakeStrength;

		_projectilePool.Add (obj); // Добавляем её в пул

		return obj; // и возвращаем;
	}

	// Эта функция вызывается Unity когда объект уничтожен
	void OnDestroy()
	{
		// Для каждой пули в списке
		foreach(var obj in _projectilePool)
		{
			if(obj == null) // если она не существует
				continue; // пропустить её
			
			var proj = obj.GetComponent<Projectile>();

			if(proj != null)
			{
				proj._dieOnHit = true;
			}
		}
	}
}
