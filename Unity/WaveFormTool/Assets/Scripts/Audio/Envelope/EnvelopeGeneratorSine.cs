using System;
using UnityEngine;

public class EnvelopeGeneratorSine :  EnvelopeGenerator
{
	public EnvelopeGeneratorSine () : base("Sine")
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
		return Mathf.Sin (phase * 2f * Mathf.PI);
	}

	#endregion IEnvelopeProvider

}

