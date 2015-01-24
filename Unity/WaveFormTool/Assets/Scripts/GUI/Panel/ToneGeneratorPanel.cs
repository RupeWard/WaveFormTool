using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToneGeneratorPanel : SingletonSceneLifetime< ToneGeneratorPanel>  
{
	public UIInput frequencyInput;
	public UILabel messageLabel;
	public UILabel playButtonLabel;
	public UISprite background_;
	public WaveFormPlayer player_;
	public UILabel sourceLabel;

	public IWaveFormProvider waveFormProvider_ = null;

	private float frequency_ = 220f;
	private bool isPlaying = false;

	private static readonly float s_minFrequency = 0.00001f;
	private static readonly float s_maxFrequency = float.MaxValue;

	private static readonly string playStr = "Play";
	private static readonly string stopStr = "Stop";
	
	public void Start()
	{
//		HUDManager.Instance.AddPopup (gameObject);		

		frequencyInput.text = frequency_.ToString ();
//		this.gameObject.SetActive (false);
	}

	public Vector2 Size()
	{
		return new Vector2 ( background_.transform.localScale.x, background_.transform.localScale.y );
	}
	

	public void SetActive(bool b)
	{
		if (b)
		{
			HUDManager.Instance.CloseAllPopupsExcept(gameObject);
		}
		gameObject.SetActive (b);
	}

	public void SetWaveFormProvider(string s, IWaveFormProvider i)
	{
		sourceLabel.text = s;
		waveFormProvider_ = i;
		if (isPlaying)
		{
			player_.toneGenerator.init ( waveFormProvider_, frequency_);
			messageLabel.text = "Playing '" + waveFormProvider_.WaveFormName() + "' at "+frequency_.ToString();
		}
	}

	public void OnFrequencyInputChanged(string str)
	{
		float newFrequency;
		if (float.TryParse (str, out newFrequency))
		{
			if (newFrequency < s_minFrequency || newFrequency > s_maxFrequency)
			{
				messageLabel.text = "Freqency range ("+s_minFrequency+", "+s_maxFrequency+")";
			}
			else
			{
				frequency_ = newFrequency;
				if (isPlaying)
				{
					player_.toneGenerator.init ( waveFormProvider_, frequency_);
					messageLabel.text = "Playing '" + waveFormProvider_.WaveFormName() + "' at "+frequency_.ToString();
				}
			}
		}
		else
		{
			messageLabel.text = "Frequency has to be a number";
		}
		frequencyInput.text = frequency_.ToString ();
	}

	public void OnGenerateToneButtonClicked()
	{
//		Debug.Log ("Generate clicked");
		if (isPlaying)
		{
			player_.audio.Stop();
			messageLabel.text = "";
			isPlaying = false;
		}
		else
		{
			if (waveFormProvider_ != null)
			{
				player_.toneGenerator.init ( waveFormProvider_, frequency_);
				player_.audio.Play();
				messageLabel.text = "Playing '" + waveFormProvider_.WaveFormName() + "' at "+frequency_.ToString();
				isPlaying = true;
			}
			else
			{
				messageLabel.text = "No source defined";
			}
		}
		SetPlayButtonLabel();
	}

	public void OnCloseButtonClicked()
	{
//		Debug.Log ("Close clicked");
		this.gameObject.SetActive (false);
	}

	public void OnEnable()
	{
		SetPlayButtonLabel ();
	}

	private void SetPlayButtonLabel()
	{
		playButtonLabel.text = (player_.audio.isPlaying) ? (stopStr) : (playStr);
	}
}
