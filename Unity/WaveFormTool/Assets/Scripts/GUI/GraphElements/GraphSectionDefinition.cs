using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GraphSectionDefinition : IDebugDescribable, IEnvelopeProvider
{
	protected int numPoints_;
	public int NumPoints
	{
		get { return numPoints_; }
	}
	protected string defnName_ = "UNSET";
	public string DefnName
	{
		get { return defnName_; }
	}

	private Vector2 range_ = new Vector2();
	public Vector2 Range 
	{
		get { return range_; }
	}

	public float Length
	{
		get { return range_.y - range_.x; }
	}

	public GraphSectionDefinition(string sn, Vector2 r, int n)
	{
		defnName_ = sn;
		range_ = r;
		numPoints_ = n;
	}
	abstract public float ComputeY(float x);

	abstract public float maxValue();
	abstract public float minValue();

	#region IDebugDescribable
	abstract public void DebugDescribe ( System.Text.StringBuilder sb );
	#endregion

	#region IEnvelopeProvider
	public abstract float GetValueForTime(float time);
	
	public string EnvelopeName()
	{
		return defnName_ + "_ENV";
	}
	
	public bool IsReady()
	{
		return true;
	}
		
	public abstract float EnvelopeLength ( );

	public abstract IEnvelopeProvider Clone ( );
	#endregion IEnvelopeProvider
}

public class GraphSectionLinear : GraphSectionDefinition
{
	private Vector2 firstPoint;
	private Vector2 lastPoint;

	public GraphSectionLinear(string n, Vector2 fp, Vector2 lp) : base(n, new Vector2(fp.x, lp.x), 2)
	{
		firstPoint = fp;
		lastPoint = lp;
	}

	#region IEnvelopeProvider
	public override IEnvelopeProvider Clone()
	{
		string cloneName = defnName_;
		if ( !defnName_.Contains ( "Clone" ) )
		{
			cloneName += "_Clone";
		}
		else
		{
			cloneName += "X";
		}
		return new GraphSectionLinear(cloneName, firstPoint, lastPoint);
	}

	public override float EnvelopeLength (  )
	{
		return (lastPoint.x - firstPoint.x);
	}
	
	public override float GetValueForTime(float time)
	{
		if ( time.GreaterThanApprox(firstPoint.x, Length/100f) && time.LessThanApprox(lastPoint.x,Length/100f) )
		{
			return ComputeY(time);
		}
		Debug.LogWarning ( "Asked for out of time value " + time + " " + this.DebugDescribe ( ) );
		return 0f;
	}

	#endregion IEnvelopeProvider

	#region GraphSectionDefinition

	override public float maxValue()
	{
		float max = firstPoint.y;
		if ( max < lastPoint.y )
		{
			max = lastPoint.y;
		}
		return max;
	}

	override public float minValue()
	{
		float min = firstPoint.y;
		if ( min > lastPoint.y )
		{
			min = lastPoint.y;
		}
		return min;
	}
	

	override public float ComputeY(float x)
	{
		if ( x < firstPoint.x || x > lastPoint.x )
		{
			Debug.LogError("X="+x+" out of range for "+this.DebugDescribe());
			return 0f;
		}
		return firstPoint.y + ( lastPoint.y - firstPoint.y ) * ( x - firstPoint.x ) / ( lastPoint.x - firstPoint.x );
	}
	#endregion GraphSectionDefinition

	
	#region IDebugDescribable
	public override void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[GraphSectionLinear ");
		sb.Append ( defnName_ ).Append ( " " );
		sb.Append (firstPoint.ToString());
		sb.Append (" to ");
		sb.Append (lastPoint.ToString());
		sb.Append (" ]");
	}	
	#endregion
}

public class GraphSectionSpline : GraphSectionDefinition
{
	public GraphSectionSpline(string n, Vector2 r, int num) : base(n, r, num)
	{
	}

	#region IEnvelopeProvider
	public override float GetValueForTime(float time)
	{
		Debug.LogWarning ( "Not implemented ");
		return 0f;
	}
	
	public override IEnvelopeProvider Clone()
	{
		string cloneName = defnName_;
		if ( !defnName_.Contains ( "Clone" ) )
		{
			cloneName += "_Clone";
		}
		else
		{
			cloneName += "X";
		}
		return new GraphSectionSpline(cloneName, Range, numPoints_);
	}
	public override float EnvelopeLength (  )
	{
		return Length;
	}
	
	#endregion IEnvelopeProvider
	

	#region GraphSectionDefinition
	override public float maxValue()
	{
		//FIXME
		return 0f;
	}
	
	override public float minValue()
	{
		//FIXME
		return 0f;
	}
	override public float ComputeY(float x)
	{
		//FIXME
		return 0f;
	}
	#endregion GraphSectionDefinition

	#region IDebugDescribable
	public override void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[GraphSectionSpline ");
		sb.Append ( defnName_ ).Append (" ");
		sb.Append (" ]");
	}	
	#endregion IDebugDescribable
}
