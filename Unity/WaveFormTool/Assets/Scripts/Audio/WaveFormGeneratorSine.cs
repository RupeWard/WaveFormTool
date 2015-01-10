using System;
using UnityEngine;

public class WaveFormGeneratorSine :  WaveFormGenerator
{
	public WaveFormGeneratorSine () : base("Sine")
	{
	}

	#region IWaveFormProvider 
	
	public override float GetValueForPhase (float phase, IWaveFormDataInterpolator ignored)
	{
		return GetValueForPhase (phase);
	}
	
	public override float GetValueForPhase (float phase)
	{
		return Mathf.Sin (phase * 2f * Mathf.PI);
	}

	#endregion IWaveFormProvider

}

