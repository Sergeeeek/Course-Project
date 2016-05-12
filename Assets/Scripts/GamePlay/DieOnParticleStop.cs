using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Класс для уничтожения или отключения игровых объектов при остановке системы частиц
/// </summary>
public class DieOnParticleStop : MonoBehaviour
{
    // Список систем частиц в этом объекте
	List<ParticleSystem> _particleSystems;
    [Tooltip("Если true, то объект будет отключён, а не уничтожен")]
    public bool _disable;

	void Start()
	{
		_particleSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
	}

	void Update()
	{
        // Если систем частиц нет или все системы частиц остановлены
		if(_particleSystems.Count == 0 || _particleSystems.All(x => x.isStopped))
        {
            if(_disable)
            {
                // Отключаем игровой объект
                gameObject.SetActive(false);
            }
            else
            {
                // Уничтожаем объект
                Destroy(gameObject);
            }
        }
			
	}
}
