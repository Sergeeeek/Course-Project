using UnityEngine;

public class ShipController : MonoBehaviour
{
	public ProjectileManager _gun;

	public float _acceleration;
	public float _maxSpeed;
	public float _drag;

	Vector3 _currentVelocity;
	Vector3 _targetPosition;

	Vector3 _objectSize;

	void Start()
	{
		if(_gun == null)
		{
			Debug.LogError("Gun = null");
			gameObject.SetActive(false);
		}

		_objectSize = GetComponent<SpriteRenderer>().bounds.size;
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
			var xInput = Input.GetAxisRaw("Horizontal");
			var yInput = Input.GetAxisRaw("Vertical");

			input = new Vector3(xInput, yInput);

			var drag = new Vector3();

			if(Mathf.Approximately(0, xInput))
			{
				drag.x = -_currentVelocity.x * _drag;
			}
			if(Mathf.Approximately(0, yInput))
			{
				drag.y = -_currentVelocity.y * _drag;
			}

			_currentVelocity += drag;
		}

		input.Normalize();
		_currentVelocity += _acceleration * input;

		if(_currentVelocity.magnitude >= _maxSpeed)
		{
			_currentVelocity *= _maxSpeed / _currentVelocity.magnitude;
		}

		if(Mathf.Approximately(0, _currentVelocity.magnitude))
		{
			_currentVelocity = Vector3.zero;
		}

		transform.Translate(_currentVelocity * Time.deltaTime, Space.World);

		keepOnScreen();

		_gun._shooting = Input.GetButton("ShipFire") || Input.touchSupported;
	}

	void keepOnScreen()
	{
		var bottomleftCam = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
		var toprightCam = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));

		var z = transform.position.z;

		var size = _objectSize;

		if(transform.position.x - size.x / 2f <= bottomleftCam.x)
		{
			transform.position = new Vector3(bottomleftCam.x + size.x / 2f, transform.position.y, z);
			_currentVelocity.x = 0f;
		}
		if(transform.position.x + size.x / 2f >= toprightCam.x)
		{
			transform.position = new Vector3(toprightCam.x - size.x / 2f, transform.position.y, z);
			_currentVelocity.x = 0f;
		}
		if(transform.position.y - size.y / 2f <= bottomleftCam.y)
		{
			transform.position = new Vector3(transform.position.x, bottomleftCam.y + size.y / 2f, z);
			_currentVelocity.y = 0f;
		}
		if(transform.position.y + size.y / 2f >= toprightCam.y)
		{
			transform.position = new Vector3(transform.position.x, toprightCam.y - size.y / 2f, z);
			_currentVelocity.y = 0f;
		}
	}
}
