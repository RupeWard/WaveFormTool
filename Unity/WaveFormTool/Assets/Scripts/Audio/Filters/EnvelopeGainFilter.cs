using UnityEngine;
using System;

//http://www.develop-online.net/tools-and-tech/procedural-audio-with-unity/0117433

public class EnvelopeGainFilter : MonoBehaviour 
{
	private IEnvelopeProvider envelopeProvider_ = null;
	public IEnvelopeProvider EnvelopeProvider
	{
		get { return envelopeProvider_; }
		set { envelopeProvider_ = value; }
	}

	private static double s_increment_;
	private double currentTime_ = 0;
	private static double s_sampling_frequency = -1;

	public void Awake()
	{
		if (s_sampling_frequency == -1)
		{
			s_sampling_frequency = AudioSettings.outputSampleRate;
			s_increment_ = 1f / s_sampling_frequency;

			Debug.Log("Audio output sampling rate is "+s_sampling_frequency.ToString ());
		}
	}

	public void ResetTime()
	{
		currentTime_ = 0;
	}

	public void init(IEnvelopeProvider i)
	{
		envelopeProvider_ = i;
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
//		Debug.Log (data.Length + " samples in " + channels + " channels");

		if (envelopeProvider_ != null)
		{
			// update increment in case frequency has changed
			for (var i = 0; i < data.Length; i = i + channels)
			{
				currentTime_ = currentTime_ + s_increment_;
				
				data[i] *= envelopeProvider_.GetValueForTime((float)currentTime_, null); // TODO null settings?
				
				// if we have stereo, we copy the mono data to each channel
				if (channels == 2) data[i + 1] = data[i];
//				if (phase > 1f) phase = 0;
			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
	}
}
