using UnityEngine;

public class ShipMovementScript : MonoBehaviour
{
	public float _acceleration;
	public float _maxSpeed;
	public float _drag;

	public Vector3 _currentVelocity;

	void Update()
	{
		var x = Input.GetAxisRaw("Horizontal");
		var y = Input.GetAxisRaw("Vertical");

		var input = new Vector3(x, y);
		input.Normalize();

		_currentVelocity += _acceleration * input;

		if(_currentVelocity.magnitude >= _maxSpeed)
		{
			_currentVelocity.Normalize();
			_currentVelocity *= _maxSpeed;
		}

		var drag = new Vector3();

		if(Mathf.Approximately(0, x))
		{
			drag.x = -_currentVelocity.x * _drag;
		}
		if(Mathf.Approximately(0, y))
		{
			drag.y = -_currentVelocity.y * _drag;
		}

		_currentVelocity += drag;

		if(Mathf.Approximately(0, _currentVelocity.magnitude))
		{
			_currentVelocity = Vector3.zero;
		}

		transform.Translate(_currentVelocity * Time.deltaTime);
	}
}
