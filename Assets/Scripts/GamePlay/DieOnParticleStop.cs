using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DieOnParticleStop : MonoBehaviour
{
	List<ParticleSystem> _particleSystems;
    public bool _disable;

	void Start()
	{
		_particleSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
	}

	void Update()
	{
		if(_particleSystems.Count == 0 || _particleSystems.All(x => x.isStopped))
        {
            if(_disable)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
			
	}
}
