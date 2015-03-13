using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointDef :IDebugDescribable
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
		GraphIO.WriteStartLine( file, "Point" );

		GraphIO.WriteInt(file, "ID", id);
		GraphIO.WriteVector2(file, "Point", pt);
		GraphIO.WriteFixedState(file, eFixedState);
		GraphIO.WriteFunctionalState(file, eFunctionalState);
		GraphIO.WriteInt(file,"Follower",followerId);
		GraphIO.WriteBool (file,"RangeStart",isRangeStart);
		GraphIO.WriteBool (file,"RangeEnd",isRangeEnd);

		GraphIO.WriteEndLine(file,  "Point" );

	}

	static public GraphPointDef ReadFromFile(System.IO.TextReader file)
	{
		GraphPointDef def = new GraphPointDef ( );

		string line = file.ReadLine ( );
		if ( line == null)
		{
			return null;
		}
		if ( line.StartsWith ( GraphIO.EndLine("Points")) )
		{
			Debug.Log ("Found Points END");
			return null;
		}
		if (! line.StartsWith(GraphIO.StartLine("Point"))) 
		{
			Debug.LogError ("No Point START in '"+line+"'");
			return null;
		}
		
		line = file.ReadLine ( );
		if (! GraphIO.ReadInt ( line, "ID", ref def.id ) )
		{
			Debug.LogError ("No ID in '"+line+"'");
			return null;
		}

		line = file.ReadLine ( );
		if (! GraphIO.ReadVector2 ( line, "Point", ref def.pt ) )
		{
			Debug.LogError ("No Point in '"+line+"'");
			return null;
		}

		line = file.ReadLine ( );
		if (! GraphIO.ReadFixedState ( line, ref def.eFixedState ) )
		{
			Debug.LogError ("No FixedState in '"+line+"'");
			return null;
		}
		
		line = file.ReadLine ( );
		if ( ! GraphIO.ReadFunctionalState ( line, ref def.eFunctionalState ) )
		{
			Debug.LogError ("No FunctionalState in '"+line+"'");
			return null;
		}
		
		line = file.ReadLine ( );
		if ( ! GraphIO.ReadInt ( line, "Follower", ref def.followerId ) )
		{
			Debug.LogError ("No FollowerID in '"+line+"'");
			return null;
		}
		
		line = file.ReadLine ( );
		if ( ! GraphIO.ReadBool ( line, "RangeStart", ref def.isRangeStart ) )
		{
			Debug.LogError ("No RangeStart in '"+line+"'");
			return null;
		}
		
		line = file.ReadLine ( );
		if ( ! GraphIO.ReadBool ( line, "RangeEnd", ref def.isRangeEnd ) )
		{
			Debug.LogError ("No RangeEnd in '"+line+"'");
			return null;
		}
		
		line = file.ReadLine ( );
		if (line == null || false == line.StartsWith(GraphIO.EndLine("Point"))) 
		{
			Debug.LogError ("No Point END in '"+line+"'");
			return null;
		}
		Debug.Log(" Read Point "+def.DebugDescribe());
		return def;		
	}
	
#endregion IO

#region IDebugDescribable
	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[GraphPointDef ( ");
		sb.Append ( id );
		sb.Append(" ) @ ");
		sb.Append (pt.x);
		sb.Append (", ");
		sb.Append (pt.y);
		sb.Append (" ) ");
		if (eFixedState == EFixedState.Fixed)
		{
			sb.Append ("Fixed ");
		}
		if (eFunctionalState == EFunctionalState.NonFunctional)
		{
			sb.Append ("Dead ");
		}
		if ( followerId >=0 )
		{
			sb.Append (" Followed by "+followerId);
		}
		sb.Append (" ]");
	}
#endregion

}
