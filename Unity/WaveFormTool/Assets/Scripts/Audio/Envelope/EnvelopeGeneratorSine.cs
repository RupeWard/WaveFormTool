using System;
using UnityEngine;

public class EnvelopeGeneratorSine :  OldEnvelopeGenerator
{
	public EnvelopeGeneratorSine (OldEnvelopeSettings s) : base(s, "Sine")
	{
	}

	#region IEnvelopeProvider 

	protected override float GetLeadInValueForTime (float time)
	{
		return 0f;
	}
	protected override float GetTailOutValueForTime (float time)
	{
		return 0f;
	}


	/*
	public override float GetValueForPhase (float phase, IWaveFormDataInterpolator ignored)
	{
		return GetValueForPhase (phase);
	}
	*/

	/*
	public override float GetValueForTime (float phase)
	{
		return Mathf.Sin (phase * 2f * Mathf.PI);
	}*/

	#endregion IEnvelopeProvider

}

