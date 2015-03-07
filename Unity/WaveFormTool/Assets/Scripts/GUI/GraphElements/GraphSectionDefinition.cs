using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GraphSectionDefinition : IDebugDescribable
{
	protected string defnName_ = "UNSET";
	public string DefnName
	{
		get { return defnName_; }
	}

	private float length_=0f;
	public float Length
	{
		get { return length_; }
	}
	public GraphSectionDefinition(string sn, float l)
	{
		defnName_ = sn;
		length_ = l;
	}
	abstract public float ComputeY(float x);

	#region IDebugDescribable
	abstract public void DebugDescribe ( System.Text.StringBuilder sb );
	#endregion

}

public class GraphSectionLinear : GraphSectionDefinition
{
	private Vector2 firstPoint;
	private Vector2 lastPoint;

	public GraphSectionLinear(string n, Vector2 fp, Vector2 lp) : base(n, lp.x - fp.x)
	{
		firstPoint = fp;
		lastPoint = lp;
	}

	#region GraphSectionDefinition
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
	public GraphSectionSpline(string n, float l) : base(n, l)
	{
	}

	#region GraphSectionDefinition
	override public float ComputeY(float x)
	{
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
	#endregion
}
