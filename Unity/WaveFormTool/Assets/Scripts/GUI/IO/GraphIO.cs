using UnityEngine;
using System.Collections;

public class GraphIO 
{
	public class GraphIOException : System.Exception
	{
		public GraphIOException(string s) : base(s){}
	}

	static public string SaveFolder
	{
		get 
		{
			return Application.persistentDataPath + "/Graphs/";
		}
	}

	static readonly char[] separators = new char[] {' '};

	static private string[] getWords(string line)
	{
		return line.Split ( separators );
	}

	static private string[] getWords(string type, int numExpected, string line)
	{
		string[] result = getWords ( line );
		if (result.Length != numExpected)
		{
			result = null;
			throw new GraphIOException(type+" needs "+numExpected+" =words not '"+line+"'");
		}
		return result;
	}

	static public void WriteVector2(System.IO.TextWriter file, string key, Vector2 v)
	{
		file.Write ( "vec2 "+key + " " + v.x + " " + v.y + "\n" );
	}

	static public bool ReadVector2(string line, string key, ref Vector2 v)
	{
		bool valid = false;
		float x;
		float y;

		string[] words = getWords ( "vec2", 4, line );
		if (words[0] == "vec2")
		{
			if (words[1] == key)
			{
				if (float.TryParse(words[2], out x) && float.TryParse(words[3], out y))
				{
					v = new Vector2(x,y);
					valid = true;
				}
				else
				{
					throw new GraphIOException("vec2 needs floats not '"+line+"'");
				}
			}
		}

		return valid;
	}

	static public void WriteBool(System.IO.TextWriter file, string key, bool b)
	{
		file.Write ( "bool "+key + " " + b.ToString () + "\n" );
	}

	static public bool ReadBool(string line, string key, ref bool b)
	{
		bool valid = false;

		string[] words = getWords ( "bool", 3, line );
		if (words[0] == "bool")
		{
			if (words[1] == key)
			{
				if (words[2] == "true")
				{
					b = true;
					valid = true;
				}
				else if (words[2] == "false")
				{
					b = false;
					valid = true;
				}
				else
				{
					throw new GraphIOException("bool needs true or false not '"+line+"'");
				}
			}
		}
		
		return valid;
	}

	static public void WriteInt(System.IO.TextWriter file, string key, int i)
	{
		file.Write ( "int "+key + " " + i.ToString () + "\n" );
	}
	
	static public bool ReadInt(string line, string key, ref int i)
	{
		bool valid = false;
		
		string[] words = getWords ( "int", 3, line );

		if (words[0] == "int")
		{
			if (words[1] == key)
			{
				int iTry;
				if (int.TryParse(words[2], out iTry))
				{
					i = iTry;
					valid = true;
				}
				else
				{
					throw new GraphIOException("int needs int not '"+line+"'");
				}
			}
		}
		
		return valid;
	}

	static public void WriteString(System.IO.TextWriter file, string key, string s)
	{
		file.Write ( "int "+key + " \"" + s + "\"\n" );
	}
	
	static public bool ReadString(string line, string key, ref string s)
	{
		bool valid = false;
		
		string[] words = getWords ( "text", 3, line );
		
		if (words[0] == "text")
		{
			if (words[1] == key)
			{
				if (words[2].StartsWith("\"") && words[2].EndsWith("\"") ) 
				{
					s = words[2].Substring(1,words[2].Length -2);
					valid = true;
				}
				else
				{
					throw new GraphIOException("text needs text enclosed by \"\" not '"+line+"'");
				}
			}
		}
		
		return valid;
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

	static public void WriteFixedState(System.IO.TextWriter file, GraphPointDef.EFixedState f)
	{
		file.Write ( "FixedState "+ f.ToString () + "\n" );
	}
	
	static public bool ReadFixedState(string line, ref GraphPointDef.EFixedState f)
	{
		bool valid = false;
		
		string[] words = getWords ( "FixedState", 2, line );
		
		if (words[0] == "FixedState")
		{
			if (words[1] == "Fixed")
			{
				f = GraphPointDef.EFixedState.Fixed;
				valid = true;
			}
			else if (words[1] == "Free")
			{
				f = GraphPointDef.EFixedState.Free;
				valid = true;
			}
			else
			{
				throw new GraphIOException("FixedState needs Fixed or Free not '"+line+"'");
			}
		}
		
		return valid;
	}

	static public void WriteFunctionalState(System.IO.TextWriter file, GraphPointDef.EFunctionalState f)
	{
		file.Write ( "FunctionalState "+ f.ToString () + "\n" );
	}
	
	static public bool ReadFunctionalState(string line, ref GraphPointDef.EFunctionalState f)
	{
		bool valid = false;
		
		string[] words = getWords ( "FunctionalState", 2, line );
		
		if (words[0] == "FunctionalState")
		{
			if (words[1] == "Functional")
			{
				f = GraphPointDef.EFunctionalState.Functional;
				valid = true;
			}
			else if (words[1] == "NonFunctional")
			{
				f = GraphPointDef.EFunctionalState.NonFunctional;
				valid = true;
			}
			else
			{
				throw new GraphIOException("FunctionalState needs Functional or NonFunctional not '"+line+"'");
			}
		}
		
		return valid;
	}

}
