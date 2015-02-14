using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointMoverSpline : GraphPointMoverBase // FIXME refactor to Y
{
	private static readonly bool DEBUG_POINTMOVEMENT = true;

	public bool fixStartSlope = true;
	public bool fixEndSlope = true;
	public bool fixStartAdjustSlope = true;
	public bool fixEndAdjustSlope = true;
	
	private const float RAD_TO_DEGREES_FACTOR = 180f / Mathf.PI;
	private float radTodegrees(float f)
	{
		return f * RAD_TO_DEGREES_FACTOR;
	}

	private const float DEGREES_TO_RADS_FACTOR = Mathf.PI / 180f;
	private float degreesToRads(float f)
	{
		return f * DEGREES_TO_RADS_FACTOR;
	}

	public override void MoveGraphPoint(GraphPoint pt, Vector2 newValues)
	{
		if (newValues.x != pt.Point.x)
		{
			Debug.LogWarning("Mover "+moverName+" can't change x to "+newValues.x+" on "+pt.DebugDescribe());
			return;
		}

		if ( pt.IsFixed )
		{
			Debug.LogWarning("Mover "+moverName+" can't moved fixed point "+pt.DebugDescribe());
			return;
		}

		System.Text.StringBuilder sb = null;
		if (DEBUG_POINTMOVEMENT)
		{
			sb = new System.Text.StringBuilder();
			Debug.Log("Spline shift "+" from "+pt.DebugDescribe());
			sb.Append("Spline shift "+" from "+pt.DebugDescribe());
			sb.Append("\nSpline Point movements... ");
		}

		float newY = newValues.y;
		float oldY = pt.Point.y;
		if (DEBUG_POINTMOVEMENT)
		{
			sb.Append ("\nmoved the point itself");
		}
		pt.SetY(newY);
		GraphPanel graph = pt.graphPanel;
		GraphSettings settings = graph.graphSettings;

		List< GraphPoint> splinePoints = new List< GraphPoint >();

		float startSlope = float.NaN;
		float endSlope = float.NaN;

		bool bFoundPointToMove = false;
		GraphPoint startSplinePoint = null;
		if ( pt.PreviousPoint != null )
		{
			startSplinePoint = pt.PreviousPoint;
			while ( false == startSplinePoint.IsFixed && startSplinePoint.PreviousPoint != null )
			{
				bFoundPointToMove = true;
				startSplinePoint = startSplinePoint.PreviousPoint;
			}
		}
		else
		{
			Debug.LogWarning("Mover "+moverName+" can't moved first point "+pt.DebugDescribe());
			return;
		}

		GraphPoint endSplinePoint = null;
		if ( pt.NextPoint != null )
		{
			endSplinePoint = pt.NextPoint;
			while ( false == endSplinePoint.IsFixed && endSplinePoint.NextPoint != null )
			{
				bFoundPointToMove = true;
				endSplinePoint = endSplinePoint.NextPoint;
			}
		}
		else
		{
			Debug.LogWarning("Mover "+moverName+" can't moved last point "+pt.DebugDescribe());
			return;
		}

		if ( false == bFoundPointToMove )
		{
			Debug.LogWarning("Mover "+moverName+" can't move any other points because "+pt.DebugDescribe()
			                 + " has neighbours "+startSplinePoint.DebugDescribe()+" & "+endSplinePoint.DebugDescribe());
			return;
		}
		splinePoints.Add ( startSplinePoint );
		splinePoints.Add ( pt);
		splinePoints.Add ( endSplinePoint);

		if (fixStartSlope)
		{
			if (startSplinePoint.PreviousPoint != null)
			{
				//					startSlope = GraphPoint.SlopeBetween(pt.PreviousPoint.PreviousPoint.PreviousPoint, pt.PreviousPoint.PreviousPoint);
				startSlope = Mathf.Tan(GraphPoint.SlopeBetween(startSplinePoint.PreviousPoint, startSplinePoint));
				if (DEBUG_POINTMOVEMENT)
				{
					sb.Append ("\nfixed start slope at "+startSlope);
				}
			}
			else
			{
				if (DEBUG_POINTMOVEMENT)
				{
					sb.Append ("\nnot fixed start slope because of null ");
				}
			}
		}

		if (fixEndSlope)
		{
			if (endSplinePoint.NextPoint != null)
			{
				//					endSlope = GraphPoint.SlopeBetween(pt.NextPoint.NextPoint.NextPoint, pt.NextPoint.NextPoint);
				endSlope = Mathf.Tan (GraphPoint.SlopeBetween(endSplinePoint.NextPoint, endSplinePoint));
				if (DEBUG_POINTMOVEMENT)
				{
					sb.Append ("\nfixed end slope at "+endSlope);
				}
			}
			else
			{
				if (DEBUG_POINTMOVEMENT)
				{
					sb.Append ("\nnot fixed end slope because of null");
				}
			}
		}


		if (DEBUG_POINTMOVEMENT)
		{
			sb.Append ("\nfitting spline to points "
			           +"\n"+startSplinePoint.DebugDescribe()
			           +"\n"+pt.DebugDescribe()
			           +"\n"+endSplinePoint.DebugDescribe()
			           +"\nslopes = "+startSlope+" / "+endSlope);
		}
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

		spline.Fit(x,y,
	           ( (fixStartSlope)?(startSlope):(float.NaN)),
	           ( (fixEndSlope)?(endSlope):(float.NaN))
	           );
	
		if (DEBUG_POINTMOVEMENT)
		{
			sb.Append("\nslope fitting is "+fixStartSlope+" / "+fixEndSlope);
		}

		float[] fitx = new float[1];
		float[] fity = null;

		GraphPoint movePoint = startSplinePoint.NextPoint;
		while (movePoint != endSplinePoint)
		{
			if (movePoint != pt)
			{
				if (DEBUG_POINTMOVEMENT)
				{
					sb.Append ("\nMoved point "+movePoint.DebugDescribe());
				}
				fitx[0] = movePoint.Point.x;
				fity = spline.Eval(fitx);

				movePoint.SetY(fity[0]);

				if (DEBUG_POINTMOVEMENT)
				{
					sb.Append (" to "+movePoint.DebugDescribe());
				}
			}
			movePoint = movePoint.NextPoint;
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
		sb.Append ("[Spline]");
	}

}
