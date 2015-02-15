using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPoint : MonoBehaviour, IDebugDescribable
{
	static public readonly bool DEBUG_POINT = true;
	static public readonly bool DEBUG_POINTLINE = false;

	static readonly Color s_functionalColor = Color.green;
	static readonly Color s_functionalColorFixed = Color.blue;
	static readonly Color s_nonFunctionalColor = new Color (0.5f, 0.5f, 1f,1f);

	public UISprite pointSprite;
	public UISprite lineSprite;

	private GraphPoint nextPoint_ = null;
	private GraphPoint previousPoint_ = null;

	public GraphPointDef pointDef = null;
	public void CreatePointDef(int id)
	{
		pointDef = new GraphPointDef ( );
		pointDef.id = id;
		pointDef.pt = new Vector2 ( point_.x, point_.y );
		pointDef.eFixedState = eFixedState_;
		pointDef.eFunctionalState = eFunctionalState_;

		pointDef.isRangeStart = (this == graphPanel.RangeStart);
		pointDef.isRangeEnd = (this == graphPanel.RangeEnd);
	}
	public void UpdatePointDef()
	{
		if ( follower_ != null )
		{
			pointDef.followerId = follower_.pointDef.id;
		}
	}

	private Quaternion flatLineRotation_ =  new Quaternion(0,0,0,1);

	public GraphPoint NextPoint
	{
		get { return nextPoint_; }
		set 
		{ 
			nextPoint_ = value;
			isAppearanceDirty_ = true; 
		}
	}
	public GraphPoint PreviousPoint
	{
		get { return previousPoint_; }
		set 
		{ 
			previousPoint_ = value;
			isAppearanceDirty_ = true; 
		}
	}

	public bool showLine = true;

	private GraphPoint follower_ = null;
	public GraphPoint Follower
	{
		get { return follower_; }

		set 
		{
//			Debug.Log ("Set follower: " + f.DebugDescribe () + this.DebugDescribe ());

			if (value != null)
			{
				value.SetNonFunctional();
//				if (!value.IsFunctional)
//				{
					if (value.HasFollower)
					{
						Debug.LogWarning ("Shouldn't chain following, aborting set follower to "
						                  +value.DebugDescribe()+ " for " +this.DebugDescribe());
						return;
					}
					follower_ = value;
//				}
//				else
//				{
//					Debug.LogWarning ("Can't be functional follower " + value.DebugDescribe () +" of "+ this.DebugDescribe ());
//				}
				Debug.Log ("Set follower: " + value.DebugDescribe () +" of "+ this.DebugDescribe ());
			}
			else
			{
				if (follower_ == null)
				{
					Debug.LogWarning ("Follower already null for " + this.DebugDescribe ());
				}
				else
				{
					Debug.Log ("Removing follower: " + value.DebugDescribe () + " of " +this.DebugDescribe ());
				}
				follower_ = null;
			}
		}
	}
	public bool HasFollower
	{
		get  { return (follower_ != null); }
	}

	private bool isAppearanceDirty_ = false;
	public bool IsAppearanceDirty
	{
		get { return isAppearanceDirty_; }
	}
	public void ClearAppearanceDirty()
	{
		isAppearanceDirty_ = false;
	}

	private bool isDataDirty_ = false;
	public bool IsDataDirty
	{
		get { return isDataDirty_; }
	}
	public void ClearDataDirty()
	{
		isDataDirty_ = false;
	}
	public void SetDataDirty()
	{
		isDataDirty_ = true;
	}


	private GraphPointDef.EFixedState eFixedState_ = GraphPointDef.EFixedState.Free;
	public GraphPointDef.EFixedState EFixedState 
	{ 
		get { return eFixedState_; }
		set
		{
			if (value != eFixedState_)
			{
				eFixedState_ = value;
				isAppearanceDirty_ = true;
				isDataDirty_ = true;
			}
		}
	}
	public bool IsFixed 
	{ 
		get { return eFixedState_ == GraphPointDef.EFixedState.Fixed;}
	}
	public void SetFixed()
	{
		EFixedState = GraphPointDef.EFixedState.Fixed;
	}
	public void SetFree()
	{
		EFixedState = GraphPointDef.EFixedState.Free;
	}
	public void ToggleFixedState()
	{
		if ( eFixedState_ == GraphPointDef.EFixedState.Free )
		{
			SetFixed ();
		}
		else if ( eFixedState_ == GraphPointDef.EFixedState.Fixed )
		{
			SetFree ();
		}

	}

	private GraphPointDef.EFunctionalState eFunctionalState_ = GraphPointDef.EFunctionalState.Functional;
	public GraphPointDef.EFunctionalState EFunctionalState
	{
		get { return eFunctionalState_; }
		set 
		{
			if (value != eFunctionalState_)
			{
				eFunctionalState_ = value;
				isAppearanceDirty_ = true;
			}
		}
	}
	public bool IsFunctional 
	{ 
		get { return eFunctionalState_ == GraphPointDef.EFunctionalState.Functional; }
	}
	public void SetFunctional()
	{
		EFunctionalState = GraphPointDef.EFunctionalState.Functional;
	}
	public void SetNonFunctional()
	{
		EFunctionalState = GraphPointDef.EFunctionalState.NonFunctional;
	}

	private Vector2 graphPosition_;
	public Vector2 GraphPosition
	{
		get { return graphPosition_; }
	}

	private GraphPanel myGraph_; 
	public GraphPanel graphPanel
	{
		get { return myGraph_; }
	}

	private Vector2 point_ = new Vector2 ();
	public Vector2 Point
	{
		get { return point_; }
	}

	public void init (GraphPanel p, float x, float y, GraphPointDef.EFunctionalState functionalState)
	{
		myGraph_ = p;
		EFunctionalState = functionalState;
		SetXY (x, y);
	}

	private void SetColour()
	{
		switch ( eFunctionalState_ )
		{
			case GraphPointDef.EFunctionalState.Functional:
			{
				// TODO Refactor
				pointSprite.color = 
					(eFixedState_ == GraphPointDef.EFixedState.Fixed) 
						? (s_functionalColorFixed) : (s_functionalColor);
				break;
			}
			case GraphPointDef.EFunctionalState.NonFunctional:
			{
				pointSprite.color = s_nonFunctionalColor;
				break;
			}
			default :
			{
				Debug.LogError ("Unhandled functional state "+eFunctionalState_);
				break;
			}
		}
		if ( lineSprite != null )
		{
			if (eFunctionalState_ == GraphPointDef.EFunctionalState.Functional 
			    && nextPoint_ != null && nextPoint_.EFunctionalState == GraphPointDef.EFunctionalState.Functional)
			{
				// TODO separate colours for lines & points
				lineSprite.color = s_functionalColor;
			}
			else
			{
				// TODO separate colours for lines & points
				lineSprite.color = s_nonFunctionalColor;
			}
		}
	}

	public void SetXY (Vector2 v)
	{
		SetXY (v.x, v.y);
	}

	public void SetXY (float x, float y)
	{
		point_.x = x;
		point_.y = y;
		pointSprite.transform.SetLocalXYSize (myGraph_.graphSettings.pointSize); 
		adjustPosition ();
		updateLine ();
		if (previousPoint_ != null)
		{
			previousPoint_.updateLine();
		}
		if (follower_ != null)
		{
			follower_.SetY (y); 
			// TODO this works so long as we don't change x values, 
			// need to revisit if/when new movers can move x too, follower's
			// x move will have to be relative
		}
	}

	public void SetY (float y)
	{
		point_.y = y;
		pointSprite.transform.SetLocalXYSize (myGraph_.graphSettings.pointSize); 
		adjustPosition ();
		updateLine ();
		if (previousPoint_ != null)
		{
			previousPoint_.updateLine();
		}
		if (follower_ != null)
		{
			follower_.SetY (y);
		}
	}

	public void SetX (float x)
	{
		point_.x = x;
		pointSprite.transform.SetLocalXYSize (myGraph_.graphSettings.pointSize); 
		adjustPosition ();
		updateLine ();
		if (previousPoint_ != null)
		{
			previousPoint_.updateLine();
		}
		if (follower_ != null)
		{
			follower_.SetX (x);
		}
	}

	public void updateLine()
	{
		bool bShow = false;
		if (showLine)
		{
			if (nextPoint_ != null)
			{

				Vector2 pointPosition = myGraph_.GetLocationForPoint (point_.x, point_.y);
				Vector2 nextPointPosition = myGraph_.GetLocationForPoint (nextPoint_.Point.x, nextPoint_.Point.y);

				float xDist = nextPointPosition.x - pointPosition.x;
				float yDist =  nextPointPosition.y - pointPosition.y;
				lineSprite.transform.SetLocalXYPosition( 0.5f*xDist, 0.5f*yDist );

				float length = Mathf.Sqrt( xDist * xDist + yDist*yDist );
				lineSprite.transform.SetLocalXYSize (length, 2f);

				lineSprite.transform.localRotation = flatLineRotation_;
				float angle = Mathf.Atan( yDist / xDist);
				// Debug.Log ("line between "+this.DebugDescribe()+" "+nextPoint_.DebugDescribe());
				lineSprite.transform.Rotate(0f, 0f, 180f * angle / Mathf.PI);

				bShow = true;

			}
			else
			{
				if (DEBUG_POINTLINE)
					Debug.LogWarning("No line because nextpoint");
			}
		}
		else
		{
//			if (DEBUG_POINTLINE)
//				Debug.LogWarning("No line because showline");
		}
		lineSprite.gameObject.SetActive(bShow);
	}

	public void adjustPosition()
	{
		graphPosition_ = myGraph_.GetLocationForPoint (point_.x, point_.y);
		transform.SetLocalXYPosition(graphPosition_.x, graphPosition_.y);
		transform.localScale = Vector3.one;
	}
	
	public void OnSelected()
	{
		if (IsFunctional)
		{
//			Debug.Log ("Point Selected: " + this.DebugDescribe ());
			myGraph_.OnPointSelected (this);
		}
		else
		{
			Debug.Log ("Attempt to click "+this.DebugDescribe());
		}
	}
	
	private float throbTime = 0f;
	private bool isSelected = true;

	public void Update()
	{
		if (isAppearanceDirty_)
		{
			updateLine();
			SetColour();
			ClearAppearanceDirty();
		}

		if (this == myGraph_.pointPanel_.Point)
		{
			if (!isSelected)
			{
				if (DEBUG_POINT)
				{
//					Debug.Log("Point selection detected: "+this.DebugDescribe());
				}
			}
			isSelected = true;

			float phase = throbTime / myGraph_.graphSettings.selectedPointThrobTime; // 0 to 1
			float size = Mathf.Lerp (myGraph_.graphSettings.pointSize, myGraph_.graphSettings.selectedPointMaxSize, Mathf.Sin (2f * Mathf.PI * phase));
			pointSprite.transform.SetLocalXYSize (size); 
				
			throbTime += Time.deltaTime;
			if (throbTime > myGraph_.graphSettings.selectedPointThrobTime)
			{
				throbTime -= myGraph_.graphSettings.selectedPointThrobTime;
			}
		}
		else
		{
			if (isSelected)
			{
				if (DEBUG_POINT && !myGraph_.IsCreatingGraph)
				{
				//	Debug.Log("Point deselection detected: "+this.DebugDescribe());
				}
				pointSprite.transform.SetLocalXYSize (myGraph_.graphSettings.pointSize); 
			}
			isSelected = false;
		}

	}

	public float slopeBetween(GraphPoint other)
	{
		if (other == null)
		{
			throw new System.ArgumentNullException();
		}
		float xDist = other.Point.x - point_.x;
		float yDist = other.Point.y - point_.y;
		return Mathf.Atan (yDist/xDist);
	}

	public static float SlopeBetween(GraphPoint a, GraphPoint b)
	{
		float result = a.slopeBetween (b);
		if (result != b.slopeBetween (a))
		{
			Debug.LogError ("slopebtween!");
		}
		return result;
	}

	public bool MustBeFixed()
	{
		bool result = false;
		if ( this == graphPanel.RangeStart || this == graphPanel.RangeEnd )
		{
			result = true;
		}
		return result;
	}

#region IDebugDescribable
	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[GraphPoint ( ");
		sb.Append (point_.x);
		sb.Append (", ");
		sb.Append (point_.y);
		sb.Append (" ) at ");
		sb.Append (transform.localPosition);
		sb.Append (" ");
		if (IsFixed)
		{
			sb.Append ("Fixed ");
		}
		if (!IsFunctional)
		{
			sb.Append ("Dead ");
		}
		if ( HasFollower )
		{
			sb.Append (" Followed by "+follower_.DebugDescribe());
		}
		else
		{
			sb.Append (" NotFollower");
		}
		sb.Append (" ]");
	}

#endregion

}
