using UnityEngine;
using System.Collections;

public interface IWaveFormDataInterpolator
{
	float GetValueForIndex(WaveFormDataRegular data, float findex);
}

public class WaveFormDataInterpolatorFloor : Singleton<WaveFormDataInterpolatorFloor>, IWaveFormDataInterpolator
{
	public float GetValueForIndex(WaveFormDataRegular data, float findex)
	{
		return data.getSample (Mathf.FloorToInt (findex));
	}
}

public class WaveFormDataInterpolatorLinear : Singleton<WaveFormDataInterpolatorFloor>, IWaveFormDataInterpolator
{
	public float GetValueForIndex(WaveFormDataRegular data, float findex)
	{
		float fLowerIndex = Mathf.Floor (findex);
		float fLowerValue = data.getSample( (int)fLowerIndex);
		if (fLowerIndex == findex)
		{
			return fLowerValue;
		}

		float fUpperIndex = fLowerIndex + 1f;
		float fUpperValue = data.getSample( (int)fUpperIndex);
		float fFraction = findex - fLowerIndex;
		return fLowerValue + fFraction * (fUpperValue - fLowerValue);
	}

}


