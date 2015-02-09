using UnityEngine;
using System;

//http://www.develop-online.net/tools-and-tech/procedural-audio-with-unity/0117433

public class BaseWaveGeneratorFilter : MonoBehaviour 
{
	private IWaveFormProvider waveFormProvider_ = null;
	private double frequency_;

	private double increment_;
	private double phase;

	private static double s_sampling_frequency = -1;

	public void Awake()
	{
		if (s_sampling_frequency == -1)
		{
			s_sampling_frequency = AudioSettings.outputSampleRate;
			Debug.Log("Audio output sampling rate is "+s_sampling_frequency.ToString ());
		}
	}

	public void init(IWaveFormProvider i, float f)
	{
		waveFormProvider_ = i;
		frequency_ = (double)f;
	}

	public void SetFrequency(float f)
	{
		frequency_ = (double)f;
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
//		Debug.Log (data.Length + " samples in " + channels + " channels");

		if (waveFormProvider_ != null)
		{
			increment_ = frequency_ / s_sampling_frequency;
			for (var i = 0; i < data.Length; i = i + channels)
			{
				phase = phase + increment_;
				
				data[i] = waveFormProvider_.GetValueForPhase((float)phase, WaveFormDataInterpolatorLinear.Instance);
				
				// if we have stereo, we copy the mono data to each channel
				if (channels == 2) data[i + 1] = data[i];
				if (phase > 1f) phase = 0;
			}
		}
	}

}

