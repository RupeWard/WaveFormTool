using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateGraphMenu : Menu 
{
	static readonly string functionsOption = "Functions";

	public void Awake ()
	{
		AddOption (functionsOption, true);
	}

	public override void OnOptionSelected(string option)
	{
		if (option == functionsOption)
		{
//			Debug.Log ("Menu '" + gameObject.name + "' selected option '" + functionsOption + "'");
			if ( WaveGeneratorPanel.IsInitialised())
			{
				bool b = WaveGeneratorPanel.Instance.gameObject.activeSelf;
//				Debug.Log((b)?("Closing"):("Opening")+ "Functions panel");
				WaveGeneratorPanel.Instance.SetActive(!b);
			}
			else
			{
				Debug.LogError ("WaveGeneratorPanel doesn't exist");
			}
		}
		else
		{
			Debug.LogWarning ("Menu '" + gameObject.name + "' doesn't know about option '" + option + "'");
		}
	}

}
