/// <summary>
/// Wave form data.
/// Constraints - must start with a zero and pass through zero once between there and the end
/// </summary>
using System;
using UnityEngine;

public class WaveFormDataRegular : IWaveFormProvider
{
	private string waveformName_;

	public int NumSamples
	{
		get { return samples_.Length; }
	}

	private float[] samples_;

	static int s_nextTempName = 0;
	static readonly string s_tempName = "tempWaveForm_";

	public WaveFormDataRegular (string tname, int tn)
	{
		if (tn <= 1)
		{
			throw new ArgumentNullException("NumSamples has to be positive, not "+tn);
		}
		if (tname.Length == 0)
		{
			tname = s_tempName + s_nextTempName.ToString();
			s_nextTempName++;
		}
		waveformName_ = tname;
		samples_ = new float[tn];
	}

	public float getSampleConstrained(int i)
	{
		if (i < 0 || i >= samples_.Length)
		{
			throw new System.ArgumentOutOfRangeException("WaveFormData asked for sample #"+i+" :"+DebugDescribe());
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
	

	private bool Validate()
	{
		// FIXME implement
		return true;
	}

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

	public string DebugDescribe()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder ();
		DebugDescribe (sb);
		return sb.ToString ();
	}



	private float getIndexForPhase(float phase)
	{
		phase = phase - Mathf.Floor (phase);
		return phase * NumSamples; 
	}

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

