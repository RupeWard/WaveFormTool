using System;
using UnityEngine;

public class EnvelopeGeneratorSaw :  EnvelopeGenerator
{
	public EnvelopeGeneratorSaw () : base("Saw")
	{
	}

	#region IEnvelopeProvider 

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

	private static float s_tolerance = 0.001f;
	protected override float GetTailOutValueForTime (float time, BasicEnvelopeSettings settings)
	{
		float result = 0f;
		if (settings.isInTail (time))
		{
			time -= (settings.leadInLength + settings.midLength);
			float timeFraction = time/ settings.tailOutLength;
			if (timeFraction < (0f-s_tolerance) || (timeFraction > 1f+s_tolerance))
			{
				Debug.LogError ("TimeFraction shouldn't be "+timeFraction);
				timeFraction = Mathf.Clamp (timeFraction, 0f,1f);
			}
			result = settings.midValue - settings.midValue * timeFraction;
		}
		else
		{
			Debug.LogError("Not in tail: "+time);
		}
		return result;
	}

	#endregion IEnvelopeProvider

}

