using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogSystem : MonoBehaviour
{
	public List<Dialog> _dialog;

	int _currentDialogIndex;
	bool _inDialog;

	public void StartDialog(int index)
	{
		var dialog = _dialog[index];

		if (dialog == null)
		{
			Debug.Log("Диалог под индексом " + index.ToString () + " не найден");
		}
	}

	public void StartDialog(string name)
	{
		var dialog = _dialog.Find (x => x.name == name);

		if (dialog == null)
		{
			Debug.Log("Диалог с именем " + name + " не найден");
			return;
		}
	}
}
