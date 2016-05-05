using UnityEngine;
using System.Collections.Generic;

public enum Character
{
	Tyan,
	Kun,
	Burrito
}

[System.Serializable]
public struct Phrase
{
	public string _text;
	public AudioClip _audio;
}

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

[CreateAssetMenu()]
public class Dialog : ScriptableObject
{
	public List<Message> _messages;
}
