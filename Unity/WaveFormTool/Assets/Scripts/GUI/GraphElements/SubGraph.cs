using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubGraph : /*MonoBehaviour,*/ IDebugDescribable
{
	static public readonly bool DEBUG_POINT = true;
	static public readonly bool DEBUG_POINTLINE = false;

	// FIXME playing colour
	static readonly Color s_functionalColor = Color.green;
	static readonly Color s_functionalColorFixed = Color.blue;
	static readonly Color s_nonFunctionalColor = new Color (0.5f, 0.5f, 1f,1f);

	public UISprite pointSprite;
	public UISprite lineSprite;

//	private GraphPoint nextPoint_ = null; // segment
//	private GraphPoint previousPoint_ = null; // segment

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

	private GraphPanel myGraph_; 
	public GraphPanel graphPanel
	{
		get { return myGraph_; }
	}

	public void init (GraphPanel p, float x, float y, bool functional)
	{
		myGraph_ = p;
		IsFunctional = functional;

		// FIXME
	}


	public void updateLines()
	{
		// FIXME

		/*
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
		*/
	}

	public void adjustPositions()
	{
		// FIXME
	}
	
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
	
	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		/*
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
		*/
	}

	public string DebugDescribe()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder ();
		DebugDescribe (sb);
		return sb.ToString ();
	}


}
