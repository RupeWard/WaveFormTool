using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphAxis : MonoBehaviour 
{
	public UISprite axisSprite;
	
	private AxisDefinition definition_ = null;
	private bool isTemporary_ = false;
	public bool IsTemporary
	{
		get { return isTemporary_; }
	}

	public EXYDirection Direction
	{
		get { return definition_.eDirection; }
	}

	public float Value
	{
		get { return definition_.value; }
	}

	public string AxisName
	{
		get { return definition_.axisName; }
	}

	private List< GraphTick > ticks_ = new List< GraphTick>();
	
	protected GraphPanel graph_; 
	public GraphPanel Graph
	{
		get { return graph_; }
	}
	
	private Vector2 ViewRange()
	{
		switch (Direction)
		{
			case EXYDirection.X:
			{
				return graph_.settings.xView;
			}
			case EXYDirection.Y:
			{
				return graph_.settings.yView;
			}
		}
		return Vector2.zero;
	}


	public void initTemporary (GraphPanel p, AxisDefinition d)
	{
		isTemporary_ = true;
		init (p, d);
	}

	public void init (GraphPanel p, AxisDefinition d)
	{
		definition_ = d;
		definition_.transform.parent = transform;

		gameObject.name = d.axisName;

		graph_ = p;

		transform.parent = graph_.axesContainer;
		transform.localScale = Vector3.one;

		SetSpriteSize (definition_.value);
		CreateTicks ();

		adjustPosition ();
	}

	public void OnDestroy()
	{
		if (definition_ != null)
		{
			GameObject.DestroyImmediate(definition_);
			definition_ = null;
		}
	}

	public void CreateTicks()
	{
		ClearTicks ();

		float tickStart = definition_.tickBase;

		Vector2 viewRange = ViewRange ();

		while (tickStart > viewRange.x)
		{
			tickStart -= definition_.tickSpacing;
		}
		float tickEnd = 1f;
		while (tickEnd < viewRange.y)
		{
			tickEnd += definition_.tickSpacing;
		}
		
		float value = tickStart;
		while (value <= tickEnd)
		{
			if (true)
			{
				// create tick 
				GraphTick newTick = null;
				
				switch (Direction)
				{
					case EXYDirection.X:
					{
						newTick  = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/XTick"))as GameObject).GetComponent< GraphTick>();
						break;
					}
					case EXYDirection.Y:
					{
						newTick  = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/YTick"))as GameObject).GetComponent< GraphTick>();
						break;
					}
				}
				
				// init it
				newTick.init (this, value, definition_.tickLabelSize);
				
				ticks_.Add(newTick);

			}
			value += definition_.tickSpacing;
		}
	}

	public void ClearTicks()
	{
		foreach (GraphTick tick in ticks_)
		{
			GameObject.DestroyImmediate(tick.gameObject);
		}
		ticks_.Clear ();
	}
	
	public void adjustPosition()
	{
		switch (Direction)
		{
			case EXYDirection.X:
			{
				transform.localPosition 
					= new Vector3 (
						graph_.GetXLocationLerp ( graph_.settings.xView, 0.5f ), 
						graph_.GetYLocationForPoint( Value ), 
						transform.localPosition.z);
				break;
			}
			case EXYDirection.Y:
			{
				transform.localPosition 
					= new Vector3 (
						graph_.GetXLocationForPoint ( Value ), 
						graph_.GetYLocationLerp(graph_.settings.yView, 0.5f),
						transform.localPosition.z);
				break;
			}
		}

		foreach (GraphTick t in ticks_)
		{
			t.adjustPosition();
		}
		transform.localScale = Vector3.one;
		gameObject.SetActive ( IsInView());
	}


	public void SetSpriteSize(float f)
	{ 
		switch (Direction)
		{
			case EXYDirection.X:
			{
				axisSprite.transform.localScale 
					= new Vector3 ( 
					               graph_.GetXLocationForPoint(graph_.settings.xView.y) - graph_.GetXLocationForPoint(graph_.settings.xView.x), 
					               definition_.axisLineWidth,
					               axisSprite.transform.localScale.z);

				break;
			}
			case EXYDirection.Y:
			{
				axisSprite.transform.localScale 
					= new Vector3 ( 
					               definition_.axisLineWidth,
					               graph_.GetYLocationForPoint(graph_.settings.yView.y) - graph_.GetYLocationForPoint(graph_.settings.yView.x), 
					               axisSprite.transform.localScale.z);
				break;
			}
		}

	}
	
	public bool IsInView()
	{
		bool result = false;
		switch (Direction)
		{
			case EXYDirection.X:
			{
				result = graph_.settings.IsYInView(Value);
				break;
			}
			case EXYDirection.Y:
			{
				result = graph_.settings.IsXInView(Value);
				break;
			}
		}
		return result;
	}
}
