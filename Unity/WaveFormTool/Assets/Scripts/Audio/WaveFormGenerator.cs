using System;
using UnityEngine;

public abstract class WaveFormGenerator : IWaveFormProvider
{
	private string generatorName_;
	public string GeneratorName
	{
		get { return generatorName_; }
	}

	public WaveFormGenerator (string tname)
	{
		generatorName_ = tname;
	}

	#region IWaveFormProvider 
	
	public abstract float GetValueForPhase (float phase, IWaveFormDataInterpolator interpolator);
	public abstract float GetValueForPhase (float phase);
	virtual public string WaveFormName()
	{
		return generatorName_;
	}
	

	#endregion IWaveFormProvider

}

