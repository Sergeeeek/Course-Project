using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Размер линии из которой будут появлятся враги")]
	public float _size = 10f;
    [Tooltip("Кривая определяющая как будет двигаться \"курсор\" - точка которая двигается независимо от появления врагов и в которой они появляются")]
	public AnimationCurve _animation;
    [Tooltip("Если true, \"курсор\" будет проигнорирован и корабли будут появлятся в случайных позициях на линии")]
	public bool _spawnAtRandomLocations;

    [Tooltip("Масштаб времени для получения значений из кривой Animation")]
	public float _timeScale = 1f;
    [Tooltip("Интервал в сек. между появлениями врагов")]
	public float _spawnInterval = 1f;

    [Tooltip("Префаб врага который будет создаваться")]
	public GameObject _enemyPrefab;

    [Tooltip("Сколько врагов создать перед отключением этого объекта")]
	public int _spawnCount;
    [Tooltip("Если true, то враги будут создаваться бесконечно")]
	public bool _alwaysSpawn = false;

    // Таймер для получения значений из кривой
	float _timer;
    // Время последнего создания врага
	float _lastSpawnTime;
    // Сколько врагов уже создано
	int _currentSpawnCount;

	void Start()
	{
		_timer = 0f;
	}

	void Update()
	{
        // Увеличиваем таймер
		_timer += Time.deltaTime;

        // Если прошло больше _spawnInterval с момента предыдущего спауна и заспавнены ещё не все враги или их нужно создавать бесконечно
		if(_timer >= _lastSpawnTime + _spawnInterval && (_currentSpawnCount < _spawnCount || _alwaysSpawn))
		{
            // Находим векторы низа и верха данного объекта
			var p1 = transform.TransformPoint(Vector3.down * _size / 2f);
			var p2 = transform.TransformPoint(Vector3.up * _size / 2f);

            // Если нужно создавать в случайных местах, то взять случайное число от 0, до 1 иначе получаем значение их кривой через _timer и _timeScale
			var t = _spawnAtRandomLocations ? Random.Range(0f, 1f) : _animation.Evaluate(_timer * _timeScale);

            // Делаем линейную интерполяцию между верхней и нижней точкой чтобы найти положение где создастся враг
			var lerpPos = Vector3.Lerp(p1, p2, t);

			if(_enemyPrefab != null)
				Instantiate(_enemyPrefab, lerpPos, _enemyPrefab.transform.rotation);

            // Увеличиваем кол-во созданных врагов и запоминаем время спауна
			_currentSpawnCount++;
			_lastSpawnTime = _timer;
		}
        // Если созданы все враги и не нужно бесконечно их создавать
        else if(_currentSpawnCount >= _spawnCount && !_alwaysSpawn)
        {
            // Отключаем этот объект, чего ждёт GameManager
            gameObject.SetActive(false);
        }
	}
}
