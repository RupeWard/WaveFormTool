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

	public EnvelopeGraphPanel graphPanel;
	public UILabel messageLabel;
	public UISprite background_;

	private float leadInLength_ = 0.4f;
	private float leadInPeakValue_ = 1.5f;
	private float leadInPeakTime_ = 0.3f;
	private float tailOutLength_ = 0.8f;
	private float midLength_ = 0.3f;

	private Dictionary< string, EnvelopeGenerator > generatorDB_ = new Dictionary<string, EnvelopeGenerator>();
	private int numSamples_ = 32;
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
		tailOutLengthInput.text = tailOutLength_.ToString ();
		leadInLengthInput.text = leadInLength_.ToString ();
		leadInPeakValueInput.text = leadInPeakValue_.ToString ();
		leadInPeakTimeInput.text = leadInPeakTime_.ToString ();
		midLengthInput.text = midLength_.ToString ();

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

	public void OnLeadOutDurationInputChanged(string str)
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
				tailOutLength_ = newValue;
				Debug.Log ("TailOutDuration changed to "+tailOutLength_);
			}
		}
		else
		{
			messageLabel.text = "TailOut length must be a number!";
		}
		tailOutLengthInput.text = tailOutLength_.ToString ();
	}
		
	public void OnLeadInDurationInputChanged(string str)
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
				if (leadInPeakTime_ > newValue)
				{
					leadInPeakTime_ = newValue * leadInPeakTime_ / leadInLength_;
					leadInPeakTimeInput.text = leadInPeakTime_.ToString();
				}
				leadInLength_ = newValue;
				Debug.Log ("LeadInLength changed to "+leadInLength_);
			}
		}
		else
		{
			messageLabel.text = "LeadIn length must be a number!";
		}
		leadInLengthInput.text = leadInLength_.ToString ();
	}

	public void OnLeadInInputPeakValueChanged(string str)
	{
		float newValue;
		if (float.TryParse (str, out newValue))
		{
			if (newValue < 1f)
			{
				messageLabel.text = "LeadIn must have peak >=1";
			}
			else
			{
				leadInPeakValue_ = newValue;
				Debug.Log ("LeadInPeakValue changed to "+leadInPeakValue_);
			}
		}
		else
		{
			messageLabel.text = "LeadIn peak must be a number!";
		}
		leadInPeakValueInput.text = leadInPeakValue_.ToString ();
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
			else if (newValue > leadInLength_ )
			{
				messageLabel.text = "LeadIn time must be < length";
			}
			else
			{
				leadInPeakTime_ = newValue;
				Debug.Log ("LeadInPeakTime changed to "+leadInPeakTime_);
			}
		}
		else
		{
			messageLabel.text = "LeadIn peak time must be a number!";
		}
		leadInPeakTimeInput.text = leadInPeakTime_.ToString ();
	}

	public void OnMidDurationInputChanged(string str)
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
				midLength_ = newValue;
				Debug.Log ("MidDuration changed to "+midLength_);
			}
		}
		else
		{
			messageLabel.text = "Mid length must be a number!";
		}
		midLengthInput.text = midLength_.ToString ();
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
			graphPanel.CreateGraph (generatorDB_ [selected], numSamples_, false);
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
