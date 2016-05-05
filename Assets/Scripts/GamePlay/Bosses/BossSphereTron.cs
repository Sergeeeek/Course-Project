using UnityEngine;
using System.Collections;

public class BossSphereTron : MonoBehaviour {

	public Transform _spinningBody;
	public float _spinSpeed;

	// Use this for initialization
	void Start ()
	{
		if (_spinningBody == null)
		{
			Debug.LogError ("Не найден корпус корабля босса");
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		_spinningBody.Rotate(new Vector3 (0f, 0f, _spinSpeed * Time.deltaTime));
	}
}
