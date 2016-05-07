﻿using UnityEngine;using System.Collections.Generic;struct MachineGunBullet{    public float _timeActive;    public GameObject _bullet;}public class MachineGun : MonoBehaviour, IGun{    public bool _shooting { get; set; }    public bool _alwaysShooting;    public GunOwner _owner;    public List<Transform> _shootPoints;    public float _shakeIntensity;    public float _shakeDuration;    public int _bulletsPerSecond;    public float _distance;    public float _distanceSpread;    public float _angleSpread;    public float _damage;    CameraShake _shake;    public GameObject _bulletPrefab;    public float _bulletVisibleTime;    List<MachineGunBullet> _bulletPool;    int _activeBulletsCount;    float _lastShootTime;    int _currentShootPoint;    bool _lastShooting;    bool _lastAlwaysShooting;    void Start()    {        _shake = FindObjectOfType<CameraShake>();        _bulletPool = new List<MachineGunBullet>();        SetupPool();    }    void Update()    {        if (_shootPoints.Count == 0)            return;        ProcessBullets();        if (_shooting != _lastShooting || _alwaysShooting != _lastAlwaysShooting)
        {
            _lastShootTime = Time.time;
        }        _lastShooting = _shooting;        _lastAlwaysShooting = _alwaysShooting;        if (!_shooting && !_alwaysShooting)            return;        var shootCount = (int)(_bulletsPerSecond * (Time.time - _lastShootTime));        if(shootCount > 0)        {            Shoot(shootCount);        }            }    void ProcessBullets()    {        for(int i = 0; i < _activeBulletsCount; i++)        {            _bulletPool[i] = new MachineGunBullet { _bullet = _bulletPool[i]._bullet, _timeActive = _bulletPool[i]._timeActive + Time.deltaTime };            if(_bulletPool[i]._timeActive > _bulletVisibleTime)            {                var tmp = new MachineGunBullet { _bullet = _bulletPool[i]._bullet };                tmp._bullet.SetActive(false);
                // Передвигаем неактивную пулю в хвост массива ко всем остальным неактивным пулям и уменьшаем кол-во активных пуль
                _activeBulletsCount -= 1;                _bulletPool[i] = _bulletPool[_activeBulletsCount];                _bulletPool[_activeBulletsCount] = tmp;            }        }    }    void SetupPool()    {        var length = _bulletsPerSecond + (int)(_bulletsPerSecond * _bulletVisibleTime);        IncreasePool(length);    }    void IncreasePool(int length)    {        for (int i = 0; i < length; i++)        {            var obj = Instantiate(_bulletPrefab);            obj.SetActive(false);            _bulletPool.Add(new MachineGunBullet() { _bullet = obj });        }    }    void Shoot(int numBullets)    {        var neededCound = _activeBulletsCount + numBullets;        if (neededCound > _bulletPool.Count)        {            IncreasePool(neededCound - _bulletPool.Count);        }        var count = _activeBulletsCount + numBullets;        for (int i = _activeBulletsCount; i < count; i++)        {            var p = _shootPoints[_currentShootPoint].transform;            _currentShootPoint += 1;            if (_currentShootPoint >= _shootPoints.Count)                _currentShootPoint = 0;            var dir = p.up;            var theta = Random.Range(-_angleSpread, _angleSpread) * Mathf.Deg2Rad;            var cos = Mathf.Cos(theta);            var sin = Mathf.Sin(theta);            dir = new Vector3(dir.x * cos - dir.y * sin, dir.x * sin + dir.y * cos);            var distance = _distance + Random.Range(-_distanceSpread, _distanceSpread);            var hit = Physics2D.Raycast(new Vector2(p.position.x, p.position.y), dir, distance, LayerMask.GetMask(_owner == GunOwner.Player ? "Enemies" : "Player"));            Vector3 endPoint;            if(hit.collider != null)            {                endPoint = hit.point;                var health = hit.collider.gameObject.GetComponent<Health>();                if(health != null)
                {
                    health.UpdateHealth(-_damage);
                }            }            else            {                endPoint = p.position + dir * distance;            }            var b = _bulletPool[i]._bullet;            var diff = endPoint - p.position;            b.transform.position = Vector3.Lerp(p.position, endPoint, 0.5f);            b.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);            b.transform.localScale = new Vector3(diff.magnitude, b.transform.localScale.y, b.transform.localScale.z);            _shake.Shake(_shakeDuration, _shakeIntensity);            b.SetActive(true);            _bulletPool[i] = new MachineGunBullet { _bullet = b, _timeActive = 0f };            _activeBulletsCount++;        }        _lastShootTime = Time.time;    }}