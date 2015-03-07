using System;
using UnityEngine;

public abstract class OldEnvelopeGenerator : IEnvelopeProvider
{
	protected OldEnvelopeSettings envelopeSettings_;

	public GraphCreator MakeGraphCreator()
	{
		return envelopeSettings_.MakeGraphCreator ( );
	}

	public OldEnvelopeGenerator(OldEnvelopeSettings b, string tname)
	{
		envelopeSettings_ = b;
		generatorName_ = tname;
	}

	private string generatorName_;
	public string GeneratorName
	{
		get { return generatorName_; }
	}

	public void SetSettings(OldEnvelopeSettings s)
	{
		envelopeSettings_ = s;
	}

	#region IEnvelopeProvider 
	
	public float GetValueForTime (float time)
	{
		if (envelopeSettings_.isInMid (time))
		{
			return envelopeSettings_.midValue;
		}
		if (envelopeSettings_.isInTail (time))
		{
			return GetTailOutValueForTime(time);
		}
		if (envelopeSettings_.isInLeadIn (time))
		{
			return GetLeadInValueForTime(time);
		}
		Debug.LogError ("Envelope settings don't seem to want to provide anything for" + time);
		return 0f;
	}

	abstract protected float GetLeadInValueForTime (float time);
	abstract protected float GetTailOutValueForTime (float time);

	virtual public string EnvelopeName()
	{
		return generatorName_;
	}

	public bool IsReady()
	{
		return ( true );
	}

	public float EnvelopeLength()
	{
		return envelopeSettings_.TotalLength;
	}

	public IEnvelopeProvider Clone()
	{
		Debug.LogError ( "Don't call this!" );
		return null;
	}

	#endregion IEnvelopeProvider

}

