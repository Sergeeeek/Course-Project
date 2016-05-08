using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Менеджер диалогов
/// </summary>
public class DialogManager : MonoBehaviour
{
    [Tooltip("Интервал между написанием каждого символа в строке")]
    public float _characterInterval;
    [Tooltip("Интервал между фразами в диалоге")]
    public float _phraseInterval;

    [Tooltip("Диалоговое окно для главного героя")]
    public GameObject _kunBox;
    [Tooltip("Диалоговое окно для помощника главного героя")]
    public GameObject _tyanBox;
    [Tooltip("Диалоговое окно для злодея")]
    public GameObject _burritoBox;

    [Tooltip("Картинка главного героя")]
    public Image _kunFace;
    [Tooltip("Картинка помощника главного героя")]
    public Image _tyanFace;
    [Tooltip("Картинка главного злодея")]
    public Image _burritoFace;

    [Tooltip("Строка для текста главного героя")]
    public Text _kunText;
    [Tooltip("Строка для текста помощника главного героя")]
    public Text _tyanText;
    [Tooltip("Строка для текста злодея")]
    public Text _burritoText;

    // Функция, которая будет вызыватся когда диалог завершится
    System.Action<string> _callback;

    // Текущий диалог
    Dialog _currentDialog;
    // Флаг говорящий о том что сейчас идёт диалог
    bool _dialogInProgress;

    // Текущее сообщение
    int _currentMessage;
    // Текущая фраза
    int _currentPhrase;
    // Текущий символ
    int _currentCharacter;
    // Флаг завершилась ли фраза
    bool _isPhraseFinished;
    // Время написания последнего символа
    float _lastCharacterTypeTime;
    // Время написания последней фразы
    float _lastPhraseTime;

    void Start()
    {
        // Сбрасываем диалог
        ResetDialog();
    }

    /// <summary>
    /// Публичная функция для начала диалога
    /// </summary>
    /// <param name="name">Название диалога</param>
    /// <param name="finishedCallback">Функция которая будет вызываться когда диалог завершится</param>
	public void StartDialog(string name, System.Action<string> finishedCallback = null)
	{
        // Сбрасываем текущий диалог
        ResetDialog();

        // Загружаем диалог с диска, все диалоги должны находится в папке Assets\Resources\Dialogs\
        var dialog = Resources.Load<Dialog>("Dialogs/" + name);

		if (dialog == null)
		{
			Debug.Log("Диалог с именем " + name + " не найден");
			return;
		}

        _currentDialog = dialog;
        _dialogInProgress = true;
        _callback = finishedCallback;

        // Подготавливаемся к первому сообщению
        SetupMessage();
	}

    // Функция сброса диалого
    void ResetDialog()
    {
        // Прячем все диалоговые окна
        _kunBox.SetActive(false);
        _tyanBox.SetActive(false);
        _burritoBox.SetActive(false);

        // Сбрасываем счётчики текущего символа, сообщенния и фразы
        _currentCharacter = 0;
        _currentMessage = 0;
        _currentPhrase = 0;
    }

    void Update()
    {
        // Если диалог не идёт
        if(_dialogInProgress == false)
        {
            return;
        }

        // skip будет хранить true если игрок нажал кнопку пропуска диалога (по умолчанию Enter)
        var skip = Input.GetButtonDown("SkipDialog");

        // Если фраза не завершена и пришло время печатать следующий символ
        if(Time.time - _lastCharacterTypeTime > _characterInterval && _isPhraseFinished == false)
        {
            // Получаем строку в интерфейсе для текущего персонажа
            Text currentText = GetText();
            // Получаем строку которую нужно вывести
            string curString = _currentDialog._messages[_currentMessage]._phrases[_currentPhrase]._text;

            // Если все символы строки выведены
            if(curString.Length == _currentCharacter + 1)
            {
                // То фраза завершена, запоминаем когда она завершилась
                _lastPhraseTime = Time.time;
                _isPhraseFinished = true;
            }
            else
            {
                // Печатаем следующий символ, запоминаем время последнего символа
                _currentCharacter += 1;
                currentText.text = curString.Substring(0, _currentCharacter + 1);
                _lastCharacterTypeTime = Time.time;
            }
        }
        else if(_isPhraseFinished == false && skip) // Если фраза не завершена и игрок хочет её пропустить
        {
            // Вывотим всю строку сразу
            var curString = _currentDialog._messages[_currentMessage]._phrases[_currentPhrase]._text;
            GetText().text = curString;
            _currentCharacter = curString.Length - 1;
        }
        
        // Если фраза завершена и пора начать новую или игрок хочет пропустить
        if(_isPhraseFinished && (Time.time - _lastPhraseTime > _phraseInterval || skip))
        {
            // Переключаемся на следующую фразу
            NextPhrase();
        }
    }

    void NextPhrase()
    {
        // Если текущая фраза последняя
        if(_currentPhrase + 1 == _currentDialog._messages[_currentMessage]._phrases.Count)
        {
            // Если текущее сообщение последнее
            if(_currentMessage + 1 == _currentDialog._messages.Count)
            {
                // Останавливаем диалог
                StopDialog();
                return;
            }

            // Запоминаем следующее сообщение и сбрасываем всё остальное
            var msg = _currentMessage + 1;
            ResetDialog();
            _currentMessage = msg;

            // Настраиваем интерфейс для нового сообщения
            SetupMessage();
        }
        else
        {
            // Переключаем фразу и начинаем с первого символа
            _currentPhrase++;
            _currentCharacter = 0;
        }

        // Сбрасываем текст в интерфейсе, сбрасываем флаг завершения фразы
        GetText().text = "";
        _isPhraseFinished = false;
    }

    void SetupMessage()
    {
        // Активируем диалоговое окно текущего персонажа
        GetBox().SetActive(true);
        // Устанавливаем картинку для персонажа из файла диалога
        GetImage().sprite = _currentDialog._messages[_currentMessage]._sprite;
    }

    public void StopDialog()
    {
        // Сбрасываем диалог
        ResetDialog();
        _dialogInProgress = false;

        if(_callback != null)
        {
            // Вызываем функцию по завершению
            _callback(_currentDialog.name);
            _callback = null;
        }
    }

    // Функция помощник, получает текущую текстовую строку в зависимости от персонажа
    Text GetText()
    {
        Text currentText = null;

        switch (_currentDialog._messages[_currentMessage]._speaker)
        {
            case Character.Kun:
                currentText = _kunText;
                break;
            case Character.Tyan:
                currentText = _tyanText;
                break;
            case Character.Burrito:
                currentText = _burritoText;
                break;
        }

        return currentText;
    }

    GameObject GetBox()
    {
        GameObject box = null;

        switch (_currentDialog._messages[_currentMessage]._speaker)
        {
            case Character.Kun:
                box = _kunBox;
                break;
            case Character.Tyan:
                box = _tyanBox;
                break;
            case Character.Burrito:
                box = _burritoBox;
                break;
        }

        return box;
    }

    Image GetImage()
    {
        Image img = null;

        switch (_currentDialog._messages[_currentMessage]._speaker)
        {
            case Character.Kun:
                img = _kunFace;
                break;
            case Character.Tyan:
                img = _tyanFace;
                break;
            case Character.Burrito:
                img = _burritoFace;
                break;
        }

        return img;
    }
}
