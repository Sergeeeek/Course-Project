using UnityEngine;

public class SimpleEnemyController : MonoBehaviour
{
    [Tooltip("Скорость движения корабля")]
	public float _speed;
    [Tooltip("Скорость увеличения параметра x для функции sin")]
    public float _sinSpeed;
    [Tooltip("Масштаб параметра x для функции sin, увеличивает или снижает период колебаний")]
    public float _sinScale = 1f;
    [Tooltip("Сдвиг параметра x для функции sin при создании объекта")]
    public float _xOffset;
    [Tooltip("Масштаб координат корабля в системе координат экрана.\nУменьшить если корабли выходят за края экрана.")]
    public float _viewportYScale = 0.9f;

    // Текущий параметр для sin(x)
    float x;

    void Start()
    {
        // Определяем начальные координаты по y для получения соответсвующего x по формуле x=arcsin(y)
        // Чтобы получить координаты y - [-1;1] нужно перевести позицию объекта из координат мира в координаты
        // Viewport'а, в которых нижний левый край экрана имеет координаты (0,0), а верхний правый (1,1).
        // Затем y находится по формуле y=y' * 2 - 1, где y' - [0;1]
        var _startY = Camera.main.WorldToViewportPoint(transform.position).y * 2f - 1f;
        x = Mathf.Asin(_startY) + _xOffset;
    }

	void Update()
	{
        x += _sinSpeed * Time.deltaTime;

        // Чтобы получить мировые координаты из координат Viewport'а нужно провести обратный процесс
        // Сначала расчитывается y'=sin(x). Затем чтобы перевести координаты из y' - [-1;1]
        // в y - [0;1], которые нужны для перевода обратно в мировые координаты
        // используется формула y=(y'+1)/2
        var y = (Mathf.Sin(x * _sinScale) * _viewportYScale + 1f) / 2f;
        var worldY = Camera.main.ViewportToWorldPoint(new Vector3(0f, y)).y;

        // Расчёт новой позиции
        var newVec = new Vector3(transform.position.x - _speed * Time.deltaTime, worldY);
        // Разность векторов для нахождения угла
        var diffVec = new Vector3(transform.position.x, transform.position.y) - newVec;

        // Находим угол вектора через arctan отношения y и x
        var angle = Mathf.Atan2(diffVec.y, diffVec.x);

        // Устанавливаем новую позицию и ротацию объекта
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        transform.position = newVec;
	}
}
