using UnityEngine;
using System.Collections;

public interface IEnvelopeProvider 
{
	float GetValueForTime(float time, BasicEnvelopeSettings settings);

	string EnvelopeName();

	bool IsReady();

	float EnvelopeLength(BasicEnvelopeSettings settings);
}
