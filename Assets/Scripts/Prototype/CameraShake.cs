using UnityEngine;
using System.Collections.Generic;

public struct ShakeData
{
    public float _shakeStart;
    public float _shakeDuration;
    public float _shakeStrength;
    public float _noiseRandom;
}

public class CameraShake : MonoBehaviour
{
    public AnimationCurve _startEndEasing;

    public List<ShakeData> _shakeQueue;

    void Start()
    {
        _shakeQueue = new List<ShakeData>();
    }

    void Update()
    {
        for(int i = _shakeQueue.Count - 1; i >= 0; i--)
        {
            var shake = _shakeQueue[i];
            if (Time.time - shake._shakeStart > shake._shakeDuration)
            {
                _shakeQueue.RemoveAt(i);
                continue;
            }

            var easing = _startEndEasing.Evaluate((Time.time - shake._shakeStart) / shake._shakeDuration);
            var noise = Mathf.PerlinNoise(Time.time - shake._shakeStart + shake._noiseRandom, 0f) * shake._shakeStrength * easing;
            var rand = Random.insideUnitCircle * noise;
            transform.position += new Vector3(rand.x, rand.y) * Time.deltaTime;
            var tempz = transform.localPosition.z;
            var backforce = transform.localPosition * easing;
            backforce.z = tempz;
            transform.localPosition = backforce;
        }
    }

    public void Shake(float duration, float strength)
    {
        var data = new ShakeData();
        data._shakeStart = Time.time;
        data._shakeDuration = duration;
        data._shakeStrength = strength;
        data._noiseRandom = Random.Range(0f, 100000f);
        _shakeQueue.Add(data);
    }
}