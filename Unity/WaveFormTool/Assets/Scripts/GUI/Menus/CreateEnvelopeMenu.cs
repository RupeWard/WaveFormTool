using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateEnvelopeMenu : Menu 
{
	static readonly string functionsOption = "Functions";
	static readonly string clearOption = "Clear";

	public EnvelopeGraphPanel envelopeGraphPanel;

	public void Awake ()
	{
		AddOption (functionsOption, true);
		AddOption (clearOption, true);
	}

	public override void OnOptionSelected(string option)
	{
		if (option == functionsOption)
		{
//			Debug.Log ("Menu '" + gameObject.name + "' selected option '" + functionsOption + "'");
			if (EnvelopeGeneratorPanel.IsInitialised ())
			{
				bool b = EnvelopeGeneratorPanel.Instance.gameObject.activeSelf;
//				Debug.Log((b)?("Closing"):("Opening")+ "Functions panel");
				EnvelopeGeneratorPanel.Instance.SetActive (!b);
			}
			else
			{
				Debug.LogError ("EnvelopeGeneratorPanel doesn't exist");
			}
		}
		else if (option == clearOption)
		{
			envelopeGraphPanel.ClearGraph();
		}
		else
		{
			Debug.LogWarning ("Menu '" + gameObject.name + "' doesn't know about option '" + option + "'");
		}
	}

}
