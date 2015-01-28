using System;
using UnityEngine;

public class EnvelopeGeneratorFlat :  EnvelopeGenerator
{
	public EnvelopeGeneratorFlat () : base("Flat")
	{
	}

	#region IEnvelopeProvider 

	/*
	public override float GetValueForPhase (float phase, IWaveFormDataInterpolator ignored)
	{
		return GetValueForPhase (phase);
	}
	*/

	public override float GetValueForTime (float phase)
	{
		return 0f;
	}

	#endregion IEnvelope

}

