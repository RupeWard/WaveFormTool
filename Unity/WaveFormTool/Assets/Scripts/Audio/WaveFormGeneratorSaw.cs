using System;
using UnityEngine;

public class WaveFormGeneratorSaw :  WaveFormGenerator
{
	public WaveFormGeneratorSaw () : base("Saw")
	{
	}

	#region IWaveFormProvider 


	public override float GetValueForPhase (float phase, IWaveFormDataInterpolator ignored)
	{
		return GetValueForPhase (phase);
	}

	public override float GetValueForPhase (float phase)
	{
		phase = phase - Mathf.Floor(phase);

		if (phase <= 0.25f)
		{
			return Mathf.Lerp(0f, 1f, phase/0.25f);
		}
		if (phase <= 0.75f)
		{
			return Mathf.Lerp (1f, -1f, (phase - 0.25f)/(0.5f));
		}
		return Mathf.Lerp (-1f, 0f, (phase - 0.75f)/0.25f); 
	}

	#endregion IWaveFormProvider

}

