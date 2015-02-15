using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPanel : MonoBehaviour 
{
	public static readonly bool DEBUG_ADD_DELETE = true;

	public UILabel messageLabel;

	public GraphSettings graphSettings;

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

	protected bool isCreatingGraph_ = false;
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
		viewMinInput.text = graphSettings.xView.x.ToString();
		viewMaxInput.text = graphSettings.xView.y.ToString();

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
		return graphSettings.IsXInView (x);
	}

	public bool IsYInView(float y)
	{
		return graphSettings.IsYInView (y);
	}

	public bool IsInView(float x, float y)
	{
		return graphSettings.IsInView (x, y);
	}

	public bool IsInView(Vector2 v)
	{
		return (IsInView(v.x, v.y));
	}

	public float GetXLocationForPoint(float xIn)
	{
		float xFraction = (xIn - graphSettings.xView.x)/(graphSettings.XViewLength);
		return UnityHelpers.LerpFree(-0.5f* width_, 0.5f*width_,xFraction);  // ( xFraction - 0.5f) * (width_); // FIXME assumes centred on zero
	}
	
	public float GetYLocationForPoint(float yIn)
	{
		float yFraction = (yIn - graphSettings.yView.x)/(graphSettings.YViewLength);
		return UnityHelpers.LerpFree (-0.5f * height_, 0.5f*height_, yFraction); // y * ((height_ ) / 2f); // FIXME assumes centred on zero
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
		return GetXLocationForPoint(Mathf.Lerp (x1, x2, fraction));
	}
	
	public float GetXLocationLerp(Vector2 x, float fraction)
	{
		return GetXLocationLerp (x.x, x.y, fraction);
	}
	
	public float GetYLocationLerp(float y1, float y2, float fraction)
	{
		return GetYLocationForPoint( Mathf.Lerp (y1, y2, fraction));
	}
	
	public float GetYLocationLerp(Vector2 y, float fraction)
	{
		return GetYLocationLerp (y.x, y.y, fraction);
	}
	
	public virtual void ResetView ()
	{
		graphSettings.ResetView();
		viewMinInput.text = graphSettings.xView.x.ToString();
		viewMaxInput.text = graphSettings.xView.y.ToString();
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

		foreach (AxisDefinition definition in graphSettings.axisDefinitions)
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
			xAxis.value = graphSettings.YViewCentre;
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
			yAxis.value = graphSettings.XViewCentre;
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

	protected IEnumerator AdjustPointPositionsCR()
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
//			pt.PreviousPoint = null;
			GraphPoint nextPoint = pt.NextPoint;
//			pt.NextPoint = null;
			GameObject.Destroy(pt.gameObject);

			pt = nextPoint;
//			yield return null;
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
			
			if (graphSettings.allowYChange (oldY, newY, errSb))
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

	public GraphPoint AddPointBefore(GraphPoint pt)
	{
		return AddPointBefore ( pt, false );
	}

	public GraphPoint AddFollowerBefore(GraphPoint pt)
	{
		return AddPointBefore ( pt, true );
	}

	public void AddAllPointsBefore(GraphPoint pt)
	{
		GraphPoint ptToAddBefore = null;
		do
		{
			ptToAddBefore = pt;
			if (pt != null) pt = pt.PreviousPoint;
		} while (ptToAddBefore!=null && AddPointBefore(ptToAddBefore));
	}

	public void AddAllPointsAfter(GraphPoint pt)
	{
		GraphPoint ptToAddAfter = null;
		do
		{
			ptToAddAfter = pt;
			if (pt != null) pt = pt.NextPoint;
		} while (ptToAddAfter != null && AddPointAfter(ptToAddAfter));
	}

	private GraphPoint AddPointBefore(GraphPoint pt, bool isFollower)
	{
		if ( DEBUG_ADD_DELETE )
		{
			Debug.Log ("AddBefore "+pt.DebugDescribe()
			           +( (isFollower)?(" (follower)"):("not follower")));
		}
		if ( pt.PreviousPoint == null )
		{
			Debug.LogWarning ("Can't add point before "+pt.DebugDescribe()+" because no previous");
			return null;
		}
		if (!isFollower && !pt.PreviousPoint.IsFunctional )
		{
			Debug.LogWarning ("Can't add point before "+pt.DebugDescribe()+" because previous non-functional"); 
			return null;
		}
		GraphPoint newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
		newPoint.transform.parent = pointsContainer;
		newPoint.init(this, 
	              0.5f *(pt.Point.x + pt.PreviousPoint.Point.x), 
	              0.5f *(pt.Point.y + pt.PreviousPoint.Point.y), 
		      (isFollower)?(GraphPointDef.EFunctionalState.NonFunctional):(GraphPointDef.EFunctionalState.Functional)
	              );
		newPoint.PreviousPoint = pt.PreviousPoint;
		newPoint.NextPoint = pt;

		pt.PreviousPoint.NextPoint = newPoint;
		pt.PreviousPoint = newPoint;

		if (!isFollower)
		{
			if (pt.HasFollower)
			{
				GraphPoint follower = AddFollowerBefore(pt.Follower); 
				if (follower != null)
				{
					newPoint.Follower = follower;
					
					if (DEBUG_ADD_DELETE)
					{
						Debug.Log ("Added "+follower.DebugDescribe() 
						           +" as follower of "+newPoint.DebugDescribe()); 
					}
				}
			}
			else
			{
				Debug.Log ("Not adding follower because has no follower "+pt.DebugDescribe()); 
			}
		}
		else
		{
			if (DEBUG_ADD_DELETE)
			{
				Debug.Log ("Not adding follower to follower "+newPoint.DebugDescribe()); 
			}
		}
		return newPoint;
	}

	public GraphPoint AddPointAfter(GraphPoint pt)
	{
		return AddPointAfter (pt, false );
	}

	public GraphPoint AddFollowerAfter(GraphPoint pt)
	{
		return AddPointAfter (pt, true );
	}

	private GraphPoint AddPointAfter(GraphPoint pt, bool isFollower)
	{
		if ( DEBUG_ADD_DELETE )
		{
			Debug.Log ("AddAfter "+pt.DebugDescribe()
			           +( (isFollower)?(" (follower)"):("")));
		}

		if ( pt.NextPoint == null )
		{
			Debug.LogWarning ("Can't add point after "+pt.DebugDescribe()+" because no next");
			return null;
		}
		if (!isFollower && !pt.IsFunctional )
		{
			Debug.LogWarning ("Can't add point after "+pt.DebugDescribe()+" because non-functional"); 
			return null;
		}
		
		GraphPoint newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
		newPoint.transform.parent = pointsContainer;
		newPoint.init(this, 
		              0.5f *(pt.Point.x + pt.NextPoint.Point.x), 
		              0.5f *(pt.Point.y + pt.NextPoint.Point.y), 
		              GraphPointDef.EFunctionalState.Functional
		              );
		newPoint.NextPoint = pt.NextPoint;
		newPoint.PreviousPoint = pt;
		
		pt.NextPoint.PreviousPoint = newPoint;
		pt.NextPoint = newPoint;

		if (!isFollower)
		{
			if (pt.HasFollower)
			{
				GraphPoint follower = AddFollowerAfter(pt.Follower); 
				if (follower != null)
				{
					newPoint.Follower = follower;
					if (DEBUG_ADD_DELETE)
					{
						Debug.Log ("Added "+follower.DebugDescribe() 
						           +" as follower of "+newPoint.DebugDescribe()); 
					}
				}
			}
			else
			{
				Debug.Log ("Not adding follower because has no follower "+pt.DebugDescribe()); 
			}
		}
		else
		{
			if (DEBUG_ADD_DELETE)
			{
				Debug.Log ("Not adding follower because is follower "+pt.DebugDescribe()); 
			}
		}

		return newPoint;
	}
	

	public void DeletePoint(GraphPoint pt)
	{
		DeletePoint ( pt, false );
	}

	public void DeleteFollower(GraphPoint pt)
	{
		DeletePoint ( pt, true );
	}

	private void DeletePoint(GraphPoint pt, bool isFollower)
	{
		Debug.Log ( "DeletePoint " + pt );
		if ( isCreatingGraph_ )
		{
			Debug.LogWarning ("Attempt to delete point while creating ");
			return;
		}
		if (!isFollower && !pt.IsFunctional)
		{
			Debug.LogWarning ("Attempt to delete non-functional point "+pt.DebugDescribe());
			return;
		}
		if (pt.IsFixed)
		{
			Debug.LogWarning ("Attempt to delete fixed point "+pt.DebugDescribe());
			return;
		}
		if (pt == rangeStart_)
		{
			Debug.LogWarning ("Attempt to delete start point "+pt.DebugDescribe());
			return;
		}
		if (pt == rangeEnd_)
		{
			Debug.LogWarning ("Attempt to delete end point "+pt.DebugDescribe());
			return;
		}
		if ( pt.PreviousPoint != null )
		{
			pt.PreviousPoint.NextPoint = pt.NextPoint;
		}
		if ( pt.NextPoint != null )
		{
			pt.NextPoint.PreviousPoint = pt.PreviousPoint;
		}
		if (!isFollower && pt.HasFollower )
		{
			DeleteFollower(pt.Follower);
		}
		GameObject.Destroy ( pt.gameObject );
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
			if (newVal >= graphSettings.xView.y)
			{
				messageLabel.text = "Must be less than the right hand value";
			}
			else
			{
				graphSettings.xView.x = newVal;

				AdjustAxes ();

				StartCoroutine(AdjustPointPositionsCR());
				messageLabel.text = "View min changed to " + newVal;
			}
		}
		else
		{
			messageLabel.text = ("That's not a number");
		}
		viewMinInput.text = graphSettings.xView.x.ToString();
	}

	public void OnViewMaxChanged(string inStr)
	{
		if (DEBUG_VIEW)
			Debug.Log ("OnViewMaxChanged( "+inStr+" )");
		float newVal;
		if (float.TryParse (inStr, out newVal))
		{
			if (newVal <= graphSettings.xView.x)
			{
				messageLabel.text = "Must be greater than the left hand value";
			}
			else
			{
				graphSettings.xView.y = newVal;

				AdjustAxes ();

				StartCoroutine(AdjustPointPositionsCR());
				messageLabel.text = "View max changed to " + newVal;
			}
		}
		else
		{
			messageLabel.text = ("That's not a number");
		}
		viewMaxInput.text = graphSettings.xView.y.ToString();
	}

