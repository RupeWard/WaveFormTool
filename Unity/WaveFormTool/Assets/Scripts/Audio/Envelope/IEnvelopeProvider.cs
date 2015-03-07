using UnityEngine;
using System.Collections;

public interface IEnvelopeProvider 
{
	float GetValueForTime(float time);

	string EnvelopeName();

	bool IsReady();

	float EnvelopeLength();

	IEnvelopeProvider Clone();
}
