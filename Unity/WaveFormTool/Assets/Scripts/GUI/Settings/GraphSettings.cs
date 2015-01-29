using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphSettings : MonoBehaviour
{

	public Vector2 xRange = new Vector2();
	public Vector2 yRange = Vector2.zero;

	public Vector2 xView = new Vector2();
	public Vector2 yView = new Vector2();

	public AxisDefinition[] axisDefinitions = {};

	public float pointSize = 4f;
	public float selectedPointMaxSize =16f;
	public float selectedPointThrobTime = 1f;

	public bool allowCrossingXAxis = true;

	public bool loop = true;

	private float xViewCentre_ = 0f;
	public float XViewCentre
	{
		get { return xViewCentre_; }
	}

	private float yViewCentre_ = 0f;
	public float YViewCentre
	{
		get { return yViewCentre_; }
	}

	public void ResetViewCentres()
	{
		xViewCentre_ = 0.5f * (xView.y + xView.x);
		yViewCentre_ = 0.5f * (yView.y + yView.x);
	}

	private Vector2 defXView_ = new Vector2();
	private Vector2 defYView_ = new Vector2();

	public void Awake()
	{
		defXView_.Set (xView.x, xView.y);
		defYView_.Set (yView.x, yView.y);
		ResetViewCentres ();
	}

	public void ResetView()
	{
		xView.Set (defXView_.x, defXView_.y);
		yView.Set (defYView_.x, defYView_.y);
		ResetViewCentres ();
	}

	public float XViewLength
	{
		get { return xView.y - xView.x; }
	}

	public float YViewLength
	{
		get { return yView.y - yView.x; }
	}

	public float XRangeLength
	{
		get { return xRange.y - xRange.x; }
	}

	public bool IsXInRange(float x)
	{
		return (x >= xRange.x && x <= xRange.y);
	}

	public bool HasYRange()
	{
		return (yRange.x < yRange.y);
	}

	public bool IsYInRange(float y)
	{
		return (y >= yRange.x && y <= yRange.y);
	}

	public float ClampYToRange(float y)
	{
		if (y < yRange.x)
		{
			return yRange.x;
		}
		if (y > yRange.y)
		{
			return yRange.y;
		}
		return y;
	}
	

	public bool IsXInView(float x)
	{
		return (x >= xView.x && x <= xView.y);
	}

	public bool IsYInView(float y)
	{
		return (y >= yView.x && y <= yView.y);
	}

	public bool IsInView(float x, float y)
	{
		return IsXInView (x) && IsYInView (y);
	}

	public virtual bool allowYChange(float oldY, float newY, System.Text.StringBuilder sb)
	{
		if (oldY == newY)
		{
			if (sb != null)
			{
				sb.Append ("No change in Y, so not doing it");
			}
			return false;
		}
		if (!allowCrossingXAxis && oldY * newY < 0f)
		{
			if (sb != null)
			{
				sb.Append ("Can't change sign of Y");
			}
			return false;
		}
		if (!IsYInRange (newY))
		{
			if (sb != null)
			{
				sb.Append ("Y out of range "+yRange);
			}
			return false;
		}
		
		return true;
	}

}
