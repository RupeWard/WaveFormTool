using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateBaseWaveMenu : Menu 
{
	static readonly string functionsOption = "Functions";
	static readonly string clearOption = "Clear";
	static readonly string saveOption = "Save";
	static readonly string loadOption = "Load";

	public WaveGraphPanel waveGraphPanel;

	public void Awake ()
	{
		AddOption (functionsOption, true);
		AddOption (clearOption, true);
		AddOption (loadOption, true);
		AddOption (saveOption, true);
	}

	public override void OnOptionSelected(string option)
	{
		if ( option == functionsOption )
		{
//			Debug.Log ("Menu '" + gameObject.name + "' selected option '" + functionsOption + "'");
			if ( WaveGeneratorPanel.IsInitialised ( ) )
			{
				bool b = WaveGeneratorPanel.Instance.gameObject.activeSelf;
//				Debug.Log((b)?("Closing"):("Opening")+ "Functions panel");
				WaveGeneratorPanel.Instance.SetActive ( !b );
			}
			else
			{
				Debug.LogError ( "WaveGeneratorPanel doesn't exist" );
			}
		}
		else if ( option == clearOption )
		{
			waveGraphPanel.ClearGraph ( );
		}
		else if ( option == saveOption )
		{
			SaveGraphPanel.Instance.Init(waveGraphPanel);
			SaveGraphPanel.Instance.SetActive(true);
		}
		else if ( option == loadOption )
		{
//			SaveGraphPanel.Instance.Init(waveGraphPanel);
//			SaveGraphPanel.Instance.SetActive(true);
		}
		else
		{
			Debug.LogWarning ("Menu '" + gameObject.name + "' doesn't know about option '" + option + "'");
		}
	}

}
