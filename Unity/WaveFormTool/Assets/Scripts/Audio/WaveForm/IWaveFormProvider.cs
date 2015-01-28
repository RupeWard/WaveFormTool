using UnityEngine;
using System.Collections;

public interface IWaveFormProvider 
{
	float GetValueForPhase(float phase, IWaveFormDataInterpolator interpolator);
	float GetValueForPhase(float phase);

	string WaveFormName();
}
