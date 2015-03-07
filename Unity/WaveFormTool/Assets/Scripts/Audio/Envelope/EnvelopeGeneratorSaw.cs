using System;
using UnityEngine;

public class EnvelopeGeneratorSaw :  OldEnvelopeGenerator
{
	public EnvelopeGeneratorSaw (OldEnvelopeSettings s) : base(s, "Saw")
	{
	}

	#region IEnvelopeProvider 

	protected override float GetLeadInValueForTime (float time)
	{
		float result = 0f;
		if (envelopeSettings_.isInLeadIn (time))
		{
			if (envelopeSettings_.isInLeadInUpToPeak(time))
			{
				float timeFraction = time/ envelopeSettings_.leadInPeakTime;
				if (timeFraction < 0f || timeFraction > 1f)
				{
					Debug.LogError ("TimeFraction shouldn't be "+timeFraction);
				}
				result = timeFraction * envelopeSettings_.leadInPeakValue;
			}
			else
			{
				time -= envelopeSettings_.leadInPeakTime;
				float timeFraction = time/ (envelopeSettings_.leadInLength - envelopeSettings_.leadInPeakTime);
				if (timeFraction < 0f || timeFraction > 1f)
				{
					Debug.LogError ("TimeFraction shouldn't be "+timeFraction);
				}
				result = envelopeSettings_.leadInPeakValue - timeFraction * (envelopeSettings_.leadInPeakValue - envelopeSettings_.midValue);
			}
		}
		else
		{
			Debug.LogError("Not in leadIn: "+time);
		}
		return result;
	}

	private static float s_tolerance = 0.001f;
	protected override float GetTailOutValueForTime (float time)
	{
		float result = 0f;
		if (envelopeSettings_.isInTail (time))
		{
			time -= (envelopeSettings_.leadInLength + envelopeSettings_.midLength);
			float timeFraction = time/ envelopeSettings_.tailOutLength;
			if (timeFraction < (0f-s_tolerance) || (timeFraction > 1f+s_tolerance))
			{
				Debug.LogError ("TimeFraction shouldn't be "+timeFraction);
				timeFraction = Mathf.Clamp (timeFraction, 0f,1f);
			}
			result = envelopeSettings_.midValue - envelopeSettings_.midValue * timeFraction;
		}
		else
		{
			Debug.LogError("Not in tail: "+time);
		}
		return result;
	}

	#endregion IEnvelopeProvider

}

