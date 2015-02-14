using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointMoverSplineOld : GraphPointMoverBase // FIXME refactor to Y
{
	private static readonly bool DEBUG_POINTMOVEMENT = true;

	public bool fixStartSlope = true;
	public bool fixEndSlope = true;
	public bool fixStartAdjustSlope = true;
	public bool fixEndAdjustSlope = true;

	public int numToAdjust = 1;
	public int numEachSide = 2;

//	public bool stopAtZero = true;

	private interface IMoveFactorCalculator
	{
		float getMoveFactor (GraphPoint movingPoint, GraphPoint refPoint);
	}

	private float radTodegrees(float f)
	{
		return 180f * f / Mathf.PI;
	}

	private float degreesToRads(float f)
	{
		return Mathf.PI * f / 180f;
	}

	private bool adjustPoint(GraphPoint pt, int numEachSide, System.Text.StringBuilder sb)
	{
		if (pt.IsFixed)
		{
			if (sb != null)
			{
				sb.Append ("\nNot adjusting fixed point " + pt.DebugDescribe());
			}
			return false;
		}
		if (sb != null)
		{
			sb.Append("\nAdjusting point "+pt+" (using "+numEachSide+" each side)");
		}

		List< GraphPoint> splinePoints = new List< GraphPoint >();

		float startSlope = float.NaN;
		float endSlope = float.NaN;

		int nBefore = 0;
		GraphPoint testPoint = pt.PreviousPoint; 
		while (nBefore < numEachSide && testPoint !=null)
		{
			splinePoints.Add (testPoint);
			if (DEBUG_POINTMOVEMENT)
				sb.Append ("\nAdded previous spline point "+testPoint.DebugDescribe());
			nBefore ++;
//			if (testPoint.IsFixed)
//				break;
			testPoint = testPoint.PreviousPoint;
		}
		if (nBefore == 0)
		{
			if (sb!=null)
				sb.Append("\nNo points before so not splining");
			return false;
		}
		if (sb != null)
			sb.Append ("\nFound " + nBefore + " before: " + splinePoints.Count+"\nReversing");
		splinePoints.Reverse ();

		int nAfter = 0;
		testPoint = pt.NextPoint; 
		while (nAfter < numEachSide && testPoint !=null)
		{
			splinePoints.Add (testPoint);
			if (DEBUG_POINTMOVEMENT)
				sb.Append ("\nAdded after spline point "+testPoint.DebugDescribe());
			nAfter ++;
//			if (testPoint.IsFixed)
//				break;
			testPoint = testPoint.NextPoint;
		}
		if (nAfter == 0)
		{
			if (sb!=null)
				sb.Append("\nNo points after so not splining");
			return false;
		}
		if (sb != null)
			sb.Append ("\nFound " + nAfter + " after: " + splinePoints.Count);

		if (splinePoints.Count < 3)
		{
			if (sb != null)
				sb.Append ("\nNot enough points ");
			return false;
		}

		GraphPoint firstPoint = splinePoints [0];
		if (firstPoint.PreviousPoint != null)
		{
//			startSlope = GraphPoint.SlopeBetween(firstPoint.PreviousPoint, firstPoint);
			startSlope = Mathf.Tan (GraphPoint.SlopeBetween(firstPoint.PreviousPoint, firstPoint));
			if (sb != null)
			{
				sb.Append ("\nfixed start slope at "+startSlope);
			}
			else
			{
				if (sb != null)
					sb.Append ("\nnot fixed start slope because of null ");
			}
		}

		GraphPoint lastPoint = splinePoints [ splinePoints.Count -1];
		if (lastPoint.NextPoint != null)
		{
//			endSlope = GraphPoint.SlopeBetween(lastPoint.PreviousPoint, lastPoint);
			endSlope = Mathf.Tan (GraphPoint.SlopeBetween(lastPoint.PreviousPoint, lastPoint));
			if (sb != null)
			{
				sb.Append ("\nfixed end slope at "+endSlope);
			}
			else
			{
				if (sb != null)
					sb.Append ("\nnot fixed end slope because of null ");
			}
		}

		if (sb!=null)
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
			
		spline.Fit (x, y, 
            ( (fixStartAdjustSlope)?( startSlope):(float.NaN)), 
	            ((fixEndAdjustSlope)?(endSlope):(float.NaN)));
		if (sb!=null)
			sb.Append("\nslope fitting on adjust = "+fixStartAdjustSlope+" / "+fixEndAdjustSlope);

		float[] fitx = new float[1];
		float[] fity = null;
		//if (pt.PreviousPoint != null && false == splinePoints.Contains(pt.PreviousPoint))
		{
				fitx[0] = pt.Point.x;
				fity = spline.Eval(fitx);
				
				pt.SetY(fity[0]);
				
				if (sb!=null)
					sb.Append ("\npoint adjusted by spline: "+pt.DebugDescribe());
		}		
		return true;
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
		GraphSettings settings = graph.graphSettings;

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
//					startSlope = GraphPoint.SlopeBetween(pt.PreviousPoint.PreviousPoint.PreviousPoint, pt.PreviousPoint.PreviousPoint);
					startSlope = Mathf.Tan(GraphPoint.SlopeBetween(pt.PreviousPoint.PreviousPoint.PreviousPoint, pt.PreviousPoint.PreviousPoint));
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
//					startSlope = GraphPoint.SlopeBetween(pt.PreviousPoint.PreviousPoint, pt.PreviousPoint);
					startSlope = Mathf.Tan (GraphPoint.SlopeBetween(pt.PreviousPoint.PreviousPoint, pt.PreviousPoint));
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
//					endSlope = GraphPoint.SlopeBetween(pt.NextPoint.NextPoint.NextPoint, pt.NextPoint.NextPoint);
					endSlope = Mathf.Tan (GraphPoint.SlopeBetween(pt.NextPoint.NextPoint.NextPoint, pt.NextPoint.NextPoint));
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
//					endSlope = GraphPoint.SlopeBetween(pt.NextPoint.NextPoint, pt.NextPoint);
					endSlope = Mathf.Tan (GraphPoint.SlopeBetween(pt.NextPoint.NextPoint, pt.NextPoint));
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

			spline.Fit(x,y,
			           ( (fixStartSlope)?(startSlope):(float.NaN)),
			           ( (fixEndSlope)?(endSlope):(float.NaN))
			           );
			if (DEBUG_POINTMOVEMENT)
				sb.Append("\nslope fitting is "+fixStartSlope+" / "+fixEndSlope);

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
					sb.Append ("\nnext point moved by spline");
			}

			if (DEBUG_POINTMOVEMENT)
			{
				sb.Append ("\nLooking for "+numToAdjust+" each side to adjust"); 
			}
			int nBefore = 0;
			GraphPoint beforePoint = pt.PreviousPoint.PreviousPoint;
			while (nBefore < numToAdjust && beforePoint != null)
			{
				if (DEBUG_POINTMOVEMENT)
				{
					sb.Append ("\nTrying to adjust before #"+(nBefore+1)+ " "+beforePoint.DebugDescribe()); 
				}
				if (adjustPoint(beforePoint, numEachSide, sb))
				{
					nBefore++;
					beforePoint = beforePoint.PreviousPoint;
				}
				else
				{
					break;
				}
			}
			int nAfter = 0;
			GraphPoint afterPoint = pt.NextPoint.NextPoint;
			while (nAfter < numToAdjust && afterPoint != null)
			{
				if (DEBUG_POINTMOVEMENT)
				{
					sb.Append ("\nTrying to adjust after #"+(nAfter+1)+" "+afterPoint.DebugDescribe()); 
				}
				if (adjustPoint(afterPoint, numEachSide, sb))
				{
					nAfter++;
					afterPoint = afterPoint.NextPoint;
				}
				else
				{
					break;
				}
			}

			// Now adjust
		}
		else
		{
			Debug.LogError ("Splinear move point "+pt.DebugDescribe()+" found no moveable neighbours");
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

	private void doSplineMove(GraphPoint pt)
	{
	}

	public override void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[LinearB]");
	}

}
