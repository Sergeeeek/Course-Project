using UnityEngine;

public class CameraMovementTest : MonoBehaviour
{
    public float _cameraSpeed = 1f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(x, y) * _cameraSpeed * Time.deltaTime);
    }
}
