using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPanel : MonoBehaviour 
{
	public UILabel messageLabel;

	public GraphSettings settings;

	public GameObject title;

	public UISprite background;

	public UIInput viewMinInput;
	public UIInput viewMaxInput;

	public GraphPointPanel pointPanel_;

	public Transform pointsContainer;
	public Transform axesContainer;

	protected GraphPoint firstPoint_ = null;
	protected GraphPoint rangeStart_ = null;
	public GraphPoint RangeStart
	{
		get { return rangeStart_; }
	}
	protected GraphPoint rangeEnd_ = null;
	public GraphPoint RangeEnd
	{
		get { return rangeEnd_; }
	}

	private List < GraphAxis > axes_ = new List< GraphAxis> ();

	private float width_; 
	private float height_;

	private float backgroundMargin = 20f;

	protected bool isCreatingGraph_ = true;
	public bool IsCreatingGraph
	{
		get { return isCreatingGraph_; }
	}
	

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

		float leftX = -0.5f * size.x + 0.5f * elementSize.x + HUDManager.Instance.Margin;
		float rightX = 0.5f*size.x - 0.5f* elementSize.x - HUDManager.Instance.Margin;
		float topY = 0.5f * (size.y) - HUDManager.Instance.Margin - 0.5f * elementSize.y;
		float bottomY = -0.5f * (size.y) + HUDManager.Instance.Margin + 0.5f * elementSize.y;
		pointPanel_.TopLeftPosition = new Vector2(leftX, topY);
		pointPanel_.TopRightPosition = new Vector2 (rightX, topY);
		pointPanel_.BottomLeftPosition = new Vector2(leftX, bottomY);
		pointPanel_.BottomRightPosition = new Vector2 (rightX, bottomY);

		postInit ();
	}

	protected virtual void postInit()
	{
	}

#endregion setup

#region view
	private static readonly bool DEBUG_VIEW = false;

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
			GameObject.Destroy(a.gameObject);
		}
		axes_.Clear ();
	}

	protected void DrawAxes()
	{
		ClearAxes ();

		foreach (AxisDefinition definition in settings.axisDefinitions)
		{
			GraphAxis a = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/Axis"))as GameObject).GetComponent< GraphAxis>();
			if (a == null) 
			{
				Debug.LogError("Null Axis");
			}
			else
			{
				a.init (this, definition);
				axes_.Add(a);
			}
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
			if (GraphAxis.DEBUG_AXES)
				Debug.Log ("No visible X axis, creating default");

			AxisDefinition xAxis = new AxisDefinition ();
			xAxis.axisName = "X";
			xAxis.eDirection = EXYDirection.X;
			xAxis.value = settings.YViewCentre;
			xAxis.axisName = "Default X";
			GraphAxis a = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/Axis"))as GameObject).GetComponent< GraphAxis>();
			a.init(this, xAxis);
			axes_.Add(a);
		}

		if (!bHasY)
		{
			if (GraphAxis.DEBUG_AXES)
				Debug.Log ("No visible Y axis, creating default");

			AxisDefinition yAxis = new AxisDefinition ();
			yAxis.eDirection = EXYDirection.Y;
			yAxis.value = settings.XViewCentre;
			yAxis.axisName = "Default Y";
			GraphAxis a = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/Axis"))as GameObject).GetComponent< GraphAxis>();
			a.init (this, yAxis);
			axes_.Add(a);
		}
	}

	public void AdjustAxes()
	{
		DrawAxes ();
	}
	
#endregion axes

#region graph
	protected static readonly bool DEBUG_GRAPH = false;

	public void ClearGraph()
	{
		StartCoroutine (ClearPointsCR ());
		pointPanel_.SetActive(false);
	}

	private IEnumerator AdjustPointPositionsCR()
	{
		GraphPoint pt = firstPoint_;
		while (pt != null)
		{
			pt.adjustPosition();
			pt.updateLine();
			pt = pt.NextPoint;
			yield return null;
		}
		yield return null;
	}

	public IEnumerator UpdateLinesCR()
	{
		GraphPoint pt = firstPoint_;
		while (pt != null)
		{
			pt.updateLine();
			pt = pt.NextPoint;
			yield return null;
		}
		yield return null;
	}
	

	protected IEnumerator ClearPointsCR()
	{
		pointPanel_.SetPoint (null);
		rangeStart_ = null;
		rangeEnd_ = null;

		GraphPoint pt = firstPoint_;
		while (pt != null)
		{
			pt.PreviousPoint = null;
			GraphPoint nextPoint = pt.NextPoint;
			pt.NextPoint = null;
			GameObject.Destroy(pt.gameObject);

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

	public void MovePointY(GraphPoint p, GraphPointMoverBase mover, float newY)
	{	
		if (!IsCreatingGraph)
		{
			float oldY = p.Point.y;
			
			System.Text.StringBuilder errSb = new System.Text.StringBuilder ();
			
			if (settings.allowYChange (oldY, newY, errSb))
			{
				mover.MoveGraphPointY (p, newY);
//				StartCoroutine(UpdateLinesCR());
				HandleDataChange ();
			} //if (settings.allowYChange(oldY, newY, errSb))
			
			else
			{
				messageLabel.text = errSb.ToString ();
			}
		}
		else
		{
			messageLabel.text = "Can't move while making";
		}
	}

	protected int NumGraphPoints()
	{
		int n = 0;
		GraphPoint p = rangeStart_;
		while (p != null && p != rangeEnd_)
		{
			n++;
			p = p.NextPoint;
		}
		return n;
	}

#endregion graph

#region delegation

	public virtual void HandleDataChange ()
	{
	}

#endregion

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
