using UnityEngine;
using System.Collections.Generic;

public class Parallax : MonoBehaviour
{
    public List<Transform> _tiledLayers;
    public List<Transform> _layers;
    public Vector3 _parallaxDir = Vector3.left;
    public float _playerSpeed = 1;

    List<Transform> _tiledLayerTiles;

    void Start()
    {
        _tiledLayers = _tiledLayers ?? new List<Transform>();
        _layers = _layers ?? new List<Transform>();

        _tiledLayerTiles = new List<Transform>();

        foreach (var tile in _tiledLayers)
        {
            var obj = Instantiate(tile);
            obj.transform.SetParent(tile.transform.parent, false);
            _tiledLayerTiles.Add(obj);
        }
    }

    void Update()
    {
        foreach (var layer in _layers)
        {
            TranslateLayer(layer);
        }

        for (int i = 0; i < _tiledLayers.Count; i++)
        {
            var width = _tiledLayers[i].gameObject.GetComponent<SpriteRenderer>().bounds.size.x;

            TranslateLayer(_tiledLayers[i].transform);

            if(_tiledLayers[i].position.x <= -width)
            {
                var temp = _tiledLayerTiles[i];
                _tiledLayerTiles[i] = _tiledLayers[i];
                _tiledLayers[i] = temp;
            }

            _tiledLayerTiles[i].position = _tiledLayers[i].position + Vector3.right * width;
        }
    }

    void TranslateLayer(Transform layer)
    {
        layer.Translate(_parallaxDir * _playerSpeed * (1f / layer.position.z) * Time.deltaTime, Space.World);
    }
}