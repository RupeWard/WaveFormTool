using System;
using UnityEngine;

public abstract class EnvelopeGenerator : IEnvelopeProvider
{
	private string generatorName_;
	public string GeneratorName
	{
		get { return generatorName_; }
	}

	public EnvelopeGenerator (string tname)
	{
		generatorName_ = tname;
	}

	#region IEnvelopeProvider 
	
	//public abstract float GetValueForPhase (float phase, IWaveFormDataInterpolator interpolator);
	public float GetValueForTime (float time, BasicEnvelopeSettings settings)
	{
		if (settings.isInMid (time))
		{
			return settings.midValue;
		}
		if (settings.isInTail (time))
		{
			return GetTailOutValueForTime(time, settings);
		}
		if (settings.isInLeadIn (time))
		{
			return GetLeadInValueForTime(time, settings);
		}
		Debug.LogError ("Envelope settings don't seem to want to provide anything for" + time);
		return 0f;
	}

	abstract protected float GetLeadInValueForTime (float time, BasicEnvelopeSettings settings);
	abstract protected float GetTailOutValueForTime (float time, BasicEnvelopeSettings settings);

	virtual public string EnvelopeName()
	{
		return generatorName_;
	}
	

	#endregion IEnvelopeProvider

}

