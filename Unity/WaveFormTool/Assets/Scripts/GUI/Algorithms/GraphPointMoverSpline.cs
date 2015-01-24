using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointMoverSpline : GraphPointMoverBase // FIXME refactor to Y
{
	private static readonly bool DEBUG_POINTMOVEMENT = true;

	public bool fixSlopes = true;

	private interface IMoveFactorCalculator
	{
		float getMoveFactor (GraphPoint movingPoint, GraphPoint refPoint);
	}

	public override void MoveGraphPoint(GraphPoint pt, Vector2 newValues)
	{
		if (newValues.x != pt.Point.x)
		{
			Debug.LogWarning("Mover "+moverName+" can't change x to "+newValues.x+" on "+pt.DebugDescribe());
		}

		System.Text.StringBuilder sb = null;
		if (DEBUG_POINTMOVEMENT)
		{
			sb = new System.Text.StringBuilder();
			Debug.Log("Splinear shift "+" from "+pt.DebugDescribe());
			sb.Append("Splinear shift "+" from "+pt.DebugDescribe());
			sb.Append("\nSplinear Point movements... ");
		}

		float newY = newValues.y;
		float oldY = pt.Point.y;
		if (DEBUG_POINTMOVEMENT)
			sb.Append ("\nmoved the point itself");
		pt.SetY(newY);
		GraphPanel graph = pt.graphPanel;
		GraphSettings settings = graph.settings;

		List< GraphPoint> splinePoints = new List< GraphPoint >();
		bool bFoundAPointToMove = false;

		float startSlope = float.NaN;
		float endSlope = float.NaN;

		if (pt.PreviousPoint != null)
		{
			if (!pt.PreviousPoint.IsFixed && pt.PreviousPoint.PreviousPoint != null)
			{
				if (DEBUG_POINTMOVEMENT)
					sb.Append ("\npre-previous is spline point");
				splinePoints.Add(pt.PreviousPoint.PreviousPoint);

				if (pt.PreviousPoint.PreviousPoint.PreviousPoint != null)
				{
					startSlope = GraphPoint.SlopeBetween(pt.PreviousPoint.PreviousPoint.PreviousPoint, pt.PreviousPoint.PreviousPoint);
					if (DEBUG_POINTMOVEMENT)
						sb.Append ("\nfixed start slope at "+startSlope);
				}
				else
				{
					if (DEBUG_POINTMOVEMENT)
						sb.Append ("\nnot fixed start slope because of null ");
				}
				bFoundAPointToMove = true;
			}
			else
			{
				splinePoints.Add(pt.PreviousPoint);
				if (DEBUG_POINTMOVEMENT)
					sb.Append ("\nprevious is spline point");
				if (pt.PreviousPoint.IsFixed && pt.PreviousPoint.PreviousPoint != null)
				{
					startSlope = GraphPoint.SlopeBetween(pt.PreviousPoint.PreviousPoint, pt.PreviousPoint);
					if (DEBUG_POINTMOVEMENT)
						sb.Append ("\nfixed start slope at "+startSlope);
				}
				else
				{
					if (DEBUG_POINTMOVEMENT)
						sb.Append ("\nnot fixed start slope with fixed = "+pt.PreviousPoint.IsFixed);
				}
			}
		}
		splinePoints.Add (pt);
		if (DEBUG_POINTMOVEMENT)
			sb.Append ("\npoint itself is spline point");
		if (pt.NextPoint != null)
		{
			if (!pt.NextPoint.IsFixed && pt.NextPoint.NextPoint != null)
			{
				bFoundAPointToMove = true;
				splinePoints.Add(pt.NextPoint.NextPoint);
				if (DEBUG_POINTMOVEMENT)
					sb.Append ("\nnext-next is spline point");
				if (pt.NextPoint.NextPoint.NextPoint != null)
				{
					endSlope = GraphPoint.SlopeBetween(pt.NextPoint.NextPoint.NextPoint, pt.NextPoint.NextPoint);
					if (DEBUG_POINTMOVEMENT)
						sb.Append ("\nfixed end slope at "+startSlope);
				}
				else
				{
					if (DEBUG_POINTMOVEMENT)
						sb.Append ("\nnot fixed end slope because of null");
				}

			}
			else
			{
				splinePoints.Add(pt.NextPoint);
				if (DEBUG_POINTMOVEMENT)
					sb.Append ("\nnext is spline point");
				if (pt.NextPoint.IsFixed && pt.NextPoint.NextPoint != null)
				{
					endSlope = GraphPoint.SlopeBetween(pt.NextPoint.NextPoint, pt.NextPoint);
					if (DEBUG_POINTMOVEMENT)
						sb.Append ("\nfixed end slope at "+endSlope);
				}
				else
				{
					if (DEBUG_POINTMOVEMENT)
						sb.Append ("\nnot fixed end slope with fixed =  "+pt.NextPoint.IsFixed);
				}

			}
		}
		if (bFoundAPointToMove)
		{
			if (DEBUG_POINTMOVEMENT)
				sb.Append ("\nfitting spline with "+splinePoints.Count
				           +" points, slopes = "+startSlope+" / "+endSlope);

			float[] x = new float[splinePoints.Count];
			float[] y = new float[splinePoints.Count];
			for (int i = 0; i < splinePoints.Count; i++)
			{
				x[i] = splinePoints[i].Point.x;
				y[i] = splinePoints[i].Point.y;
			}

			TestMySpline.CubicSpline spline = new TestMySpline.CubicSpline();

			// add slope stuff
			//spline.Fit(x,y, startSlope, endSlope);
			//spline.Fit(x,y, 0f, 0f);
			spline.Fit(x,y);
			Debug.LogWarning("slope fitting turned Off");

			float[] fitx = new float[1];
			float[] fity = null;
			if (pt.PreviousPoint != null && false == splinePoints.Contains(pt.PreviousPoint))
			{
				fitx[0] = pt.PreviousPoint.Point.x;
				fity = spline.Eval(fitx);

				pt.PreviousPoint.SetY(fity[0]);

				if (DEBUG_POINTMOVEMENT)
					sb.Append ("\nprevious poinr moved by spline");

			}
			if (pt.NextPoint != null && false == splinePoints.Contains(pt.NextPoint))
			{
				fitx[0] = pt.NextPoint.Point.x;
				fity = spline.Eval(fitx);

				pt.NextPoint.SetY(fity[0]);

				if (DEBUG_POINTMOVEMENT)
					sb.Append ("\nnext poinr moved by spline");
			}
		}
		else
		{
			Debug.LogError ("Splinear move point "+pt.DebugDescribe()+" found no moveable neighbours");
		}

		/*

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
			else if (newAbs < oldAbs)
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
		*/
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
