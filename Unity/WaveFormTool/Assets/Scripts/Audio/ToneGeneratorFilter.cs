using UnityEngine;
using System;

//http://www.develop-online.net/tools-and-tech/procedural-audio-with-unity/0117433

public class ToneGeneratorFilter : MonoBehaviour 
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

		// update increment in case frequency has changed
		increment_ = frequency_ / sampling_frequency;
		for (var i = 0; i < data.Length; i = i + channels)
		{
			phase = phase + increment_;

			data[i] = waveFormProvider_.GetValueForPhase((float)phase, WaveFormDataInterpolatorLinear.Instance);

			// if we have stereo, we copy the mono data to each channel
			if (channels == 2) data[i + 1] = data[i];
			if (phase > 1f) phase = 0;
		}
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
	}
}

/*
public class Sinus : MonoBehaviour
{
  // un-optimized version
  public double frequency = 440;
  public double gain = 0.05;

  private double increment;
  private double phase;
  private double sampling_frequency = 48000;

  void OnAudioFilterRead(float[] data, int channels)
  {
    // update increment in case frequency has changed
    increment = frequency * 2 * Math.PI / sampling_frequency;
    for (var i = 0; i < data.Length; i = i + channels)
    {
      phase = phase + increment;
    // this is where we copy audio data to make them “available” to Unity
      data[i] = (float)(gain*Math.Sin(phase));
    // if we have stereo, we copy the mono data to each channel
      if (channels == 2) data[i + 1] = data[i];
      if (phase > 2 * Math.PI) phase = 0;
    }
  }
} 
 */
