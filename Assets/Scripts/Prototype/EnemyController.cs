using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	public AnimationCurve _xAnimation;
	public float _animationScale;
	public float _downMovementSpeed;
	public float _animationLengthMultiplier;

	void Update()
	{
		_xAnimation.postWrapMode = WrapMode.Loop;

		var movement = -transform.up * _downMovementSpeed;
		movement.x = _xAnimation.Evaluate(Time.timeSinceLevelLoad) * _animationScale;

		transform.Translate(movement * Time.deltaTime);
	}
}
