/// <summary>
/// Wave form data.
/// Constraints - must start with a zero and pass through zero once between there and the end
/// </summary>
using System;
using UnityEngine;

public class WaveFormDataRegular : IWaveFormProvider, IDebugDescribable
{
	private string waveformName_;

	public int NumSamples
	{
		get { return samples_.Length; }
	}

	private float[] samples_;

	private static int s_nextTempName = 0;
	private static readonly string s_tempName = "tempWaveForm_";

	private static int s_minSamples = 5;
	private static int s_maxSamples = 2800;

	public WaveFormDataRegular( string tname, float[] samples)
	{
		if (!ValidateNumSamples(samples.Length))
		{
			throw new ArgumentNullException("NumSamples has to be in range "+s_minSamples+", "+s_maxSamples);
		}
		if (tname.Length == 0)
		{
			tname = s_tempName + s_nextTempName.ToString();
			s_nextTempName++;
		}
		waveformName_ = tname;
		samples_ = samples;
	}

	public WaveFormDataRegular (string tname, int tn)
	{
		if (!ValidateNumSamples(tn))
		{
			throw new ArgumentNullException("NumSamples has to be in range "+s_minSamples+", "+s_maxSamples);
		}
		if (tname.Length == 0)
		{
			tname = s_tempName + s_nextTempName.ToString();
			s_nextTempName++;
		}
		waveformName_ = tname;
		samples_ = new float[tn];
	}

	private bool ValidateNumSamples(int n)
	{
		if (n < s_minSamples || n > s_maxSamples)
		{
			return false;
		}
		return true;
	}

	public float getSampleConstrained(int i)
	{
		if (i < 0 || i >= samples_.Length)
		{
			throw new System.ArgumentOutOfRangeException("WaveFormData asked for sample #"+i+" :"+ this.DebugDescribe());
		}
		return samples_ [i];
	}

	public float getSample(int i)
	{
		while (i < 0)
		{
			i += NumSamples;
		}
		while (i >= NumSamples)
		{
			i -= NumSamples;
		}
		return samples_ [i];
	}
	
	private float getIndexForPhase(float phase)
	{
		phase = phase - Mathf.Floor (phase);
		return phase * NumSamples; 
	}

	private bool Validate()
	{
		// FIXME implement
		return true;
	}

#region IDebugDescribable
	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[WFD: '");
		sb.Append (waveformName_.ToString ());
		sb.Append ("' ");
		if (samples_ == null)
		{
			sb.Append ("null");
		}
		else
		{
			sb.Append (NumSamples.ToString ());
		}
		sb.Append (" ");
		if (samples_ == null)
		{
			sb.Append (" NULL");
		}
		else
		{
			sb.Append(samples_.Length.ToString());
		}
		sb.Append (" ]");
	}
#endregion

#region IWaveFormProvider 

	public float GetValueForPhase(float phase, IWaveFormDataInterpolator interpolator)
	{
		float index = getIndexForPhase (phase);
		return interpolator.GetValueForIndex (this, index);
	}

	public float GetValueForPhase(float phase)
	{
		return GetValueForPhase (phase, WaveFormDataInterpolatorLinear.Instance);
	}

	virtual public string WaveFormName()
	{
		return waveformName_;
	}


#endregion IWaveFormProvider
}

