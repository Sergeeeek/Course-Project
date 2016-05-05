using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class EnemySpawner : MonoBehaviour
{
	public float _size = 10f;
	public AnimationCurve _animation;
	public bool _spawnAtRandomLocations;

	public float _timeScale = 1f;
	public float _spawnInterval = 1f;

	public GameObject _enemyPrefab;

	public int _spawnCount;
	public bool _alwaysSpawn = false;

	float _timer;
	float _lastSpawnTime;
	int _currentSpawnCount;

	void Start()
	{
		_timer = 0f;
	}

	void Update()
	{
		if(Application.isEditor && !Application.isPlaying)
			return;
		
		_timer += Time.deltaTime;

		if(_timer >= _lastSpawnTime + _spawnInterval && (_currentSpawnCount < _spawnCount || _alwaysSpawn))
		{
			var p1 = transform.TransformPoint(Vector3.down * _size / 2f);
			var p2 = transform.TransformPoint(Vector3.up * _size / 2f);

			var t = _spawnAtRandomLocations ? Random.Range(0f, 1f) : _animation.Evaluate(_timer * _timeScale);

			var lerpPos = Vector3.Lerp(p1, p2, t);

			if(_enemyPrefab != null)
				Instantiate(_enemyPrefab, lerpPos, _enemyPrefab.transform.rotation);

			_currentSpawnCount++;
			_lastSpawnTime = _timer;
		}
        else if(_currentSpawnCount >= _spawnCount)
        {
            gameObject.SetActive(false);
        }
	}

	#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		var p1 = transform.TransformPoint(Vector3.down * _size / 2f);
		var p2 = transform.TransformPoint(Vector3.up * _size / 2f);

		Gizmos.DrawLine(p1, p2);
		_animation.postWrapMode = WrapMode.PingPong;
		var time = Application.isPlaying ? _timer : (float)EditorApplication.timeSinceStartup;
		var t = _animation.Evaluate(time * _timeScale);

		var lerpPos = Vector3.Lerp(p1, p2, t);

		Handles.DotCap(0, lerpPos, Quaternion.identity, HandleUtility.GetHandleSize(lerpPos) * 0.1f);
	}
	#endif
}
