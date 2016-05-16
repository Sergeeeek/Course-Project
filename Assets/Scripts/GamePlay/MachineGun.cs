﻿using UnityEngine;using System.Collections.Generic;struct MachineGunBullet{    public float _timeActive;    public GameObject _bullet;}[RequireComponent(typeof(AudioSource))]public class MachineGun : MonoBehaviour, IGun{    // Имплементация интерфейса IGun    public bool _shooting { get; set; }    [Tooltip("Если true, то пулемёт будет всегда стрелять")]    public bool _alwaysShooting;    [Tooltip("Кто владеет этим пулемётом")]    public GunOwner _owner;    [Tooltip("Звук выстрела")]    public AudioClip _gunSound;    [Range(0f, 1f)]    [Tooltip("Громкость выстрела")]    public float _volume = 1f;    [Tooltip("Вариация громкости выстрела")]    public float _volumeVariation;    [Tooltip("Список точек из которых будут создаваться пули")]    public List<Transform> _shootPoints;    [Tooltip("Сила тряски")]    public float _shakeIntensity;    [Tooltip("Продолжительность тряски")]    public float _shakeDuration;    [Tooltip("Кол-во выстрелов в сек")]    public int _bulletsPerSecond;    [Tooltip("Расстояние на которое пули пролетают")]    public float _distance;    [Tooltip("Вариация дистанции +-")]    public float _distanceSpread;    [Tooltip("Угол разброса +-")]    public float _angleSpread;    [Tooltip("Урон каждой пули")]    public float _damage;    // Ссылка на компонент тряски камеры    CameraShake _shake;    // Ссылка на источник звука    AudioSource _source;    [Tooltip("Префаб пули")]    public GameObject _bulletPrefab;    [Tooltip("Сколько пуля будет видна на экране")]    public float _bulletVisibleTime;    // Массив всех пуль    List<MachineGunBullet> _bulletPool;    // Кол-во активных пуль    int _activeBulletsCount;    // Время последнего выстрела    float _lastShootTime;    // Текущее дуло    int _currentShootPoint;    // Состояния переменных _shooting и _alwaysShooting с последнего кадра    bool _lastShooting;    bool _lastAlwaysShooting;    void Start()    {        // Находим компонент тряски        _shake = FindObjectOfType<CameraShake>();        // Находим источник звука        _source = GetComponent<AudioSource>();        // Создаём массив пуль        _bulletPool = new List<MachineGunBullet>();        // Заполняем массив        SetupPool();    }    // Изначальное расширение массива, предсказывает сколько понадобится пуль    void SetupPool()    {        // Кол-во пуль в массиве = кол-во выстрелов в секунду * время каждой пули на экране        var length = Mathf.CeilToInt(_bulletsPerSecond * _bulletVisibleTime);        IncreasePool(length);    }    // Функция для расширения массива    void IncreasePool(int length)    {        for (int i = 0; i < length; i++)        {            var obj = Instantiate(_bulletPrefab);            obj.SetActive(false);            _bulletPool.Add(new MachineGunBullet() { _bullet = obj });        }    }    void Update()    {        // Если нет точек для стрельбы        if (_shootPoints.Count == 0)            return;        ProcessBullets();        if (_shooting != _lastShooting || _alwaysShooting != _lastAlwaysShooting)        {            _lastShootTime = Time.time;        }        _lastShooting = _shooting;        _lastAlwaysShooting = _alwaysShooting;        if (!_shooting && !_alwaysShooting)            return;        var shootCount = (int)(_bulletsPerSecond * (Time.time - _lastShootTime));        if(shootCount > 0)        {            Shoot(shootCount);        }            }    // Эта функция проходится по активным пулям и деактивирует их если они были на экране достаточно долго    void ProcessBullets()    {        // Т.к. все неактивные пули находятся в конце массива и известно с какого индекса они начинаются (_activeBulletsCount)        // можно просто линейно пройтись по массиву предполагая что каждая пуля активан        for(int i = 0; i < _activeBulletsCount; i++)        {            // Увеличиваем время пули на экране на deltaTime (время с предыдущего кадра в сек.)            _bulletPool[i] = new MachineGunBullet { _bullet = _bulletPool[i]._bullet, _timeActive = _bulletPool[i]._timeActive + Time.deltaTime };            // Если пуля была на экране дольше чем нужно            if(_bulletPool[i]._timeActive >= _bulletVisibleTime)            {                // Запоминаем GameObject пули                var tmp = new MachineGunBullet { _bullet = _bulletPool[i]._bullet };                // Деактивируем её                tmp._bullet.SetActive(false);                // Передвигаем неактивную пулю в хвост массива ко всем остальным неактивным пулям и уменьшаем кол-во активных пуль                _activeBulletsCount -= 1;                _bulletPool[i] = _bulletPool[_activeBulletsCount];                _bulletPool[_activeBulletsCount] = tmp;            }        }    }    // Функция выстрела    void Shoot(int numBullets)    {        // Кол-во пуль которые должны быть в массиве        var neededCound = _activeBulletsCount + numBullets;        // Если нужно больше пуль чем уже есть        if (neededCound > _bulletPool.Count)        {            // То расширяем массив            IncreasePool(neededCound - _bulletPool.Count);        }        // Находим индекс последней пули которая будет использоватся в цикле ниже        var count = _activeBulletsCount + numBullets;        // Цикл от первой неактивной пули до последней которая нужна        for (int i = _activeBulletsCount; i < count; i++)        {            // Находим компонент transform начала выстрела            var p = _shootPoints[_currentShootPoint].transform;            _currentShootPoint += 1;            if (_currentShootPoint >= _shootPoints.Count)                _currentShootPoint = 0;            // Направление пули = верх в локальных координатах точки выстрела            var dir = p.up;            // Прибавляем угол разброса к направлению            var theta = Random.Range(-_angleSpread, _angleSpread) * Mathf.Deg2Rad;            var cos = Mathf.Cos(theta);            var sin = Mathf.Sin(theta);            dir = new Vector3(dir.x * cos - dir.y * sin, dir.x * sin + dir.y * cos);            // Получаем расстояние на которое полетит эта пуля с учётом расброса            var distance = _distance + Random.Range(-_distanceSpread, _distanceSpread);            // Эта функция запустит луч в 2D пространстве из точки p в направлении dir и на дистанцию distance,            // при этом она будет игнорировать корабль игрока, если это его пуля или корабли врагов если это пуля врагов            // Если луч попадёт куда-нибудь, то функция вернёт объект RaycastHit2D в котором содержится точка попадания и объект в который попал луч            var hit = Physics2D.Raycast(new Vector2(p.position.x, p.position.y), dir, distance, LayerMask.GetMask(_owner == GunOwner.Player ? "Enemies" : "Player"));            // Точка конца линии пули            Vector3 endPoint;            // Если луч куда-то попал            if(hit.collider != null)            {                // Получаем точку попадания                endPoint = hit.point;                // Получаем компонент здоровья объекта в который попал луч                var health = hit.collider.gameObject.GetComponent<Health>();                if(health != null)                {                    // Наносим урон                    health.UpdateHealth(-_damage);                }            }            else            {                // Иначе пуля пролетит всю distance и никуда не попадёт                endPoint = p.position + dir * distance;            }            // Получаем текущую пулю            var b = _bulletPool[i]._bullet;            // Вектор от начала до конца линии пули            var diff = endPoint - p.position;            // Позиция объекта линии пули = в середине между началом выстрела и концом            b.transform.position = Vector3.Lerp(p.position, endPoint, 0.5f);            // Угол пули = арктангенсу             b.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);            // Масштаб пули по X равен длине вектора разности между началом и концом выстрела            b.transform.localScale = new Vector3(diff.magnitude, b.transform.localScale.y, b.transform.localScale.z);            // Трясём камеру            _shake.Shake(_shakeDuration, _shakeIntensity);            // Активируем пулю            b.SetActive(true);            // Приравниваем время на экране к 0            _bulletPool[i] = new MachineGunBullet { _bullet = b, _timeActive = 0f };            // Увеличиваем кол-во активных пуль            _activeBulletsCount++;            // Играем звук выстрела            _source.PlayOneShot(_gunSound, _volume + Random.Range(-_volumeVariation, _volumeVariation));        }        // Запоминаем последний раз выстрела        _lastShootTime = Time.time;    }    // Когда этот объект уничтожается    void OnDestroy()    {        foreach(var bullet in _bulletPool)        {            // Уничтожить все пули в массиве            Destroy(bullet._bullet);        }    }}