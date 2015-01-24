using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointMoverLinear : GraphPointMoverBase // FIXME refactor to Y
{
	private static readonly bool DEBUG_POINTMOVEMENT = true;

	public override void MoveGraphPoint(GraphPoint pt, Vector2 newValues)
	{
		if (newValues.x != pt.Point.x)
		{
			Debug.LogWarning("Mover "+moverName+" can't change x to "+newValues.x+" on "+pt.DebugDescribe());
		}
		float newY = newValues.y;
		float oldY = pt.Point.y;
		pt.SetY(newY);
		GraphPanel graph = pt.graphPanel;
		GraphSettings settings = graph.settings;

		int sign = (newY + oldY < 0f)?(-1):(1);
		float oldAbs = Mathf.Abs(oldY);
		float newAbs = Mathf.Abs(newY);

		float bottomMultiplier = newY/oldY;
		float topMultiplier = (settings.yRange.y - newAbs)/(settings.yRange.y - oldAbs); // FIXME assume symmetry about zero

		System.Text.StringBuilder sb = null;
		if (DEBUG_POINTMOVEMENT)
		{
			Debug.Log("Linear shift of "+bottomMultiplier+" : "+topMultiplier+" from "+pt.DebugDescribe());
			sb = new System.Text.StringBuilder();
			sb.Append("\nPoint movements... ");
		}
		List < GraphPoint > pointsToMove = new List< GraphPoint>();
							
		GraphPoint tp = pt.PreviousPoint;
		while (tp != null && !tp.IsFixed && tp.PreviousPoint != null && tp.PreviousPoint.IsFunctional)
		{
			pointsToMove.Add (tp);
			tp = tp.PreviousPoint;
			if (tp != null && tp.Point.y * sign < 0f) // FIXME Only go to x axis?
			{
				break;
			}
		}
		if (DEBUG_POINTMOVEMENT)
			Debug.Log (pointsToMove.Count.ToString()+" prior points to move");

		tp = pt.NextPoint;
		while (tp != null && !tp.IsFixed && tp.PreviousPoint != null && tp.PreviousPoint.IsFunctional)
		{
			pointsToMove.Add (tp);
			tp = tp.NextPoint;
			if (tp != null && tp.Point.y * sign < 0f) // FIXME Only go to x axis?
			{
				break;
			}
		}
		if (DEBUG_POINTMOVEMENT)
			Debug.Log (pointsToMove.Count.ToString()+" total points to move");

		foreach (GraphPoint gp in pointsToMove)
		{
			float gpAbsY = Mathf.Abs(gp.Point.y);
			if (gpAbsY == oldAbs)
			{
				if (DEBUG_POINTMOVEMENT && sb != null)
				{
					sb.Append("Point not moved "+gp.DebugDescribe()+"\n");
				}
				gpAbsY = newAbs;
			}
			else if (gpAbsY < oldAbs)
			{
				if (DEBUG_POINTMOVEMENT && sb != null)
				{
					sb.Append("Point being moved from zero "+gp.DebugDescribe()+"\n");
				}
				gpAbsY *= bottomMultiplier;
			}
			else
			{
				if (DEBUG_POINTMOVEMENT && sb != null)
				{
					sb.Append("Point being moved from range "+gp.DebugDescribe()+"\n");
			}
			float distFromTop = settings.yRange.y - gpAbsY; // FIXME assumes syyemtry about zero
			distFromTop *= topMultiplier;
			gpAbsY = settings.yRange.y - distFromTop;
		}

		float gpNewY = gpAbsY * sign;
		float altY = settings.ClampYToRange( gpNewY);

		if (altY != gpNewY)
		{
			if (DEBUG_POINTMOVEMENT)
				Debug.LogWarning("Clamping point's y to "+altY+" "+gp.DebugDescribe()); 
			gpNewY = altY;
		}
		gp.SetY(gpNewY);
		}
		if (sb != null)
		{
			Debug.Log (sb.ToString());
		}
		if (DEBUG_POINTMOVEMENT)
		{
			Debug.Log(moverName+" Finished Moving point");
		}
		graph.HandleDataChange ();
	}

	public override void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[Linear]");
	}

}
