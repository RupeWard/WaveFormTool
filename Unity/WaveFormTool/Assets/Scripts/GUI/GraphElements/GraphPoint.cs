using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPoint : MonoBehaviour 
{
	static readonly Color s_functionalColor = Color.green;
	static readonly Color s_functionalColorFixed = Color.blue;
	static readonly Color s_nonFunctionalColor = new Color (0.5f, 0.5f, 1f,1f);

	public UISprite pointSprite;

	public GraphPoint nextPoint_ = null;
	public GraphPoint previousPoint_ = null;

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

	private Vector2 point_ = new Vector2 ();
	public Vector2 Point
	{
		get { return point_; }
	}

	public void init (GraphPanel p, float x, float y, bool functional)
	{
		myGraph_ = p;
		IsFunctional = functional;
		SetPoint (x, y);
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
	}

	public void SetPoint (float x, float y)
	{
		point_.x = x;
		point_.y = y;

		adjustPosition ();
	}

	public void adjustPosition()
	{
		graphPosition_ = myGraph_.GetLocationForPoint (point_.x, point_.y);
		transform.localPosition = new Vector3(graphPosition_.x, graphPosition_.y, transform.localPosition.z);
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

	public void Update()
	{
		if (isAppearanceDirty_)
		{
			SetColour();
			ClearAppearanceDirty();
		}
	}

	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[GraphPoint ( ");
		sb.Append (point_.x);
		sb.Append (", ");
		sb.Append (point_.y);
		sb.Append (" ) at ");
		sb.Append (pointSprite.transform.localPosition);
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
