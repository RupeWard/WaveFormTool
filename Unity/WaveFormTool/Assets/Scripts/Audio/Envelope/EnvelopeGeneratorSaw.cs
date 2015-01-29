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

	protected override float GetLeadInValueForTime (float time, BasicEnvelopeSettings settings)
	{
		float result = 0f;
		if (settings.isInLeadIn (time))
		{
			if (settings.isInLeadInUpToPeak(time))
			{
				float timeFraction = time/ settings.leadInPeakTime;
				if (timeFraction < 0f || timeFraction > 1f)
				{
					Debug.LogError ("TimeFraction shouldn't be "+timeFraction);
				}
				result = timeFraction * settings.leadInPeakValue;
			}
			else
			{
				time -= settings.leadInPeakTime;
				float timeFraction = time/ (settings.leadInLength - settings.leadInPeakTime);
				if (timeFraction < 0f || timeFraction > 1f)
				{
					Debug.LogError ("TimeFraction shouldn't be "+timeFraction);
				}
				result = settings.leadInPeakValue - timeFraction * (settings.leadInPeakValue - settings.midValue);
			}
		}
		else
		{
			Debug.LogError("Not in leadIn: "+time);
		}
		return result;
	}

	protected override float GetTailOutValueForTime (float time, BasicEnvelopeSettings settings)
	{
		float result = 0f;
		if (settings.isInTail (time))
		{
			time -= (settings.leadInLength + settings.midLength);
			float timeFraction = time/ settings.tailOutLength;
			if (timeFraction < 0f || timeFraction > 1f)
			{
				Debug.LogError ("TimeFraction shouldn't be "+timeFraction);
			}
			result = settings.midValue - settings.midValue * timeFraction;
		}
		else
		{
			Debug.LogError("Not in tail: "+time);
		}
		return result;
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

