using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicEnvelopeProvider : IEnvelopeProvider 
{
	private static readonly bool LOCAL_DEBUG = false;
	private List< Vector2 > points_ = new List <Vector2>();
	public void Clear() 
	{
		points_.Clear ( );
	}
	public void AddPoint(Vector2 v)
	{
		points_.Add ( v );
	}

	int currentPointIndex = -1;

	#region IEnvelopeProvider

	public float GetValueForTime(float time, BasicEnvelopeSettings settings)
	{
		float f = 0f;
		currentPointIndex = FindPointBeforeTime ( currentPointIndex, time );
		if (currentPointIndex >= 0 && currentPointIndex < points_.Count)
		{
			Vector2 currentPoint = points_[currentPointIndex];
			if (currentPointIndex >= (points_.Count -1))
			{
				f = currentPoint.y;
			}
			else
			{
				Vector2 nextPoint = points_[currentPointIndex+1];
				float timeFraction = (time - currentPoint.x)/(nextPoint.x - currentPoint.x);
				f = Mathf.Lerp ( currentPoint.y, nextPoint.y, timeFraction);
			}
		}
		return f;
	}
	
	private int FindPointBeforeTime(int startIndex, double t)
	{
		int result = -1;
		if ( t < 0 )
		{
			if (LOCAL_DEBUG)
			{
				Debug.LogWarning ( "Time " + t+" passed" );
			}
			return result;
		}
		if ( points_.Count < 3)
		{
			if (LOCAL_DEBUG)
			{
				Debug.LogWarning ( "Only " + points_.Count+" points" );
			}
			return result;
		}
		else
		{
			if ( startIndex < 0 || startIndex >= points_.Count )
			{
				if (LOCAL_DEBUG)
				{
					Debug.LogWarning(startIndex+"  passed, starting at first");
				}
				startIndex = 0;
			}
			
			if ( (double)points_[startIndex].x > t )
			{
				if (LOCAL_DEBUG)
				{
					Debug.LogWarning ("Current point is too late for "+t+", going back");
				}
				while (startIndex != -1 && (double)points_[startIndex].x > t)
				{
					startIndex--;
				}
				if (startIndex < 0)
				{
					if (LOCAL_DEBUG)
					{
						Debug.LogWarning ("Went back to the start and didn't find a point before time" +t);
					}
					return result;
				}
				else
				{
					result = startIndex;
				}
			}
			else
			{
				while (startIndex < points_.Count && (double)points_[startIndex].x < t)
				{
					startIndex++;
				}
				if (startIndex >= points_.Count)
				{
					if (LOCAL_DEBUG)
					{
						Debug.LogWarning ("Got to the end and still earlier than time "+t);
					}
					return result;
				}
				else
				{
					result = startIndex;
				}
			}
			
		}
		return result;
	}
	
	public string EnvelopeName() 
	{
		return "EnvelopeGraph";
	}	
	
	public bool IsReady()
	{
		return ( points_.Count > 2 );
	}
		
	public float EnvelopeLength(BasicEnvelopeSettings unused)
	{
		if ( !IsReady ( ) )
			return 0f;
		return ( points_[points_.Count-1].x - points_[0].x);
	}

	public IEnvelopeProvider Clone()
	{
		BasicEnvelopeProvider bep = new BasicEnvelopeProvider ( );
		foreach ( Vector2 p in points_ )
		{
			bep.AddPoint(p);
		}
		return bep;
	}

	
	
	
	#endregion
}
