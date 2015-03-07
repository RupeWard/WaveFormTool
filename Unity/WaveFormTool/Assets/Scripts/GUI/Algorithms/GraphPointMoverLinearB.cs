using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointMoverLinearB : GraphPointMoverBase // FIXME refactor to Y
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
		GraphSettings settings = graph.graphSettings;
		
		int sign = (newY + oldY < 0f)?(-1):(1);
		float oldAbs = Mathf.Abs(oldY);
		float newAbs = Mathf.Abs(newY);
		
		float bottomMultiplier = newY/oldY;
		float topMultiplier = (settings.yRange.y - newAbs)/(settings.yRange.y - oldAbs); // FIXME assume symmetry about zero
		
		System.Text.StringBuilder sb = null;
		if (DEBUG_POINTMOVEMENT)
		{
			sb = new System.Text.StringBuilder();
			Debug.Log("Linear shift of "+bottomMultiplier+" : "+topMultiplier+" from "+pt.DebugDescribe());
			sb.Append("\nPoint movements... ");
		}
		List < GraphPoint > pointsToMove = new List< GraphPoint>();
							
		GraphPoint tp = pt.PreviousPointAbsolute;
		while (tp != null && !tp.IsFixed && tp.PreviousPointAbsolute != null && tp.PreviousPointAbsolute.IsFunctional)
		{
			pointsToMove.Add (tp);
			tp = tp.PreviousPointAbsolute;
			if (tp != null && tp.Point.y * sign < 0f) // FIXME Only go to x axis?
			{
				break;
			}
		}
		GraphPoint fixedPriorPoint = tp;

		if (DEBUG_POINTMOVEMENT)
			Debug.Log (pointsToMove.Count.ToString()+" prior points to move relative to fixed point "+fixedPriorPoint.DebugDescribe());

		tp = pt.NextPointAbsolute;
		while (tp != null && !tp.IsFixed && tp.PreviousPointAbsolute != null && tp.PreviousPointAbsolute.IsFunctional)
		{
			pointsToMove.Add (tp);
			tp = tp.NextPointAbsolute;
			if (tp != null && tp.Point.y * sign < 0f) // FIXME Only go to x axis?
			{
				break;
			}
		}
		GraphPoint fixedPostPoint = tp;
		
		if (DEBUG_POINTMOVEMENT)
			Debug.Log (pointsToMove.Count.ToString()+" total points to move, post relative to fixed point "+fixedPostPoint.DebugDescribe());

		float ydiff = newY - oldY;
		float priorXdiff = pt.Point.x - fixedPriorPoint.Point.x;
		float postXdiff = fixedPostPoint.Point.x - pt.Point.x;

		foreach (GraphPoint gp in pointsToMove)
		{
			float gpNewY = gp.Point.y;
			if (gp.Point.x < pt.Point.x)
			{
				float fraction = (gp.Point.x - fixedPriorPoint.Point.x)/priorXdiff;
				gpNewY += fraction * ydiff;
			}
			else
			{
				float fraction = (fixedPostPoint.Point.x - gp.Point.x)/postXdiff;
				gpNewY += fraction * ydiff;
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
		sb.Append ("[LinearB]");
	}

}
