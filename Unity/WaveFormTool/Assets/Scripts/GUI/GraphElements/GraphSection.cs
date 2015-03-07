using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphSection : MonoBehaviour, IDebugDescribable
{
	static public GraphSection CreateGraphSection(GraphPanel parent)
	{
		GameObject go = new GameObject();
		go.AddComponent< GraphSection >();
		GraphSection result = go.GetComponent< GraphSection>();
		result.init ( parent );
		result.transform.localPosition = Vector3.zero;
		result.transform.localScale = Vector3.one;
		return result;
	}
	static public readonly bool DEBUG_POINT = true;
	static public readonly bool DEBUG_POINTLINE = false;
	static public readonly bool DEBUG_SUBGRAPH = true;

	// FIXME colour from settings
	static public readonly Color s_functionalColor = Color.green;
	static public readonly Color s_functionalColorFixed = Color.blue;
	static public readonly Color s_nonFunctionalColor = new Color (0.5f, 0.5f, 1f,1f);

	public UISprite pointSprite;
	public UISprite lineSprite;

	protected GraphPoint firstPoint_ = null;
	public GraphPoint FirstPoint
	{
		get { return firstPoint_; }
		set 
		{
			if (firstPoint_ != null)
			{
				Debug.LogError("Shouldn't be setting FirstPoint more than once");
			}
			firstPoint_ = value;
		}
	}
	public GraphPoint LastPoint
	{
		get
		{
			GraphPoint p = firstPoint_;
			while (p!= null && p.NextPointInternal != null)
			{
				p =  p.NextPointInternal;
			}
			return p;
		}
	}
	private GraphSection previousGraphSection_ = null;
	public GraphSection PreviousGraphSection
	{
		get { return previousGraphSection_; }
	}
	private GraphSection nextGraphSection_ = null;
	public GraphSection NextGraphSection
	{
		get { return nextGraphSection_; }
	}

	public bool IsFirst(GraphPoint p)
	{
		return (p==firstPoint_);
	}
	private Quaternion flatLineRotation_ =  new Quaternion(0,0,0,1);
	
	public bool showLines = true;

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
	
	private GraphPanel graphPanel_; 
	public GraphPanel GraphPanel
	{
		get { return graphPanel_; }
	}

	private void init (GraphPanel p)
	{
		graphPanel_ = p;
		transform.parent = p.pointsContainer;
	}

	public IEnumerator ClearPointsCR(System.Text.StringBuilder sb)
	{
		int n = 0;
		GraphPoint pt = firstPoint_;
		while (pt != null)
		{
			//			pt.PreviousPoint = null;
			GraphPoint nextPoint = pt.NextPointInternal;
			//			pt.NextPoint = null;
			GameObject.Destroy(pt.gameObject);
			n++;
			
			pt = nextPoint;
			yield return null;
		}
		firstPoint_ = null;
		if ( previousGraphSection_ != null )
		{
			previousGraphSection_.nextGraphSection_ = null;
		}
		if ( sb != null )
		{
			sb.Append("GraphSection:Cleared ").Append(n);
		}
		yield return null;
	}

	/* NOT NEEDED?
	public IEnumerator AdjustPointPositionsCR()
	{// FIXME
		yield return null;
	}
	*/

	/*
	private GraphPoint AddPointBefore(GraphPoint pt, bool isFollower)
	{// FIXME
		return null;
	}
	private GraphPoint AddPointAfter(GraphPoint pt, bool isFollower)
	{// FIXME
		return null;
	}
	private void DeletePoint(GraphPoint pt, bool isFollower)
	{// FIXME
	}
	*/
	protected int NumGraphPoints()
	{
		int result = 0;
		GraphPoint p = firstPoint_;
		while ( p != null )
		{
			result++;
			p = p.NextPointInternal;
		}
		return result;
	}
	/*

	private IEnumerator CreatePointDefsCR()
	{
		yield return null;
	}
	public IEnumerator updateLinesCR()
	{
		yield return null;
		// FIXME

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

	public void adjustPositions()
	{
		// FIXME
	}
*/	
	public void OnSelected()
	{
		Debug.LogError ("Not implementred");
		/*
		if (isFunctional_)
		{
			Debug.Log ("Point Selected: " + DebugDescribe ());
			myGraph_.OnPointSelected (this);
		}
		else
		{
			Debug.Log ("Attempt to click "+DebugDescribe());
		}*/
	}
	
//	private bool isSelected = true;

	public void Update()
	{
		/*
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
		*/

	}

	#region IDebugDescribable

	public void DebugDescribe(System.Text.StringBuilder sb)
	{

		sb.Append ("[GraphSection ");
		int num = NumGraphPoints ( );
		if ( num > 0 )
		{
			sb.Append( "n=").Append(num);
			sb.Append( "first=").Append (firstPoint_.DebugDescribe());
			if (num >1)
			{
				sb.Append( " last=").Append (LastPoint.DebugDescribe());
			}
		}
		else
		{
			sb.Append(" empty");
		}
		sb.Append (" ]");
	}

	#endregion IDebugDescribable

}
