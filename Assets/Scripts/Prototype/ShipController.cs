using UnityEngine;

public class ShipController : MonoBehaviour
{
	public ProjectileManager _gun;

	public float _acceleration;
	public float _maxSpeed;
	public float _drag;

	Vector3 _currentVelocity;
	Vector3 _targetPosition;

	void Start()
	{
		if(_gun == null)
		{
			Debug.LogError("Gun = null");
			gameObject.SetActive(false);
		}
	}

	void Update()
	{
		Vector3 input;

		if(Input.touchSupported)
		{
			if(Input.touchCount == 0)
				return;

			var t = Input.GetTouch(0);
			_targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y));

			input = _targetPosition - transform.position;

		}
		else
		{
			var x = Input.GetAxisRaw("Horizontal");
			var y = Input.GetAxisRaw("Vertical");

			input = new Vector3(x, y);

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
		}

		input.Normalize();

		_currentVelocity += _acceleration * input;

		if(_currentVelocity.magnitude >= _maxSpeed)
		{
			_currentVelocity.Normalize();
			_currentVelocity *= _maxSpeed;
		}

		if(Mathf.Approximately(0, _currentVelocity.magnitude))
		{
			_currentVelocity = Vector3.zero;
		}

		transform.Translate(_currentVelocity * Time.deltaTime);


		_gun._shooting = Input.GetButton("ShipFire") || Input.touchSupported;
	}
}
