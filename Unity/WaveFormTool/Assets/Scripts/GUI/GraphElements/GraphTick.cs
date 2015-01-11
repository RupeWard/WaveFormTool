using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphTick : MonoBehaviour 
{
	public UISprite tickSprite;
	public UILabel label;

	protected GraphAxis myAxis_; 

	protected float value_;
	public float Value
	{
		get { return value_; }
	}

	public float AxisPosition
	{
		get { return myAxis_.Value; }
	}

	public void init (GraphAxis a, float v, float labelSize)
	{
		myAxis_  = a;
		transform.parent = a.transform;
		transform.localScale = Vector2.one;

		SetLabelSize (labelSize);
		SetValue(v);
	}

	public void adjustPosition()
	{
		switch (myAxis_.Direction)
		{
			case EXYDirection.X:
			{
				transform.SetLocalXYPosition(
						myAxis_.Graph.GetXLocationForPoint(value_), 
						0f);
				break;
			}
			case EXYDirection.Y:
			{
				transform.SetLocalXYPosition(
						0f, 
						myAxis_.Graph.GetYLocationForPoint(value_));
				break;
			}
		}
		transform.localScale = Vector3.one;
		gameObject.SetActive ( IsInView());
	}

	public bool IsInView()
	{
		bool result = false;
		switch (myAxis_.Direction)
		{
			case EXYDirection.X:
			{
				result = myAxis_.Graph.IsInView(value_, AxisPosition);
				break;
			}
			case EXYDirection.Y:
			{
				result = myAxis_.Graph.IsInView(AxisPosition, value_);
				break;
			}
		}
		return result;
	}
	
	public void SetLabelSize(float f)
	{
		label.transform.SetLocalXYSize(f);
	}

	public void SetValue(float v)
	{
		value_ = v;
		label.text = string.Format("{0:0.##}",value_);
		adjustPosition ();
	}

	public void Set(float v)
	{
		value_ = v;
		label.text = string.Format("{0:0.##}",value_);

		adjustPosition ();
	}
	
	public Vector2 GetPosition()
	{
		switch (myAxis_.Direction)
		{
			case EXYDirection.X:
			{
				return myAxis_.Graph.GetLocationForPoint(value_, AxisPosition);
			}
			case EXYDirection.Y:
			{
				return myAxis_.Graph.GetLocationForPoint(AxisPosition, value_);
			}
		}
		throw new System.InvalidOperationException("Can't handle EXYDirection = "+myAxis_.Direction); 
	}
}
