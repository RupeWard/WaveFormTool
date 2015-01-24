using System;
using UnityEngine;

public class WaveFormGeneratorFlat :  WaveFormGenerator
{
	public WaveFormGeneratorFlat () : base("F")
	{
	}

	#region IWaveFormProvider 


	public override float GetValueForPhase (float phase, IWaveFormDataInterpolator ignored)
	{
		return GetValueForPhase (phase);
	}

	public override float GetValueForPhase (float phase)
	{
		return 0f;
	}

	#endregion IWaveFormProvider

}

