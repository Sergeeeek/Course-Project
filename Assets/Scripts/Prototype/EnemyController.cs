using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public AnimationCurve _xAnimation;
	public float _animationScale;
	public float _downMovementSpeed;
	public float _animationLength;

	void Update()
	{
		_xAnimation.postWrapMode = WrapMode.Loop;

		var movement = -transform.up * _downMovementSpeed;
		movement.x = _xAnimation.Evaluate(Time.timeSinceLevelLoad / _animationLength) * _animationScale;

		transform.Translate(movement * Time.deltaTime);
	}
}
