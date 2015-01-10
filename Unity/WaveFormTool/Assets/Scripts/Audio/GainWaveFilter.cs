using UnityEngine;
using System;

//http://www.develop-online.net/tools-and-tech/procedural-audio-with-unity/0117433

public class GainWaveFilter : MonoBehaviour 
{
	private IWaveFormProvider waveFormProvider_ = null;
	private double frequency_;

	private double increment_;
	private double phase;
	private double sampling_frequency = 48000;

	public void init(IWaveFormProvider i, float f)
	{
		waveFormProvider_ = i;
		frequency_ = (double)f;
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
//		Debug.Log (data.Length + " samples in " + channels + " channels");

		if (waveFormProvider_ != null)
		{
			// update increment in case frequency has changed
			increment_ = frequency_ / sampling_frequency;
			for (var i = 0; i < data.Length; i = i + channels)
			{
				phase = phase + increment_;
				
				data[i] *= waveFormProvider_.GetValueForPhase((float)phase, WaveFormDataInterpolatorLinear.Instance);
				
				// if we have stereo, we copy the mono data to each channel
				if (channels == 2) data[i + 1] = data[i];
				if (phase > 1f) phase = 0;
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
