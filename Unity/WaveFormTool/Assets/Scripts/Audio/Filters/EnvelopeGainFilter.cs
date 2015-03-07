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

	private static double increment_;
	private double currentTime_ = 0;
	private static double s_sampling_frequency = -1;

	public int numToSkip = 1;
	private float lastValue = -1;
	public int numSinceSkip = 0;
	public double envelopeLength_ = 0;

	public void Awake()
	{
		if (s_sampling_frequency == -1)
		{
			s_sampling_frequency = AudioSettings.outputSampleRate;
			increment_ = (double)numToSkip / s_sampling_frequency;

			//Debug.Log("Audio output sampling rate is "+s_sampling_frequency.ToString ());
		}
	}

	public int numCalls=0;
	public int numChannels = -1;
	public int numData = -1;

	public void ResetTime()
	{
		currentTime_ = 0;
		lastValue = 0f;
		numSinceSkip = 0;
		numCalls = 0;
	}

	public void init(IEnvelopeProvider i)
	{
		numToSkip = 1; // TODO remove this
		envelopeProvider_ = i;
		ResetTime ( );
		if ( envelopeProvider_ == null )
		{
			Debug.Log ("Cleared provider");
		}
		else
		{
			envelopeLength_ = (double)envelopeProvider_.EnvelopeLength ( );
			Debug.Log ( "Length = "+envelopeLength_);
		}
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
		if ( numChannels == -1 )
			numChannels = channels;
		if ( numData == -1 )
			numData = data.Length;
		numCalls++;

//		Debug.Log (data.Length + " samples in " + channels + " channels");

		if (envelopeProvider_ != null)
		{

			// update increment in case frequency has changed
			for (int i = 0; i < data.Length; i = i + channels)
			{
				numSinceSkip ++;
				if (numSinceSkip < numToSkip)
				{
					data[i] = lastValue;
				}
				else
				{
 					currentTime_ = currentTime_ + increment_;
					data[i] *= envelopeProvider_.GetValueForTime((float)currentTime_); // TODO null settings?
					numSinceSkip = 0;
					lastValue = data[i];
				}

				// if we have stereo, we copy the mono data to each channel
				if (channels == 2) data[i + 1] = data[i];
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
