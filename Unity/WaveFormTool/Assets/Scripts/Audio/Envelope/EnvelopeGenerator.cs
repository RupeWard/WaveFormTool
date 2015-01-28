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
	public abstract float GetValueForTime (float phase);
	virtual public string EnvelopeName()
	{
		return generatorName_;
	}
	

	#endregion IEnvelopeProvider

}

