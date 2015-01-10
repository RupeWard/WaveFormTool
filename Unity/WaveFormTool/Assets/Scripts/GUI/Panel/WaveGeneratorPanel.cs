using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveGeneratorPanel : SingletonSceneLifetime< WaveGeneratorPanel >  
{
	public UIPopupList generatorTypeList;
	public UIInput numSamplesInput;
	public GraphPanel graphPanel;
	public UILabel messageLabel;
	public UISprite background_;

	private Dictionary< string, WaveFormGenerator > generatorDB_ = new Dictionary<string, WaveFormGenerator>();
	private int numSamples_ = 32;
	private static readonly int s_minNumSamples = 4;
	private static readonly int s_maxNumSamples = 1024;

	public void Start()
	{
		GuiManager.Instance.AddPopup (gameObject);		

		generatorDB_.Add ("Sine (built in)", new WaveFormGeneratorSine ());
		generatorDB_.Add ("Sawtooth (built in)", new WaveFormGeneratorSaw ());

		foreach (string key in generatorDB_.Keys)
		{
			generatorTypeList.items.Add (key);
		}

		numSamplesInput.text = numSamples_.ToString ();

//		this.gameObject.SetActive (false);
	}

	public Vector2 Size()
	{
		return new Vector2 ( background_.transform.localScale.x, background_.transform.localScale.y );
	}

	// TODO BASE CLASS
	public void SetActive(bool b)
	{
		if (b)
		{
			GuiManager.Instance.CloseAllPopupsExcept(gameObject);
		}
		gameObject.SetActive (b);
	}
	

	public void OnGeneratorTypeSelectionChange(string newType)
	{
		// might need to do stuff here for other types
		/*
		if (generatorDB_.ContainsKey (newType))
		{
			Debug.Log ("Type changed to " + newType);
		}
		else
		{
			Debug.LogError ("No such generator type: "+newType);
		}
		*/
	}

	public void OnNumSamplesInputChanged(string str)
	{
		int newNumSamples;
		if (int.TryParse (str, out newNumSamples))
		{
			if (newNumSamples < s_minNumSamples || newNumSamples > s_maxNumSamples)
			{
				messageLabel.text = "Num Samples range ("+s_minNumSamples+", "+s_maxNumSamples+")";
			}
			else
			{
				numSamples_ = newNumSamples;
			}
		}
		else
		{
			messageLabel.text = "Num Samples has to be an integer!";
		}
		numSamplesInput.text = numSamples_.ToString ();
	}

	public void OnGenerateGraphButtonClicked()
	{
		string selected = generatorTypeList.selection;
//		Debug.Log ("GenerateGraphButton clicked with selection = '"+selected+"'");
		if (generatorDB_.ContainsKey (selected))
		{
			graphPanel.CreateGraph (generatorDB_ [selected], numSamples_, false);
			ToneGeneratorPanel.Instance.SetWaveFormProvider(generatorDB_[selected]);
		}
		else
		{
			Debug.LogError ("No such option");
		}
	}

	public void OnCloseButtonClicked()
	{
//		Debug.Log ("Close clicked");
		this.gameObject.SetActive (false);
	}
}
