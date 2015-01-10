using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MakeToneMenu : Menu 
{
	static readonly string playGraphOption = "PlayGraph";

	public void Awake ()
	{
		AddOption (playGraphOption, true);
	}

	public override void OnOptionSelected(string option)
	{
		if (option == playGraphOption)
		{
			Debug.Log ("Menu '" + gameObject.name + "' selected option '" + playGraphOption + "'");
			if ( ToneGeneratorPanel.IsInitialised())
			{
				bool b = ToneGeneratorPanel.Instance.gameObject.activeSelf;
				Debug.Log((b)?("Closing"):("Opening")+ "PlayGraph panel");
				ToneGeneratorPanel.Instance.SetActive(!b);
			}
			else
			{
				Debug.LogError ("ToneGeneratorPanel doesn't exist");
			}
		}
		else
		{
			Debug.LogWarning ("Menu '" + gameObject.name + "' doesn't know about option '" + playGraphOption + "'");
		}
	}

}
