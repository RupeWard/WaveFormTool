using System;
using UnityEngine;

public abstract class EnvelopeGenerator : IEnvelopeProvider
{
	public float leadInDuration;
	public float leadInPeakValue;
	public float leadInPeakTime;

	public float constSectionDuration;

	public float leadOutDuration;

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
	public float GetValueForTime (float time)
	{
		return 0f;
	}

	abstract protected float GetLeadInValueForTime (float time);
	abstract protected float GetLeadOutValueForTime (float time);

	virtual public string EnvelopeName()
	{
		return generatorName_;
	}
	

	#endregion IEnvelopeProvider

}

