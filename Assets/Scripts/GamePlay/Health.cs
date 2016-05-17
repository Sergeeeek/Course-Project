using UnityEngine;

public class Health : MonoBehaviour
{
    [Tooltip("Макс. уровень здоровья")]
	public float _maxHealth = 100f;
    [Tooltip("Префаб который будет создаваться когда этот объект погибнет")]
	public GameObject _deathEffect;
    [Tooltip("Регенерировать здоровье")]
	public bool _regenHealth;
    [Tooltip("Сколько здоровья восстанавливать в секунду")]
	public float _regenAmountPerSecond;
    [Tooltip("Время после последнего нанесения урона до начала регенерации в сек.")]
	public float _timeBeforeRegen;

    [Tooltip("Множитель времени тряски камеры в секундах при нанесении урона.\nКол-во урона * множитель = время тряски.")]
    public float _shakeDurationMultiplier;
    [Tooltip("Множитель силы тряски камеры при нанесении урона.\nКол-во урона * множитель = сила тряски.")]
    public float _shakeIntensityMultiplier;

    // Время последнего нанесения урона
	float _lastDamageTime;
    // Текущий уровень здоровья
    float _health;

    // Указатель на компонент тряски камеры
    CameraShake _shake;

    void Start()
    {
        // Находим компонент тряски для будущего использования
        _shake = FindObjectOfType<CameraShake>();

        // В начале здоровье максимально
        _health = _maxHealth;
    }

	void Update()
	{
		if (_regenHealth)
		{
            // Если прошло _timeBeforeRegen сек. с предыдущего урона
			if (Time.time - _lastDamageTime > _timeBeforeRegen)
			{
                // Здоровье = математический минимум между максимальным здоровьем и здоровьем после регенерации
				_health = Mathf.Min(_health + _regenAmountPerSecond * Time.deltaTime, _maxHealth);
			}
		}
	}

    /// <summary>
    /// Публичная функция для изменения здоровья из вне
    /// </summary>
    /// <param name="deltaHealth">сколько здоровья прибавить</param>
	public void ChangeHealth(float deltaHealth)
	{
		_health += deltaHealth;

        // Если нанесён урон
		if (deltaHealth < 0)
		{
            // Трясём камеру
            _shake.Shake(_shakeDurationMultiplier * deltaHealth, _shakeIntensityMultiplier * deltaHealth);
            // Запоминаем время последнего нанесения урона
			_lastDamageTime = Time.time;
		}

		if(_health <= 0f)
		{
            // Если префаб эффекта назначен
            if(_deathEffect != null)
            {
                // Создаём его
                var obj = Instantiate(_deathEffect);
                // Ставим его в положение текущего объекта
                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;
            }
			
            // Уничтожаем текущий объект
			Destroy(gameObject);
		}
	}
}
