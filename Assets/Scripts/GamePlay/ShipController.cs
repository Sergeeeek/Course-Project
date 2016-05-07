using UnityEngine; // Большинство классов и функций Unity
using System.Collections.Generic; // Включено для использования класса List<T>

/// <summary>
/// Этот компонент обрабатывает ввод игрока и превращает его в движение корабля
/// </summary>
public class ShipController : MonoBehaviour
{
    // Список всех орудий на корабле
	List<IGun> _guns;

	public float _acceleration; // Ускорение
	public float _maxSpeed; // Максимальная скорость
	public float _drag; // "Сопротивление воздуха" - скорость остановки корабля когда игрок ничего не нажимает

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

			_currentVelocity += drag;
		}

		input.Normalize(); // Нормализуем вектор ввода, после этого его длина становится равна 1
		_currentVelocity += _acceleration * input * Time.deltaTime; // 

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

		var shooting = Input.GetButton("ShipFire") || Input.touchSupported;
		_guns.ForEach(x => x._shooting = shooting);
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
