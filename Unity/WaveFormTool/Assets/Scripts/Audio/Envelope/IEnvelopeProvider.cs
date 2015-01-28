using UnityEngine;
using System.Collections;

public interface IEnvelopeProvider 
{
//	float GetValueForTime(float time, IWaveFormDataInterpolator interpolator);
	float GetValueForTime(float time);

	string EnvelopeName();
}
