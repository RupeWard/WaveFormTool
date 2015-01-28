using System;
using UnityEngine;

public class EnvelopeGeneratorSaw :  EnvelopeGenerator
{
	public EnvelopeGeneratorSaw () : base("Saw")
	{
	}

	#region IEnvelopeProvider 

	/*
	public override float GetValueForPhase (float phase, IWaveFormDataInterpolator ignored)
	{
		return GetValueForPhase (phase);
	}
	*/

	protected override float GetLeadInValueForTime (float time)
	{
		return 0f;
	}
	protected override float GetLeadOutValueForTime (float time)
	{
		return 0f;
	}

	/*
	public override float GetValueForTime (float phase)
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
	}*/

	#endregion IEnvelopeProvider

}

