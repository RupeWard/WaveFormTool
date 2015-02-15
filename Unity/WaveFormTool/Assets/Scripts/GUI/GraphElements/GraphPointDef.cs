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

	public int id = -1;
	public Vector2 pt = new Vector2();
	public EFixedState eFixedState = EFixedState.None;
	public EFunctionalState eFunctionalState = EFunctionalState.None;
	public int followerId = -1;

	public bool isRangeStart = false;
	public bool isRangeEnd = false;

#region IO
	public void SaveToFile(System.IO.TextWriter file)
	{
		file.Write ( ">>> Point Start\n" );

		GraphIO.WriteInt(file, "ID", id);
		GraphIO.WriteVector2(file, "ID", pt);
		GraphIO.WriteFixedState(file, eFixedState);
		GraphIO.WriteFunctionalState(file, eFunctionalState);
		GraphIO.WriteInt(file,"Follower",followerId);
		GraphIO.WriteBool (file,"RangeStart",isRangeStart);

		file.Write ( "<<< Point End\n" );
		
	}
	
#endregion IO

}
