﻿using UnityEngine;using System.Collections.Generic;/// <summary>
/// 
/// </summary>[RequireComponent(typeof(AudioSource))]public class LaserGun : MonoBehaviour, IGun{    public GunOwner _owner;    public bool _shooting { get; set; }    public bool _alwaysShooting = true;    public float _distance;    public float _damagePerSec;    public List<Transform> _shootPoints;    // Префабы начала, середины, конца и эффекта попадания лазера    public GameObject _laserStart;    public GameObject _laserMiddle;    public GameObject _laserEnd;    public GameObject _laserEffect;    Transform _start;    Transform _middle;    Transform _end;    Transform _effect;    float _startWidth;    float _middleWidth;    float _endWidth;    AudioSource _audio;    void Start()    {        _audio = gameObject.GetComponent<AudioSource>();        _start = Instantiate(_laserStart).transform;        _middle = Instantiate(_laserMiddle).transform;        _end = Instantiate(_laserEnd).transform;        _effect = Instantiate(_laserEffect).transform;        SetLaserActive(false);        _startWidth = _start.GetComponent<SpriteRenderer>().bounds.size.x;        _middleWidth = _middle.GetComponent<SpriteRenderer>().bounds.size.x;        _endWidth = _end.GetComponent<SpriteRenderer>().bounds.size.x;    }    void Update()    {        if (!_shooting && !_alwaysShooting)        {            SetLaserActive(false);            return;        }                    foreach(var point in _shootPoints)        {            var startPos = point.position;            var dir = point.up;            var hit = Physics2D.Raycast(startPos, dir, _distance, LayerMask.GetMask(_owner == GunOwner.Player ? "Enemies" : "Player"));            Vector3 endPos;            if(hit.collider != null && hit.distance >= _startWidth + _middleWidth + _endWidth)            {                endPos = hit.point;                var health = hit.collider.gameObject.GetComponent<Health>();                if(health != null)                {                    health.UpdateHealth(-_damagePerSec * Time.deltaTime);                }            }            else            {                endPos = startPos + dir * _distance;            }            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;            var startPiecePos = startPos + dir * _startWidth / 2f;            var endPiecePos = endPos - dir * _endWidth / 2f;            var middleWidth = (endPos - startPos).magnitude - _endWidth - _startWidth;            var middlePiecePos = startPos + dir * _startWidth + dir * middleWidth / 2f;            _start.position = startPiecePos;            _start.rotation = Quaternion.Euler(0, 0, angle);            _middle.position = middlePiecePos;            _middle.rotation = Quaternion.Euler(0, 0, angle);            _middle.localScale = new Vector3(middleWidth / _middleWidth, _middle.localScale.y, _middle.localScale.z);            _end.position = endPiecePos;            _end.rotation = Quaternion.Euler(0, 0, angle);            _effect.position = endPos;            SetLaserActive(true);        }    }    void SetLaserActive(bool active)    {        _start.gameObject.SetActive(active);        _middle.gameObject.SetActive(active);        _end.gameObject.SetActive(active);        _effect.gameObject.SetActive(active);        if(active)
        {
            _audio.loop = true;
            _audio.PlayOneShot(_audio.clip);
        }        else
        {
            _audio.Stop();
        }    }    void OnDestroy()
    {
        SetLaserActive(false);
    }}