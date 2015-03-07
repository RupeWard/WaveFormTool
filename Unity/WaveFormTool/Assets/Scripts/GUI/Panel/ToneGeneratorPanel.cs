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
	public UICheckbox envelopeCheckbox;

	public UIPopupList modeSelection;

	public IWaveFormProvider waveFormProvider_ = null;
	public IEnvelopeProvider envelopeProvider_ = null;

	public UICheckbox envCheckBox;

	private float frequency_ = 220f;
	private bool isPlaying = false;

	private static readonly float s_minFrequency = 0.00001f;
	private static readonly float s_maxFrequency = float.MaxValue;

	private static readonly string playStr = "Play";
	private static readonly string stopStr = "Stop";

	private bool useEnvelope_ =  false;
	public bool UseEnvelope
	{
		get { return useEnvelope_; }
	}

	private static readonly string constantMode = "Constant";
	private static readonly string repeatMode = "Repeat";
	private static readonly string singleMode = "Single";

	public void Start()
	{
//		HUDManager.Instance.AddPopup (gameObject);		

		frequencyInput.text = frequency_.ToString ();
		this.gameObject.SetActive (false);

//		modeSelection.items.Add ( );
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
			if (waveFormProvider_ == null)
			{
				player_.audio.Stop();
			}
			else
			{
				messageLabel.text = "Playing '" + waveFormProvider_.WaveFormName() + "' at "+frequency_.ToString();
			}
			player_.toneGenerator.init ( waveFormProvider_, frequency_);
		}
		SetupModeSelecter();
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

	public void OnPlayButtonClicked()
	{
		StartCoroutine ( PlayCR());
	}

	private IEnumerator PlayCR()
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

				if (useEnvelope_)
				{
					if (player_.envelopeGain.EnvelopeProvider == null)
					{
						Debug.LogError ("Player has no envelope provider, disabling");
						player_.envelopeGain.enabled = false;
					}
					else
					{
						if (player_.envelopeGain.enabled == false)
						{
							Debug.LogWarning ("Enabling envelope gain");
							player_.envelopeGain.enabled = true;
						}
						player_.envelopeGain.ResetTime();
					}
				}
				player_.audio.Play();
				messageLabel.text = "Playing '" + waveFormProvider_.WaveFormName() + "' at "+frequency_.ToString();
				SetPlayButtonLabel();
				yield return null;

				isPlaying = true;
				if (useEnvelope_ )
				{
					yield return new WaitForSeconds( envelopeProvider_.EnvelopeLength());
					player_.audio.Stop();
					string msg = "Finished Playing ";
					messageLabel.text = msg;
					 
					int numBufs;
					int bufLen;
					AudioSettings.GetDSPBufferSize(out bufLen, out numBufs);
					Debug.Log (msg+player_.envelopeGain.numCalls
					           +" calls of "+bufLen+" ("+numBufs+"), received "+player_.envelopeGain.numData
					           +" in "+player_.envelopeGain.numChannels+" channels");
					player_.envelopeGain.ResetTime();
				}
			}
			else
			{
				messageLabel.text = "No source defined";
			}
		}
		SetPlayButtonLabel();
		yield return null;
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
		playButtonLabel.text = (player_!= null && player_.audio.isPlaying) ? (stopStr) : (playStr);
	}

	public void OnEnvCheckBox(bool b)
	{
		Debug.Log ( "UseEnvelope = " + b );
		SetUseEnvelope(b);
	}

	private void SetUseEnvelope(bool b)
	{
		useEnvelope_ = b;
		SetupModeSelecter();
		player_.envelopeGain.enabled = useEnvelope_;
		if ( useEnvelope_ )
		{
			if (envelopeProvider_ == null)
			{
				Debug.Log ("Null envelope on SetUseEnvelope " + b );
				messageLabel.text = "No envelope to use";
				envelopeCheckbox.isChecked = false;
				player_.envelopeGain.init (null);
			}
			else if (false == envelopeProvider_.IsReady())
			{
				Debug.Log ("Envelope not ready on SetUseEnvelope " + b );
				messageLabel.text = "Envelope not ready to use";
				envelopeCheckbox.isChecked = false;
				player_.envelopeGain.init (null);
			}
			else
			{
				player_.envelopeGain.init(envelopeProvider_.Clone());
			}
		}
	}

	public void ToggleEnv()
	{
		if ( !useEnvelope_ && !envelopeProvider_.IsReady ( ) )
		{
			messageLabel.text = "Can't use Envelope, the graph isn't ready";
		}
		else
		{
			SetEnv ( !useEnvelope_ );
		}
	}

	private void SetEnv(bool b)
	{
		if ( useEnvelope_ != b )
		{
			envCheckBox.isChecked = ! envCheckBox.isChecked;
		}
	}

	private void SetupModeSelecter()
	{
		modeSelection.items.Clear ( );
		if ( waveFormProvider_ != null )
		{
			if (useEnvelope_)
			{
				modeSelection.items.Add(repeatMode);
				modeSelection.items.Add(singleMode);
			}
			else
			{
				modeSelection.items.Add(constantMode);
			}
		}
	}
}
