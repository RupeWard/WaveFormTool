using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphSettingsDef
{
	public Vector2 xRange = new Vector2();
	public Vector2 yRange = Vector2.zero;

	public Vector2 xView = new Vector2();
	public Vector2 yView = new Vector2();

	public bool allowCrossingXAxis = true;
	
	public bool loop = true;

	public bool ReadFromFile(System.IO.TextReader file)
	{
		string line = file.ReadLine ( );
		if ( line == null )
		{
			Debug.LogError ("No line when looking for GraphSettings START");
			return false;
		}
		if (false == line.StartsWith(GraphIO.StartLine("GraphSettings"))) 
		{
			Debug.LogError ("No '"+GraphIO.StartLine("GraphSettings")+"' in '"+line+"'");
			return false;
		}
		
		line = file.ReadLine ( );
		if (! GraphIO.ReadVector2 ( line, "XRange", ref xRange ) )
		{
			Debug.LogError ("No XRange");
			return false;
		}
		
		line = file.ReadLine ( );
		if (! GraphIO.ReadVector2 ( line, "YRange", ref yRange ) )
		{
			Debug.LogError ("No YRange");
			return false;
		}

		line = file.ReadLine ( );
		if ( ! GraphIO.ReadVector2 ( line, "Xview", ref xView ) )
		{
			Debug.LogError ("No XView");
			return false;
		}

		line = file.ReadLine ( );
		if ( ! GraphIO.ReadVector2 ( line, "Xview", ref xView ) )
		{
			Debug.LogError ("No XView");
			return false;
		}

		line = file.ReadLine ( );
		if ( ! GraphIO.ReadBool ( line, "allowXcrossing", ref allowCrossingXAxis ) )
		{
			Debug.LogError ("No allowXcrossing");
			return false;
		}
		
		line = file.ReadLine ( );
		if ( ! GraphIO.ReadBool ( line, "loop", ref loop ) )
		{
			Debug.LogError ("No loop");
			return false;
		}

		line = file.ReadLine ( );
		if (line == null || false == line.StartsWith(GraphIO.EndLine("GraphSettings"))) 
		{
			Debug.LogError ("No GraphSettings END");
			return false;
		}
		return true;		
	}

}

public class GraphSettings : MonoBehaviour
{
	public void LoadSettings(GraphSettingsDef def)
	{
		xRange = def.xRange;
		yRange = def.yRange;
		xView.Set ( def.xView );
		yView.Set ( def.yView );
		defXView_.Set ( def.xView );
		defYView_.Set ( def.yView );
		allowCrossingXAxis = def.allowCrossingXAxis;
		loop = def.loop;
		ResetViewCentres();
	}
	public Vector2 xRange = new Vector2();
	public Vector2 yRange = Vector2.zero;
	
	public Vector2 xView = new Vector2();
	public Vector2 yView = new Vector2();
	
	public bool allowCrossingXAxis = true;
	
	public bool loop = true;

	public AxisDefinition[] axisDefinitions = {};

	public float pointSize = 4f;
	public float selectedPointMaxSize =16f;
	public float selectedPointThrobTime = 1f;


	private float xViewCentre_ = 0f;
	public float XViewCentre
	{
		get { return xViewCentre_; }
	}

	private float yViewCentre_ = 0f;
	public float YViewCentre
	{
		get { return yViewCentre_; }
	}

	public void ResetViewCentres()
	{
		xViewCentre_ = 0.5f * (xView.y + xView.x);
		yViewCentre_ = 0.5f * (yView.y + yView.x);
	}

	private Vector2 defXView_ = new Vector2();
	private Vector2 defYView_ = new Vector2();

	public void Awake()
	{
		defXView_.Set (xView.x, xView.y);
		defYView_.Set (yView.x, yView.y);
		ResetViewCentres ();
	}

	public void ResetView()
	{
		xView.Set (defXView_.x, defXView_.y);
		yView.Set (defYView_.x, defYView_.y);
		ResetViewCentres ();
	}

	public float XViewLength
	{
		get { return xView.y - xView.x; }
	}

	public float YViewLength
	{
		get { return yView.y - yView.x; }
	}

	public float XRangeLength
	{
		get { return xRange.y - xRange.x; }
	}

	public bool IsXInRange(float x)
	{
		return (x >= xRange.x && x <= xRange.y);
	}

	public bool HasYRange()
	{
		return (yRange.x < yRange.y);
	}

	public bool IsYInRange(float y)
	{
		return (y >= yRange.x && y <= yRange.y);
	}

	public float ClampYToRange(float y)
	{
		if (y < yRange.x)
		{
			return yRange.x;
		}
		if (y > yRange.y)
		{
			return yRange.y;
		}
		return y;
	}
	

	public bool IsXInView(float x)
	{
		return (x >= xView.x && x <= xView.y);
	}

	public bool IsYInView(float y)
	{
		return (y >= yView.x && y <= yView.y);
	}

	public bool IsInView(float x, float y)
	{
		return IsXInView (x) && IsYInView (y);
	}

	public virtual bool allowYChange(float oldY, float newY, System.Text.StringBuilder sb)
	{
		if (oldY == newY)
		{
			if (sb != null)
			{
				sb.Append ("No change in Y, so not doing it");
			}
			return false;
		}
		if (!allowCrossingXAxis && oldY * newY < 0f)
		{
			if (sb != null)
			{
				sb.Append ("Can't change sign of Y");
			}
			return false;
		}
		if (!IsYInRange (newY))
		{
			if (sb != null)
			{
				sb.Append ("Y out of range "+yRange);
			}
			return false;
		}
		
		return true;
	}

#region IO
	public void SaveToFile(System.IO.TextWriter file)
	{
		GraphIO.WriteStartLine(file, "GraphSettings" );
		// x range
		GraphIO.WriteVector2(file, "XRange", xRange);
		// y range
		GraphIO.WriteVector2(file, "YRange", yRange);
		// def xview
		GraphIO.WriteVector2(file, "Xview", defXView_);
		// def yview
		GraphIO.WriteVector2(file, "Xview", defYView_);
		// allowCrossingYviewXAxis		
		GraphIO.WriteBool(file, "allowXcrossing", allowCrossingXAxis);
		// loop 
		GraphIO.WriteBool(file, "loop", loop);
		GraphIO.WriteEndLine(file,  "GraphSettings" );

	}

#endregion IO

}
