using UnityEngine; // Большинство классов и функций Unity
using System.Collections.Generic; // Включено для использования класса List<T>

/// <summary>
/// Этот компонент обрабатывает ввод игрока и превращает его в движение корабля
/// </summary>
public class ShipController : MonoBehaviour
{
    // Список всех орудий на корабле
	List<IGun> _guns;

    [Tooltip("Ускорение")]
	public float _acceleration;
    [Tooltip("Максимальная скорость")]
	public float _maxSpeed;
    [Tooltip("Торможение когда игрок ничего не нажимает")]
	public float _drag;

	Vector3 _currentVelocity; // Текущая скорость

	Vector3 _objectSize; // Размер корабля

	void Start()
	{
        _guns = new List<IGun>();
        // Автоматически сканируем и создаём список всех орудий из под-объектов (или "детей") этого игрового объекта
        var comps = GetComponentsInChildren(typeof(IGun));

        foreach(var comp in comps)
        {
            _guns.Add(comp as IGun);
        }

        // Размер корабля равен размеру спрайта (картинки) корабля из компонента SpriteRenderer
		_objectSize = GetComponent<SpriteRenderer>().bounds.size;
	}

	void Update()
	{
		Vector3 input;

        // Если есть поддержка сенсорного экрана
		if(Input.touchSupported)
		{
            // Если кол-во нажатий = 0
			if(Input.touchCount == 0)
				return;

            // Получаем данные о первом нажатии
			var t = Input.GetTouch(0);
            // Переводим координаты нажатия из системы координат экрана в мировую систему координат
			Vector3 _targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y));

            // Вектор направления от текущей позиции к точке нажатия
			input = _targetPosition - transform.position;

		}
		else
		{
            // Получаем горизонтальную ось ввода (A-D на клавиатуре)
			var xInput = Input.GetAxisRaw("Horizontal");
            // Получаем вертикальную ось ввода (W-S на клавиатуре)
			var yInput = Input.GetAxisRaw("Vertical");

            // Объединяем гориз. и верт. ввод в один вектор
			input = new Vector3(xInput, yInput);

			var drag = new Vector3();

            // Если x примерно равен 0
			if(Mathf.Approximately(0, input.x))
			{
                // Добавляем замедление к кораблю
				drag.x = -_currentVelocity.x * _drag;
			}
			if(Mathf.Approximately(0, input.y))
			{
				drag.y = -_currentVelocity.y * _drag;
			}

            // Добавляем торможение к скорости
			_currentVelocity += drag;
		}

		input.Normalize(); // Нормализуем вектор ввода, после этого его длина становится равна 1
		_currentVelocity += _acceleration * input * Time.deltaTime; // Прибавляем ускорение в нужном направлении к скорости

        // Если скорость выше максимальной
		if(_currentVelocity.magnitude >= _maxSpeed)
		{
            // Ограничиваем её
			_currentVelocity *= _maxSpeed / _currentVelocity.magnitude;
		}

        // Если скорость примерно 0
		if(Mathf.Approximately(0, _currentVelocity.magnitude))
		{
            // То скорость = (0, 0, 0)
			_currentVelocity = Vector3.zero;
		}

        // Передвигаем этот объект с текущей скоростью в мировых координатах
		transform.Translate(_currentVelocity * Time.deltaTime, Space.World);

        // Ограничиваем передвижение корабля границами экрана
		keepOnScreen();

        // Если кнопка ShipFire (пробел) нажата или устройство имеет сенсорный экран (например смартфон)
		var shooting = Input.GetButton("ShipFire") || Input.touchSupported;
        // То всё оружие на корабле должно стрелять
		_guns.ForEach(x => x._shooting = shooting);
	}

    // Функция для ограничения движения корабля границами экрана
	void keepOnScreen()
	{
        // Позиция нижнего левого и правого верхнего угла камеры в мировых координатах
		var bottomleftCam = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
		var toprightCam = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));

        // запоминаем текущую z позицию корабля
		var z = transform.position.z;

		var size = _objectSize;

        // Проверяем вышел ли корабль за границы экрана
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
