using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointDef
{
	public enum EFixedState
	{
		None,
		Fixed,
		Free
	}

	static public EFixedState ParseFixedState(string s)
	{
		if ( s == "Fixed" )
		{
			return EFixedState.Fixed;
		}
		else if ( s == "Free" )
		{
			return EFixedState.Free;
		}
		throw new System.NotImplementedException ( "Unrecognised EFixedState '" + s + "'" );
	}

	public enum EFunctionalState
	{
		None,
		Functional,
		NonFunctional
	}
	
	static public EFunctionalState ParseFunctionalState(string s)
	{
		if ( s == "Functional" )
		{
			return EFunctionalState.Functional;
		}
		else if ( s == "NonFunctional" )
		{
			return EFunctionalState.NonFunctional;
		}
		throw new System.NotImplementedException ( "Unrecognised EFunctionalState '" + s + "'" );
	}
	
	static public bool ParseBool(string s)
	{
		if ( s == "true" )
		{
			return true;
		}
		else if ( s == "false" )
		{
			return false;
		}
		throw new System.NotImplementedException ( "Unrecognised bool '" + s + "'" );
	}


	int id = -1;
	float x = float.NaN;
	float y =  float.NaN;
	EFixedState eFixedState = EFixedState.None;
	EFunctionalState efunctionalState = EFunctionalState.None;
	bool bIsFollower = false;
	int followerId = -1;

	bool isRangeStart = false;
	bool isRangeEnd = false;

}