#endregion controls

#region IO
	private static readonly bool DEBUG_IO = true;

	public void OnSaveButtonClicked()
	{
		SaveToFile ( "graph.txt");
	}

	public void SaveToFile(string filename)
	{
		if ( isCreatingGraph_ )
		{
			messageLabel.text = "Can't save while creating";
		}
		else if ( NumGraphPoints ( ) < 1 )
		{
			Debug.LogWarning("No points to save!");
		}
		else
		{
			StartCoroutine ( SaveToFileCR ( filename ) );
		}
	}

	public IEnumerator SaveToFileCR(string filename)
	{
		bool bFoundFolder = false;

		System.IO.DirectoryInfo saveFolder = new System.IO.DirectoryInfo ( GraphIO.SaveFolder );
		if (!saveFolder.Exists)
		{
			saveFolder.Create();
			saveFolder = new System.IO.DirectoryInfo ( GraphIO.SaveFolder );
			if (!saveFolder.Exists)
			{
				Debug.LogError ("Couldn't create save folder '"+saveFolder+"'");
			}
			else
			{
				bFoundFolder = true;
				Debug.Log ("Created save folder '"+saveFolder+"'");
			}
			yield return null;
		}
		else 
		{
			Debug.Log ("Found save folder '"+saveFolder+"'");
			bFoundFolder = true;
		}
		if (bFoundFolder)
		{
			string filePath = GraphIO.SaveFolder + filename;
			System.IO.TextWriter file = new System.IO.StreamWriter(filePath, false);
			graphSettings.SaveToFile(file);

			yield return StartCoroutine(CreatePointDefsCR());

			GraphPoint p = firstPoint_;
			while (p != null)
			{
				p.pointDef.SaveToFile(file);
				p = p.NextPoint;
			}

			file.Close();
		}

	}

	private IEnumerator CreatePointDefsCR()
	{
		Debug.Log ( "Creating point defs" );
		yield return null;
		int id = 0;
		GraphPoint p = firstPoint_;
		while (p != null)
		{
			p.CreatePointDef(id);
			id++;
			p = p.NextPoint;
		}
		Debug.Log ( "Updating point defs" );
		yield return null;
		p = firstPoint_;
		while (p != null)
		{
			p.UpdatePointDef();
			p = p.NextPoint;
		}
	}
#endregion IO

}
