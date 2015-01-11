using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPanel : MonoBehaviour 
{
	private static readonly bool DEBUG_VIEW = false;
	private static readonly bool DEBUG_GRAPH = false;

	public UILabel messageLabel;

	public GraphSettings settings;

	public GameObject title;

	public UISprite background;

	public UIInput viewMinInput;
	public UIInput viewMaxInput;

	public GraphPointPanel pointPanel_;

	public Transform pointsContainer;
	public Transform axesContainer;

	private GraphPoint firstPoint = null;

	private List < GraphAxis > axes_ = new List< GraphAxis> ();

	private float width_; 
	private float height_;

	private float backgroundMargin = 20f;
	
#region setup

	public void init(Vector2 size, Vector2 pos)
	{
		width_ = size.x - 2f*backgroundMargin;
		height_ = size.y  - 2f*backgroundMargin;
		background.transform.SetLocalXYSize(size.x, size.y);		
		transform.SetLocalXYPosition(pos);
		viewMinInput.text = settings.xView.x.ToString();
		viewMaxInput.text = settings.xView.y.ToString();

		DrawAxes ();

		Vector3 elementSize = viewMinInput.transform.FindChild ("Background").localScale;

		viewMinInput.transform.SetLocalXYPosition( -0.5f * (size.x) + HUDManager.Instance.Margin,
		                                         -0.5f * (size.y) + HUDManager.Instance.Margin + 0.5f*elementSize.y);
		elementSize = viewMaxInput.transform.FindChild ("Background").localScale;
		viewMaxInput.transform.SetLocalXYPosition( -1f*viewMinInput.transform.localPosition.x - elementSize.x,
		                                         viewMinInput.transform.localPosition.y);

		elementSize = title.transform.FindChild ("Size").localScale;
		title.transform.SetLocalXYPosition( -0.5f * (size.x) + 0.5f * elementSize.x + HUDManager.Instance.Margin,
			             0.5f * (size.y) - 0.5f * elementSize.y - HUDManager.Instance.Margin);

		elementSize = pointPanel_.Size ();
		pointPanel_.transform.SetLocalXYPosition( 0.5f*size.x - 0.5f* elementSize.x - HUDManager.Instance.Margin,
			               0.5f * (size.y) - HUDManager.Instance.Margin - 0.5f*elementSize.y);

	}

#endregion setup

#region view
	public bool IsXInView(float x)
	{
		return settings.IsXInView (x);
	}

	public bool IsYInView(float y)
	{
		return settings.IsYInView (y);
	}

	public bool IsInView(float x, float y)
	{
		return settings.IsInView (x, y);
	}

	public bool IsInView(Vector2 v)
	{
		return (IsInView(v.x, v.y));
	}

	public float GetXLocationForPoint(float xIn)
	{
		float xFraction = (xIn - settings.xView.x)/(settings.XViewLength);
		return -0.5f* width_ + xFraction * width_;  // ( xFraction - 0.5f) * (width_); // FIXME assumes centred on zero
	}
	
	public float GetYLocationForPoint(float yIn)
	{
		float yFraction = (yIn - settings.yView.x)/(settings.YViewLength);
		return -0.5f * height_ + yFraction * height_; // y * ((height_ ) / 2f); // FIXME assumes centred on zero
	}
	
	public Vector2 GetLocationForPoint(float x, float y)
	{
		return new Vector2( GetXLocationForPoint(x), GetYLocationForPoint(y) );
	}
	
	public Vector2 GetLocationForPoint(Vector2 v)
	{
		return new Vector2( GetXLocationForPoint(v.x), GetYLocationForPoint(v.y) );
	}
	
	public float GetXLocationLerp(float x1, float x2, float fraction)
	{
		return GetXLocationForPoint(x1 + fraction * (x2 - x1));
	}
	
	public float GetXLocationLerp(Vector2 x, float fraction)
	{
		return GetXLocationLerp (x.x, x.y, fraction);
	}
	
	public float GetYLocationLerp(float y1, float y2, float fraction)
	{
		return GetYLocationForPoint(y1 + fraction * (y2 - y1));
	}
	
	public float GetYLocationLerp(Vector2 y, float fraction)
	{
		return GetYLocationLerp (y.x, y.y, fraction);
	}
	
	public void ResetView ()
	{
		settings.ResetView();
		viewMinInput.text = settings.xView.x.ToString();
		viewMaxInput.text = settings.xView.y.ToString();
		AdjustAxes ();
		StartCoroutine (AdjustPointPositionsCR ());
	}

#endregion view

#region axes

	private void ClearAxes()
	{
		foreach (GraphAxis a in axes_)
		{
			GameObject.DestroyImmediate(a.gameObject);
		}
		axes_.Clear ();
	}

	private void DrawAxes()
	{
		ClearAxes ();

		foreach (AxisDefinition definition in settings.axisDefinitions)
		{
			AxisDefinition thisDef = (GameObject.Instantiate(definition.gameObject) as GameObject).GetComponent<AxisDefinition>();

			GraphAxis a = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/Axis"))as GameObject).GetComponent< GraphAxis>();
			a.init (this, thisDef);
			axes_.Add(a);
		}

		DoDefaultAxes ();
	}

	private void DoDefaultAxes()
	{
		bool bHasX = false;
		bool bHasY = false;

		foreach (GraphAxis a in axes_)
		{
			switch (a.Direction)
			{
				case EXYDirection.X:
				{
					if (a.gameObject.activeSelf)
					{
						bHasX = true;
						if (GraphAxis.DEBUG_AXES)
							Debug.Log ("Found X axis '"+a.AxisName+"'");
					}
					break;
				}
				case EXYDirection.Y:
				{
					if (a.gameObject.activeSelf)
					{
						bHasY = true;
						if (GraphAxis.DEBUG_AXES)
							Debug.Log ("Found Y axis '"+a.AxisName+"'");
					}
					break;
				}
			}
		}

		if (!bHasX)
		{
			if (settings.defaultXAxis != null)
			{
				if (GraphAxis.DEBUG_AXES)
					Debug.Log ("No visible X axis, creating default");
				AxisDefinition defn = (GameObject.Instantiate(settings.defaultXAxis.gameObject) as GameObject).GetComponent<AxisDefinition>();
				defn.value = settings.YViewCentre;
				defn.axisName = "Default Y";
				GraphAxis a = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/Axis"))as GameObject).GetComponent< GraphAxis>();
				a.initTemporary(this, defn);
				axes_.Add(a);
			}
		}

		if (!bHasY)
		{
			if (settings.defaultYAxis != null)
			{
				if (GraphAxis.DEBUG_AXES)
					Debug.Log ("No visible Y axis, creating default");

				AxisDefinition defn = (GameObject.Instantiate(settings.defaultYAxis.gameObject) as GameObject).GetComponent<AxisDefinition>();
				defn.value = settings.XViewCentre;
				defn.axisName = "Default Y";
				GraphAxis a = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/Axis"))as GameObject).GetComponent< GraphAxis>();
				a.initTemporary (this, defn);
				axes_.Add(a);
			}
		}

	}

	private void RemoveTemporaryAxes()
	{
		List <GraphAxis> temps = new List< GraphAxis >();
		foreach (GraphAxis axis in axes_)
		{
			if (axis.IsTemporary)
			{
				temps.Add(axis);
			}
		}
		foreach (GraphAxis axis in temps)
		{
			axes_.Remove(axis);
			GameObject.DestroyImmediate(axis);
		}
	}
	
	public void AdjustAxes()
	{
		RemoveTemporaryAxes ();
		foreach (GraphAxis axis in axes_)
		{
			axis.adjustPosition();
		}
		DoDefaultAxes ();
	}
	
#endregion axes

#region graph

	public void CreateGraph(IWaveFormProvider wfp, int numSamples, bool visibleOnly)
	{
		ResetView ();
		StartCoroutine (CreateGraphCR(wfp, numSamples, visibleOnly));
	}

	private bool isCreatingGraph_ = true;
	public bool IsCreatingGraph
	{
		get { return isCreatingGraph_; }
	}

	public IEnumerator CreateGraphCR(IWaveFormProvider wfp, int numSamples, bool visibleOnly)
	{
		isCreatingGraph_ = true;

		if (DEBUG_GRAPH)
			Debug.Log ("CreateGraph( " + numSamples + " )");
		yield return StartCoroutine(clearPointsCR ());
		if (DEBUG_GRAPH)
			Debug.Log ("Cleared points");

		DrawAxes ();

		float step = settings.XRangeLength / numSamples;

		float currentX = settings.xRange.x;
		while (currentX > settings.xView.x)
		{
				currentX -= step;
		}

		float finalX = settings.xRange.y;
		while (finalX < settings.xView.y)
		{
			finalX += step;
		}

		GraphPoint rangeStart = null;
		GraphPoint rangeEnd = null;

		GraphPoint previous = null;

		while (currentX <= finalX)
		{
			if (!visibleOnly || (settings.IsXInView(currentX) ))
			{
				GraphPoint newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
				newPoint.transform.parent = pointsContainer;
				newPoint.init(this, 
				              currentX, 
				              wfp.GetValueForPhase(currentX, WaveFormDataInterpolatorLinear.Instance),
				              (currentX >= 0f && currentX <= 1f)
				              );
				if (currentX == settings.xRange.x || currentX == settings.xRange.y) // FIXME specific to wave loop
				{
					if (currentX == settings.xRange.x) // FIXME specific to wave loop
					{
						rangeStart = newPoint;
					}
					if (currentX == settings.xRange.y) // FIXME specific to wave loop
					{
						rangeEnd = newPoint;
					}
					OnPointSelected(newPoint);
					pointPanel_.actionMenu.OnOptionSelected(GraphPointActionMenu.fixPointOption);
//					newPoint.IsFixed = true;
				}
				currentX += step;
				
//				Debug.Log ("Created Point : "+newPoint.DebugDescribe());
				
				if (firstPoint == null)
				{
					firstPoint = newPoint;
				}
				else
				{
					newPoint.previousPoint_ = previous;
					previous.nextPoint_ = newPoint;
				}
				previous = newPoint;
				yield return null;
			}
			OnPointSelected(null);
		}

		GraphPoint earlyPoint = rangeStart.previousPoint_;
		GraphPoint followedPoint = rangeEnd.previousPoint_;
		while (earlyPoint != null && followedPoint != null)
		{
			followedPoint.SetFollower(earlyPoint);
			earlyPoint = earlyPoint.previousPoint_;
			followedPoint = followedPoint.previousPoint_;
		}
		GraphPoint latePoint = rangeEnd.nextPoint_;
		followedPoint = rangeStart.nextPoint_;
		while (latePoint != null && followedPoint != null)
		{
			followedPoint.SetFollower(latePoint);
			latePoint = latePoint.nextPoint_;
			followedPoint = followedPoint.nextPoint_;
		}


		if (DEBUG_GRAPH)
			Debug.Log ("Created points");

		isCreatingGraph_ = false;

		yield return null;
	}

	private IEnumerator AdjustPointPositionsCR()
	{
		GraphPoint pt = firstPoint;
		while (pt != null)
		{
			pt.adjustPosition();
			pt = pt.nextPoint_;
			yield return null;
		}
		yield return null;
	}

	private IEnumerator clearPointsCR()
	{
		GraphPoint pt = firstPoint;
		while (pt != null)
		{
			pt.previousPoint_ = null;
			GraphPoint nextPoint = pt.nextPoint_;
			pt.nextPoint_ = null;
			GameObject.DestroyImmediate(pt.gameObject);

			pt = nextPoint;
			yield return null;
		}
		yield return null;
	}

	public void OnPointSelected(GraphPoint p)
	{
		pointPanel_.SetActive (p != null);
		pointPanel_.SetPoint (p);
	}

	public void MovePointY(GraphPoint p, float newY, EYChangeStrategy s)
	{			
		if (settings.IsYInRange(newY))
		{
//			bool onSameSide = true;
			float oldY = p.Point.y;

			if (newY != oldY)
			{
				if (newY * oldY >= 0f)
				{
					int sign = (newY + oldY < 0f)?(-1):(1);
					float oldAbs = Mathf.Abs(oldY);
					float newAbs = Mathf.Abs(newY);

					switch (s)
					{
						case EYChangeStrategy.Solo:
						{
							p.SetY(newY);
							break;
						}
						case EYChangeStrategy.Linear:
						{
							float multiplier = newY/p.Point.y;
							Debug.Log("Linear shift of "+multiplier+" from "+p.DebugDescribe());
							
							List < GraphPoint > pointsToMove = new List< GraphPoint>();
							
							GraphPoint tp = p;
							while (!tp.IsFixed && tp.previousPoint_ != null && tp.previousPoint_.IsFunctional)
							{
								pointsToMove.Add (tp);
								tp = tp.previousPoint_;
							}
							
							tp = p.nextPoint_;
							while (tp != null && !tp.IsFixed && tp.previousPoint_ != null && tp.previousPoint_.IsFunctional)
							{
								pointsToMove.Add (tp);
								tp = tp.nextPoint_;
							}
							foreach (GraphPoint gp in pointsToMove)
							{
								float changedY = settings.ClampYToRange( gp.Point.y * multiplier);
								gp.SetY(changedY);
							}
							
							break;
						}
					}
				}
				else
				{
					messageLabel.text = "Can't change the sign of a point"; 
				}
			}
		}
		else
		{
			messageLabel.text = "Out of range "+settings.yRange;
		}
	}

#endregion graph

#region controls

	public void OnViewMinChanged(string inStr)
	{
		if (DEBUG_VIEW)
			Debug.Log ("OnViewMinChanged( "+inStr+" )");
		float newVal;
		if (float.TryParse (inStr, out newVal))
		{
			if (newVal >= settings.xView.y)
			{
				messageLabel.text = "Must be less than the right hand value";
			}
			else
			{
				settings.xView.x = newVal;

				AdjustAxes ();

				StartCoroutine(AdjustPointPositionsCR());
				messageLabel.text = "View min changed to " + newVal;
			}
		}
		else
		{
			messageLabel.text = ("That's not a number");
		}
		viewMinInput.text = settings.xView.x.ToString();
	}

	public void OnViewMaxChanged(string inStr)
	{
		if (DEBUG_VIEW)
			Debug.Log ("OnViewMaxChanged( "+inStr+" )");
		float newVal;
		if (float.TryParse (inStr, out newVal))
		{
			if (newVal <= settings.xView.x)
			{
				messageLabel.text = "Must be greater than the left hand value";
			}
			else
			{
				settings.xView.y = newVal;

				AdjustAxes ();

				StartCoroutine(AdjustPointPositionsCR());
				messageLabel.text = "View max changed to " + newVal;
			}
		}
		else
		{
			messageLabel.text = ("That's not a number");
		}
		viewMaxInput.text = settings.xView.y.ToString();
	}

#endregion controls

}
