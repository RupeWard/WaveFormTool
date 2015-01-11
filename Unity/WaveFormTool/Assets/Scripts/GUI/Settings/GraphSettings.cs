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
	public AxisDefinition defaultXAxis = null;
	public AxisDefinition defaultYAxis = null;

	public float pointSize = 4f;
	public float selectedPointMaxSize =16f;
	public float selectedPointThrobTime = 1f;

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

	private void ResetViewCentres()
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

	
}
