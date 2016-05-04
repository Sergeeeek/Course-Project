using UnityEngine;

public class SimpleEnemyController : MonoBehaviour
{
	public float _speed;
    public float _sinSpeed;
    public float _sinScale = 1f;
    public float _viewportYScale = 0.9f;

    float x;

    void Start()
    {
        var _startY = Camera.main.WorldToViewportPoint(transform.position).y * 2f - 1f;
        x = Mathf.Asin(_startY);
    }

	void Update()
	{
        x += _sinSpeed * Time.deltaTime;
        var y = (Mathf.Sin(x * _sinScale) * _viewportYScale + 1f) / 2f;
        var worldY = Camera.main.ViewportToWorldPoint(new Vector3(0f, y)).y;

        var newVec = new Vector3(transform.position.x - _speed * Time.deltaTime, worldY);
        var diffVec = new Vector3(transform.position.x, transform.position.y) - newVec;

        var angle = Mathf.Atan2(diffVec.y, diffVec.x);

        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        transform.position = newVec;
	}
}
