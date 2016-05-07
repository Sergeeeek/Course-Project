using UnityEngine;
using System.Collections.Generic;

public class Parallax : MonoBehaviour
{
    [Tooltip("Список слоёв параллакса которые должны прокручиваться бесконечно")]
    public List<Transform> _tiledLayers;
    [Tooltip("Направление движения слоёв")]
    public Vector3 _parallaxDir = Vector3.left;
    [Tooltip("Скорость движения игрока, используется для ускорения движения слоёв")]
    public float _playerSpeed = 1;

    // Список копий слоёв для бесконечного повторения
    List<Transform> _tiledLayerTiles;

    void Start()
    {
        _tiledLayers = _tiledLayers ?? new List<Transform>();

        _tiledLayerTiles = new List<Transform>();

        foreach (var tile in _tiledLayers)
        {
            // Создаём клоны всех слоёв
            var obj = Instantiate(tile);
            // Ставим их под один и тот же игровой объект, чтобы их масштаб был одинаков
            obj.transform.SetParent(tile.transform.parent, false);
            _tiledLayerTiles.Add(obj);
        }
    }

    void Update()
    {
        for (int i = 0; i < _tiledLayers.Count; i++)
        {
            // Получаем ширину слоя из компонента SpriteRenderer
            var width = _tiledLayers[i].gameObject.GetComponent<SpriteRenderer>().bounds.size.x;

            // Сдвигаем слой
            TranslateLayer(_tiledLayers[i].transform);

            // Если слой сдвинулся за пределы экрана
            if(_tiledLayers[i].position.x <= -width)
            {
                // Меняем клон и оригинальный слой местами
                var temp = _tiledLayerTiles[i];
                _tiledLayerTiles[i] = _tiledLayers[i];
                _tiledLayers[i] = temp;
            }

            // Устанавливаем позицию клона справа от оригинального слоя
            _tiledLayerTiles[i].position = _tiledLayers[i].position + Vector3.right * width;
        }
    }

    // Функция сдвига слоя
    void TranslateLayer(Transform layer)
    {
        // Расчёт сдвига слоя в каждый кадр зависит от $z$ координаты слоя (глубины),
        // а точнее от $z^{-1}=\frac{1}{z}$, что означает что чем дальше слой, тем медленнее он будет двигаться
        layer.Translate(_parallaxDir * _playerSpeed * (1f / layer.position.z) * Time.deltaTime, Space.World);
    }
}