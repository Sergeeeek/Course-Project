using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Данные использующиеся для тряски
/// </summary>
public struct ShakeData
{
    // Время начала тряски
    public float _shakeStart;
    // Как долго длится тряска
    public float _shakeDuration;
    // Сила тряски
    public float _shakeStrength;
    // Случайный параметр для функции шума
    public float _noiseRandom;
}

/// <summary>
/// Компонент отвечающий за тряску камеры при попаданиях, взрывах и т.д.
/// </summary>
public class CameraShake : MonoBehaviour
{
    [Tooltip("Кривая для смягчения начала и конца тряски.")]
    public AnimationCurve _startEndEasing;

    // Список данных для тряски, чтобы можно было трясти камеру сразу несколькими разными способами
    public List<ShakeData> _shakeQueue;

    void Start()
    {
        // Инициализация
        _shakeQueue = new List<ShakeData>();
    }

    /// <summary>
    /// Публичная функция для запросов на тряску
    /// </summary>
    /// <param name="duration">Сколько тряска длится</param>
    /// <param name="strength">Сила тряски</param>
    public void Shake(float duration, float strength)
    {
        if (duration == 0f || strength == 0f)
            return;

        // Создаём и заполняем данные
        var data = new ShakeData();
        data._shakeStart = Time.time;
        data._shakeDuration = duration;
        data._shakeStrength = strength;
        // Случайный параметр для функции шума, чтобы тряски отличались друг от друга
        data._noiseRandom = Random.Range(0f, 100000f);
        _shakeQueue.Add(data);
    }

    void Update()
    {
        // Итерация листа с конца к началу, т.к. в теле цикла может произойти удаление элемента листа
        for(int i = _shakeQueue.Count - 1; i >= 0; i--)
        {
            var shake = _shakeQueue[i];
            // Если время тряски закончилось
            if (Time.time - shake._shakeStart > shake._shakeDuration)
            {
                // Удаляем её и продолжаем к следующему элементу цикла
                _shakeQueue.RemoveAt(i);
                continue;
            }

            // Получаем прогресс текущей тряски как значение от 0 до 1
            var t = (Time.time - shake._shakeStart) / shake._shakeDuration;
            // Подставляем прогресс в кривую из инспектора и получаем значение смягчения тряски
            var easing = _startEndEasing.Evaluate(t);

            // Используем функцию шума перлина с параметром: время с начала тряски + случайное число по x, умножаем на силу тряски и смягчение
            var noise = Mathf.PerlinNoise(Time.time - shake._shakeStart + shake._noiseRandom, 0f) * shake._shakeStrength * easing;
            // получаем случайный единичный вектор и умножаем его на шум
            var rand = Random.insideUnitCircle * noise;
            // Двигаем камеру в полученном направлении
            transform.position += new Vector3(rand.x, rand.y) * Time.deltaTime;

            // Запоминаем z позицию
            var tempz = transform.localPosition.z;
            // Умножаем позицию камеры на смягчение [0..1], для того чтобы камера плавно возвращалась в свое положение
            var backforce = transform.localPosition * easing;
            // Восстанавливаем z позицию
            backforce.z = tempz;
            transform.localPosition = backforce;
        }
    }
}