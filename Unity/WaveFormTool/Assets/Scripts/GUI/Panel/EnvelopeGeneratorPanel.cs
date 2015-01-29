using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvelopeGeneratorPanel : SingletonSceneLifetime< EnvelopeGeneratorPanel >  
{
	public UIPopupList generatorTypeList;

	public UIInput numSamplesInput;
	public UIInput leadInLengthInput;
	public UIInput leadInPeakValueInput;
	public UIInput leadInPeakTimeInput;
	public UIInput tailOutLengthInput;
	public UIInput midLengthInput;
	public UIInput midValueInput;

	public EnvelopeGraphPanel graphPanel;
	public UILabel messageLabel;
	public UISprite background_;

	private BasicEnvelopeSettings settings_ = new BasicEnvelopeSettings();

	private Dictionary< string, EnvelopeGenerator > generatorDB_ = new Dictionary<string, EnvelopeGenerator>();
	private int numSamples_ = 8;
	private static readonly int s_minNumSamples = 4;
	private static readonly int s_maxNumSamples = 1024;

	public void Start()
	{
//		HUDManager.Instance.AddPopup (gameObject);		

		generatorDB_.Add ("Sine (built in)", new EnvelopeGeneratorSine ());
		generatorDB_.Add ("Sawtooth (built in)", new EnvelopeGeneratorSaw ());

		foreach (string key in generatorDB_.Keys)
		{
			generatorTypeList.items.Add (key);
		}

		numSamplesInput.text = numSamples_.ToString ();
		tailOutLengthInput.text = settings_.tailOutLength.ToString ();
		leadInLengthInput.text = settings_.leadInLength.ToString ();
		leadInPeakValueInput.text = settings_.leadInPeakValue.ToString ();
		leadInPeakTimeInput.text = settings_.leadInPeakTime.ToString ();
		midLengthInput.text = settings_.midLength.ToString ();
		midValueInput.text = settings_.midValue.ToString ();

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
			HUDManager.Instance.CloseAllPopupsExcept(gameObject);
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

	public void OnLeadOutLengthInputChanged(string str)
	{
		float newValue;
		if (float.TryParse (str, out newValue))
		{
			if (newValue <=0f)
			{
				messageLabel.text = "TailOut must have length";
			}
			else
			{
				settings_.tailOutLength = newValue;
				Debug.Log ("TailOutDuration changed to "+settings_.tailOutLength);
			}
		}
		else
		{
			messageLabel.text = "TailOut length must be a number!";
		}
		tailOutLengthInput.text = settings_.tailOutLength.ToString ();
	}
		
	public void OnLeadInLengthInputChanged(string str)
	{
		float newValue;
		if (float.TryParse (str, out newValue))
		{
			if (newValue <=0f)
			{
				messageLabel.text = "LeadIn must have length";
			}
			else
			{
				if (settings_.leadInPeakTime > newValue)
				{
					settings_.leadInPeakTime = newValue * settings_.leadInPeakTime / settings_.leadInLength;
					leadInPeakTimeInput.text = settings_.leadInPeakTime.ToString();
				}
				settings_.leadInLength = newValue;
				Debug.Log ("LeadInLength changed to "+settings_.leadInLength);
			}
		}
		else
		{
			messageLabel.text = "LeadIn length must be a number!";
		}
		leadInLengthInput.text = settings_.leadInLength.ToString ();
	}

	public void OnLeadInInputPeakValueChanged(string str)
	{
		float newValue;
		if (float.TryParse (str, out newValue))
		{
			if (newValue < settings_.midValue)
			{
				messageLabel.text = "LeadIn must have peak >= "+settings_.midValue;
			}
			else
			{
				settings_.leadInPeakValue = newValue;
				Debug.Log ("LeadInPeakValue changed to "+settings_.leadInPeakValue);
			}
		}
		else
		{
			messageLabel.text = "LeadIn peak must be a number!";
		}
		leadInPeakValueInput.text = settings_.leadInPeakValue.ToString ();
	}

	public void OnLeadInPeakTimeInputChanged(string str)
	{
		float newValue;
		if (float.TryParse (str, out newValue))
		{
			if (newValue <= 0f )
			{
				messageLabel.text = "LeadIn must have peak time > 0";
			}
			else if (newValue >= settings_.leadInLength )
			{
				messageLabel.text = "LeadIn time must be < length";
			}
			else
			{
				settings_.leadInPeakTime = newValue;
				Debug.Log ("LeadInPeakTime changed to "+settings_.leadInPeakTime);
			}
		}
		else
		{
			messageLabel.text = "LeadIn peak time must be a number!";
		}
		leadInPeakTimeInput.text = settings_.leadInPeakTime.ToString ();
	}

	public void OnMidLengthInputChanged(string str)
	{
		float newValue;
		if (float.TryParse (str, out newValue))
		{
			if (newValue < 0f)
			{
				messageLabel.text = "Duration can't be negative";
			}
			else
			{
				settings_.midLength = newValue;
				Debug.Log ("MidDuration changed to "+settings_.midLength);
			}
		}
		else
		{
			messageLabel.text = "Mid length must be a number!";
		}
		midLengthInput.text = settings_.midLength.ToString ();
	}


	public void OnMidValueInputChanged(string str)
	{
		float newValue;
		if (float.TryParse (str, out newValue))
		{
			if (newValue <= 0f)
			{
				messageLabel.text = "Midvalue must be positive";
			}
			else if (newValue > settings_.leadInPeakValue)
			{
				messageLabel.text = "Midvalue can't exceed peak";
			}
			else
			{
				settings_.midValue = newValue;
				Debug.Log ("MidValue changed to "+settings_.midValue);
			}
		}
		else
		{
			messageLabel.text = "Mid value must be a number!";
		}
		midValueInput.text = settings_.midValue.ToString ();
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
				Debug.Log ("NumSamples changed to "+numSamples_);
			}
		}
		else
		{
			messageLabel.text = "Num Samples must be an integer!";
		}
		numSamplesInput.text = numSamples_.ToString ();
	}

	public void OnGenerateGraphButtonClicked()
	{
		string selected = generatorTypeList.selection;
//		Debug.Log ("GenerateGraphButton clicked with selection = '"+selected+"'");
		if (generatorDB_.ContainsKey (selected))
		{
			graphPanel.CreateGraph (generatorDB_ [selected], numSamples_, settings_, false);
		}
		else
		{
			Debug.LogError ("No such option");
		}
	}

	public void OnPlayButtonClicked()
	{
		string selected = generatorTypeList.selection;
		//		Debug.Log ("GenerateGraphButton clicked with selection = '"+selected+"'");
		if (generatorDB_.ContainsKey (selected))
		{
			Debug.LogWarning ("Not implemented");
//			ToneGeneratorPanel.Instance.SetWaveFormProvider(selected, generatorDB_[selected]);
//			ToneGeneratorPanel.Instance.SetActive(true);
		}
		else
		{
			Debug.LogError ("No such option");
		}
	}

	public void OnCloseButtonClicked()
	{
		this.gameObject.SetActive (false);
	}
}
