using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DialogManager : MonoBehaviour
{
    public float _characterInterval;
    public float _phraseInterval;

    public GameObject _kunBox;
    public GameObject _tyanBox;
    public GameObject _burritoBox;

    public Image _kunFace;
    public Image _tyanFace;
    public Image _burritoFace;

    public Text _kunText;
    public Text _tyanText;
    public Text _burritoText;

    System.Action<string> _callback;

    Dialog _currentDialog;
    bool _dialogInProgress;

    int _currentMessage;
    int _currentPhrase;
    int _currentCharacter;
    bool _isPhraseFinished;
    float _lastCharacterTypeTime;
    float _lastPhraseTime;

    void Start()
    {
        ResetDialog();
    }

	public void StartDialog(string name, System.Action<string> finishedCallback = null)
	{
        ResetDialog();

        var dialog = Resources.Load<Dialog>("Dialogs/" + name);

		if (dialog == null)
		{
			Debug.Log("Диалог с именем " + name + " не найден");
			return;
		}

        _currentDialog = dialog;
        _dialogInProgress = true;
        _callback = finishedCallback;

        SetupMessage();
	}

    void ResetDialog()
    {
        _kunBox.SetActive(false);
        _tyanBox.SetActive(false);
        _burritoBox.SetActive(false);

        _currentCharacter = 0;
        _currentMessage = 0;
        _currentPhrase = 0;
    }

    void Update()
    {
        if(_dialogInProgress == false)
        {
            return;
        }

        var skip = Input.GetButtonDown("SkipDialog");

        if(Time.time - _lastCharacterTypeTime > _characterInterval && _isPhraseFinished == false)
        {
            var currentText = GetText();
            var curString = _currentDialog._messages[_currentMessage]._phrases[_currentPhrase]._text;

            if(curString.Length == _currentCharacter + 1)
            {
                _lastPhraseTime = Time.time;
                _isPhraseFinished = true;
            }
            else
            {
                _currentCharacter += 1;
                currentText.text = curString.Substring(0, _currentCharacter + 1);
                _lastCharacterTypeTime = Time.time;
            }
        }
        else if(_isPhraseFinished == false && skip)
        {
            var curString = _currentDialog._messages[_currentMessage]._phrases[_currentPhrase]._text;
            GetText().text = curString;
            _currentCharacter = curString.Length - 1;
        }
        
        if(_isPhraseFinished && (Time.time - _lastPhraseTime > _phraseInterval || skip))
        {
            NextPhrase();
        }
    }

    void NextPhrase()
    {
        // Если последняя фраза
        if(_currentPhrase + 1 == _currentDialog._messages[_currentMessage]._phrases.Count)
        {
            if(_currentMessage + 1 == _currentDialog._messages.Count)
            {
                StopDialog();
                return;
            }

            var msg = _currentMessage + 1;
            ResetDialog();
            _currentMessage = msg;

            SetupMessage();
        }
        else
        {
            _currentPhrase++;
        }

        GetText().text = "";
        _isPhraseFinished = false;
    }

    void SetupMessage()
    {
        GetBox().SetActive(true);
        GetImage().sprite = _currentDialog._messages[_currentMessage]._sprite;
    }

    public void StopDialog()
    {
        ResetDialog();
        _dialogInProgress = false;

        if(_callback != null)
        {
            _callback(_currentDialog.name);
            _callback = null;
        }
        
        gameObject.SetActive(false);
    }

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
