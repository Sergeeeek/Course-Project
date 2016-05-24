using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Перечисление всех возможных персонажей
/// </summary>
public enum Character
{
	Tyan,
	Kun,
	Burrito
}

/// <summary>
/// Структура для хранения "фразы", хранит текст и аудио
/// </summary>
[System.Serializable]
public struct Phrase
{
    [Tooltip("Текст который будет печататься")]
	public string _text;
}

/// <summary>
/// Сообщение, содержит информацию о персонаже, его иконку, а также список всех фраз
/// </summary>
[System.Serializable]
public struct Message
{
    [Tooltip("Кто говорит")]
	public Character _speaker;
    [Tooltip("Какую картинку показать в диалоге")]
    public Sprite _sprite;
    [Tooltip("Текст который будет отображаться")]
	public List<Phrase> _phrases;
}

/// <summary>
/// Этот класс отвечает за хранение диалога
/// ScriptableObject позволяет сохранять все данные этого класса в отдельный файл
/// для последующей его загрузки, также этот файл можно создавать и редактировать
/// в Inspector'е Unity
/// 
/// Атрибут CreateAssetMenu создаёт пункт в меню Unity для создания файла диалога
/// </summary>
[CreateAssetMenu()]
public class Dialog : ScriptableObject
{
	public List<Message> _messages;
}
