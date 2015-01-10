using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPoint : MonoBehaviour 
{
	static readonly Color s_functionalColor = Color.blue;
	static readonly Color s_nonFunctionalColor = new Color (0.5f, 0.5f, 1f,1f);

	public UISprite pointSprite;

	public GraphPoint nextPoint_ = null;
	public GraphPoint previousPoint_ = null;

	private bool isFunctional_ = true;
	public bool IsFunctional
	{
		get { return isFunctional_; }
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

	public void init (GraphPanel p, float x, float y, bool b)
	{
		myGraph_ = p;
		SetFunctional(b);
		SetPoint (x, y);
	}

	public void SetFunctional(bool b)
	{
		isFunctional_ = b;
		pointSprite.color = (isFunctional_) ? (s_functionalColor) : (s_nonFunctionalColor);
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



	public void Awake()
	{
	}

	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[GraphPoint ( " + point_.x + ", " + point_.y + " ) at " + pointSprite.transform.localPosition+" ]");
	}

	public string DebugDescribe()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder ();
		DebugDescribe (sb);
		return sb.ToString ();
	}

}
