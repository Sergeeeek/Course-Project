using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public struct Wave
{
    public float _pauseBeforeStarting;
    public List<GameObject> _enemySpawners;
}

public class GameManager : MonoBehaviour
{
    public GameObject _gameOverScreen;
    public Camera _gameCamera;

    public string _dialogBeforeLevel;
    public List<Wave> _enemyWaves;
    public string _dialogBeforeBoss;
    public GameObject _boss;
    public string _dialogAfterBoss;

    GameObject _player;
    DialogManager _dialogManager;

    bool _dialogBeforeLevelStarted;
    bool _dialogBeforeLevelFinished;

    int _currentWave;
    int _currentSpawner;
    bool _wavesFinished;

    bool _dialogBeforeBossStarted;
    bool _dialogBeforeBossFinished;

    bool _bossFinished;

    bool _dialogAfterBossStarted;
    bool _dialogAfterBossFinished;

    float _lastWaveTime;
    bool _waitingForSpawnerToFinish;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _dialogManager = FindObjectOfType<DialogManager>();

        if (_player == null || _gameCamera == null)
        {
            Debug.LogError("Игрок или камера не найдены");
            gameObject.SetActive(false);
        }

        Camera.SetupCurrent(_gameCamera);

        foreach (var wave in _enemyWaves)
        {
            foreach (var spawner in wave._enemySpawners)
            {
                spawner.SetActive(false);
            }
        }
        if(_boss != null)
            _boss.SetActive(false);
    }

    void FixedUpdate()
    {
        // Корабль игрока уничтожен
        if (_player == null)
        {
            GameOver();

            if (_dialogManager != null)
                _dialogManager.StopDialog();
        }

        if(!_dialogBeforeLevelStarted)
        {
            StartDialog(_dialogBeforeLevel, x =>
            {
                _dialogBeforeLevelFinished = true;
                _lastWaveTime = Time.time;
            });
            _dialogBeforeLevelStarted = true;
        }
        
        if(_enemyWaves.Count == 0)
        {
            _wavesFinished = true;
        }

        if (_dialogBeforeLevelFinished && !_wavesFinished)
        {
            if(_waitingForSpawnerToFinish)
            {
                if(!_enemyWaves[_currentWave]._enemySpawners[_currentSpawner].activeInHierarchy)
                {
                    _waitingForSpawnerToFinish = false;
                    _lastWaveTime = Time.time;
                    NextEnemySpawner();
                }
            }
            else if(Time.time - _lastWaveTime > _enemyWaves[_currentWave]._pauseBeforeStarting && !_wavesFinished)
            {
                if (_enemyWaves[_currentWave]._enemySpawners.Count == 0)
                    NextEnemySpawner();

                _enemyWaves[_currentWave]._enemySpawners[_currentSpawner].SetActive(true);
                _waitingForSpawnerToFinish = true;
            }
        }
        else if(_wavesFinished)
        {
            if (!_dialogBeforeBossStarted)
            {
                StartDialog(_dialogBeforeBoss, x => _dialogBeforeBossFinished = true);
                _dialogBeforeBossStarted = true;
            }

            if(_dialogBeforeBossFinished)
            {
                if(_boss != null)
                {
                    _boss.SetActive(true);
                }
                else
                {
                    _bossFinished = true;
                }
            }

            if(_bossFinished && !_dialogAfterBossStarted)
            {
                StartDialog(_dialogAfterBoss, x => _dialogAfterBossFinished = true);
                _dialogAfterBossStarted = true;
            }
        }
    }

    void NextEnemySpawner()
    {
        if(_currentSpawner + 1 == _enemyWaves[_currentWave]._enemySpawners.Count || _enemyWaves[_currentWave]._enemySpawners.Count == 0)
        {
            if(_currentWave + 1 == _enemyWaves.Count || _enemyWaves.Count == 0)
            {
                _wavesFinished = true;
                return;
            }

            _currentSpawner = 0;
            _currentWave += 1;
        }
        else
        {
            _currentSpawner += 1;
        }
    }

    void StartDialog(string name, System.Action<string> callback)
    {
        if (_dialogManager == null || string.IsNullOrEmpty(name))
        {
            if (callback != null)
                callback(name);
        }
        else
        {
            _dialogManager.StartDialog(name, callback);
        }
    }

    void GameOver()
    {
        if (_gameOverScreen == null)
            return;

        _gameOverScreen.SetActive(true);
    }

    public void Retry()
    {
        // Перезапуск текущего уровня
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("mainmenu");
    }
}
