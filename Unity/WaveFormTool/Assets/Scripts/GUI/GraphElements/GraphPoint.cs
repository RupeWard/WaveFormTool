using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPoint : MonoBehaviour, IDebugDescribable
{
	static public readonly bool DEBUG_POINT = true;

	static readonly Color s_functionalColor = Color.green;
	static readonly Color s_functionalColorFixed = Color.blue;
	static readonly Color s_nonFunctionalColor = new Color (0.5f, 0.5f, 1f,1f);

	public UISprite pointSprite;
	public UISprite lineSprite;

	private GraphPoint nextPoint_ = null;
	private GraphPoint previousPoint_ = null;

	private Quaternion flatLineRotation_ =  new Quaternion(0,0,0,1);

	public GraphPoint NextPoint
	{
		get { return nextPoint_; }
		set 
		{ 
			nextPoint_ = value;
			updateLine (); 
		}
	}
	public GraphPoint PreviousPoint
	{
		get { return previousPoint_; }
		set 
		{ 
			previousPoint_ = value;
			updateLine (); 
		}
	}

	public bool showLine = true;

	private GraphPoint follower_ = null;
	public void SetFollower(GraphPoint f)
	{
//		Debug.Log ("Set follower: " + f.DebugDescribe () + this.DebugDescribe ());

		if (f != null)
		{
			if (!f.IsFunctional)
			{
				if (f.HasFollower)
				{
					Debug.LogWarning ("Shouldn't chain following, removing the other's follower");
					f.SetFollower (null);
				}
				follower_ = f;
//				Debug.Log ("Set follower: " + f.DebugDescribe () + this.DebugDescribe ());
			}
			else
			{
				Debug.LogWarning ("Follower can't be functional " + f.DebugDescribe () + this.DebugDescribe ());
			}
		}
		else
		{
			follower_ = null;
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


	private bool isFixed_ = false;
	public bool IsFixed
	{
		get { return isFixed_; }
		set
		{
			isFixed_ = value;
			isAppearanceDirty_ = true;
			isDataDirty_ = true;
		}
	}

	private bool isFunctional_ = true;
	public bool IsFunctional
	{
		get { return isFunctional_; }
		set 
		{
			isFunctional_ = value;
			isAppearanceDirty_ = true;
		}
		

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

	public void init (GraphPanel p, float x, float y, bool functional)
	{
		myGraph_ = p;
		IsFunctional = functional;
		SetXY (x, y);
	}

	private void SetColour()
	{
		if (isFunctional_)
		{
			pointSprite.color = (isFixed_) ? (s_functionalColorFixed) : (s_functionalColor);
		}
		else
		{
			pointSprite.color = s_nonFunctionalColor;
		}
		if (isFunctional_ || (nextPoint_ != null && nextPoint_.IsFunctional))
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

	public void SetXY (Vector2 v)
	{
		SetXY (v.x, v.y);
	}

	public void SetXY (float x, float y)
	{
		point_.x = x;
		point_.y = y;
		pointSprite.transform.SetLocalXYSize (myGraph_.settings.pointSize); 
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
		pointSprite.transform.SetLocalXYSize (myGraph_.settings.pointSize); 
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
		pointSprite.transform.SetLocalXYSize (myGraph_.settings.pointSize); 
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
				lineSprite.transform.SetLocalXYPosition 
					( 0.5f*xDist , 
					 0.5f*yDist);

				float length = Mathf.Sqrt( xDist * xDist + yDist*yDist );
				lineSprite.transform.SetLocalXYSize (length, 2f);

				lineSprite.transform.localRotation = flatLineRotation_;
				float angle = Mathf.Atan( yDist / xDist);
				lineSprite.transform.Rotate(0f, 0f, 180f * angle / Mathf.PI);

				bShow = true;
			}
			else
			{
				Debug.LogWarning("No line because nextpoint");
			}
		}
		else
		{
			Debug.LogWarning("No line because showline");
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
		if (isFunctional_)
		{
			Debug.Log ("Point Selected: " + DebugDescribe ());
			myGraph_.OnPointSelected (this);
		}
		else
		{
			Debug.Log ("Attempt to click "+DebugDescribe());
		}
	}
	
	private float throbTime = 0f;
	private bool isSelected = true;

	public void Update()
	{
		if (isAppearanceDirty_)
		{
			SetColour();
			ClearAppearanceDirty();
		}

		if (this == myGraph_.pointPanel_.Point)
		{
			if (!isSelected)
			{
				if (DEBUG_POINT)
					Debug.Log("Point selection detected: "+DebugDescribe());
			}
			isSelected = true;

			float phase = throbTime / myGraph_.settings.selectedPointThrobTime; // 0 to 1
			float size = Mathf.Lerp (myGraph_.settings.pointSize, myGraph_.settings.selectedPointMaxSize, Mathf.Sin (2f * Mathf.PI * phase));
			pointSprite.transform.SetLocalXYSize (size); 
				
			throbTime += Time.deltaTime;
			if (throbTime > myGraph_.settings.selectedPointThrobTime)
			{
				throbTime -= myGraph_.settings.selectedPointThrobTime;
			}
		}
		else
		{
			if (isSelected)
			{
				if (DEBUG_POINT && !myGraph_.IsCreatingGraph)
					Debug.Log("Point deselection detected: "+DebugDescribe());
				pointSprite.transform.SetLocalXYSize (myGraph_.settings.pointSize); 
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

	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[GraphPoint ( ");
		sb.Append (point_.x);
		sb.Append (", ");
		sb.Append (point_.y);
		sb.Append (" ) at ");
		sb.Append (transform.localPosition);
		sb.Append (" ");
		if (isFixed_)
		{
			sb.Append ("Fixed ");
		}
		if (!isFunctional_)
		{
			sb.Append ("Dead ");
		}
		sb.Append (" ]");
	}

	public string DebugDescribe()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder ();
		DebugDescribe (sb);
		return sb.ToString ();
	}


}
