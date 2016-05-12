using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Структура для хранения волн врагов
/// </summary>
[System.Serializable]
public struct Wave
{
    [Tooltip("Время перед началом волны в сек.")]
    public float _pauseBeforeStarting;
    [Tooltip("Объекты которые будут активироваться при начале волны, обычно спаунеры врагов.")]
    public List<GameObject> _enemySpawners;
}

/// <summary>
/// Класс ответственный за управление волнами и диалогом противников
/// </summary>
public class GameManager : MonoBehaviour
{
    [Tooltip("Экран \"Game Over\"")]
    public GameObject _gameOverScreen;
    [Tooltip("Игровая камера через которую будет всё отрисовываться")]
    public Camera _gameCamera;

    [Tooltip("Название файла с диалогом который будет выходить перед началом всех волн")]
    public string _dialogBeforeLevel;
    [Tooltip("Список волн врагов")]
    public List<Wave> _enemyWaves;
    [Tooltip("Название файла с диалогом который будет выходить перед боссом")]
    public string _dialogBeforeBoss;
    [Tooltip("Объект босса, который будет активирован когда все волны закончатся")]
    public GameObject _boss;
    [Tooltip("Диалог в конце уровня после босса")]
    public string _dialogAfterBoss;

    // Ссылка к объекту игрока
    GameObject _player;
    // Ссылка на менеджер диалогов
    DialogManager _dialogManager;

    // Начался ли диалог перед уровнем
    bool _dialogBeforeLevelStarted;
    // Закончился ли диалог перед уровнем
    bool _dialogBeforeLevelFinished;

    // Текущая волна врагов
    int _currentWave;
    // Текущий спаунер врагов
    int _currentSpawner;
    // Закончились ли все враги
    bool _wavesFinished;

    // Начался ли диалог перед боссом
    bool _dialogBeforeBossStarted;
    // Закончился ли диалог перед боссом
    bool _dialogBeforeBossFinished;

    // Побеждён ли босс
    bool _bossFinished;

    // Начался ли диалог после босса
    bool _dialogAfterBossStarted;
    // Закончился ли диалог после босса
    bool _dialogAfterBossFinished;

    // Время после конца последней волны врагов
    float _lastWaveTime;
    // Ожидается ли окончание создания врагов от спаунера
    bool _waitingForSpawnerToFinish;

    void Start()
    {
        // Находим игрока и DialogManager
        _player = GameObject.FindGameObjectWithTag("Player");
        _dialogManager = FindObjectOfType<DialogManager>();

        if (_player == null || _gameCamera == null)
        {
            Debug.LogError("Игрок или камера не найдены");
            // Отключаем объект
            gameObject.SetActive(false);
        }

        // Устанавливаем игровую камеру как камеру для отрисовки на экран
        Camera.SetupCurrent(_gameCamera);

        // Изначально отключаем спаунеры для всех волн
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

    // Эта функция вызывается Unity 30 раз в секунду
    void FixedUpdate()
    {
        // Корабль игрока уничтожен
        if (_player == null)
        {
            GameOver();

            if (_dialogManager != null)
                _dialogManager.StopDialog();
        }

        // Если диалог перед уровнем не начался
        if(!_dialogBeforeLevelStarted)
        {

            StartDialog(_dialogBeforeLevel, x =>
            {
                // Это анонимная функция в C#, такие функции удобно передавать как параметры в другие функции
                // В данном случае функция будет вызвана когда диалог завершится
                _dialogBeforeLevelFinished = true;
                _lastWaveTime = Time.time;
            });

            _dialogBeforeLevelStarted = true;
        }
        
        // Если волн нет
        if(_enemyWaves.Count == 0)
        {
            // То они завершены
            _wavesFinished = true;
        }

        // Если диалог перед уровнем завершён и волны не завершены
        if (_dialogBeforeLevelFinished && !_wavesFinished)
        {
            // Если ожидается окончание создания врагов от спаунера
            if (_waitingForSpawnerToFinish)
            {
                // Если объект текущего спаунера не активен
                if(!_enemyWaves[_currentWave]._enemySpawners[_currentSpawner].activeInHierarchy)
                {
                    // Больше не ожидаем окончания создания врагов
                    _waitingForSpawnerToFinish = false;
                    // Время после окончания последней волны - сейчас
                    _lastWaveTime = Time.time;
                    // Переклчаем спаунер
                    NextEnemySpawner();
                }
            }
            // Иначе если прошло время паузы перед текущей волной и волны не закончены
            else if(Time.time - _lastWaveTime > _enemyWaves[_currentWave]._pauseBeforeStarting && !_wavesFinished)
            {
                // Если кол-во спаунеров врагов в текущей волне = 0
                if (_enemyWaves[_currentWave]._enemySpawners.Count == 0)
                    NextEnemySpawner();

                // Активируем текущий спаунер
                _enemyWaves[_currentWave]._enemySpawners[_currentSpawner].SetActive(true);
                // Ожидаем его окончание
                _waitingForSpawnerToFinish = true;
            }
        }
        // Иначе если волны завершены
        else if(_wavesFinished)
        {
            // Если диалог перед боссом не начат
            if (!_dialogBeforeBossStarted)
            {
                // Начинаем диалог перед боссом
                StartDialog(_dialogBeforeBoss, x => _dialogBeforeBossFinished = true);
                _dialogBeforeBossStarted = true;
            }

            // Если диалог перед боссом завершён
            if(_dialogBeforeBossFinished)
            {
                // Если босс существует
                if(_boss != null)
                {
                    // Активируем его
                    _boss.SetActive(true);
                }
                else
                {
                    // Иначе босс побеждён
                    _bossFinished = true;
                }
            }

            // Если босс побеждён и диалог после босса не начат
            if(_bossFinished && !_dialogAfterBossStarted)
            {
                // Начинаем диалог после босса
                StartDialog(_dialogAfterBoss, x => _dialogAfterBossFinished = true);
                _dialogAfterBossStarted = true;
            }
        }
    }

    // Функция для переключения спаунеров врагов
    void NextEnemySpawner()
    {
        // Если текущий спаунер - последний или кол-во спаунеров = 0
        if(_currentSpawner + 1 == _enemyWaves[_currentWave]._enemySpawners.Count || _enemyWaves[_currentWave]._enemySpawners.Count == 0)
        {
            // Если текущая волна последняя или кол-во волн = 0
            if(_currentWave + 1 == _enemyWaves.Count || _enemyWaves.Count == 0)
            {
                // Волны завершены
                _wavesFinished = true;
                return;
            }

            // Переходим к следующей волне и первому спаунеру
            _currentSpawner = 0;
            _currentWave += 1;
        }
        else
        {
            // Переходим к следующему спаунеру
            _currentSpawner += 1;
        }
    }

    // Функция помощник для начала диалога
    void StartDialog(string name, System.Action<string> callback)
    {
        // Если DialogManager'а нет или название диалога - пустая строка
        if (_dialogManager == null || string.IsNullOrEmpty(name))
        {
            // Если функция callback существует
            if (callback != null)
                // Вызываем её
                callback(name);
        }
        else
        {
            // Начинаем диалог
            _dialogManager.StartDialog(name, callback);
        }
    }

    // Функция при пройгрыше
    void GameOver()
    {
        // Если экран пройгрыша существует
        if (_gameOverScreen == null)
            return;

        // Активируем его
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
