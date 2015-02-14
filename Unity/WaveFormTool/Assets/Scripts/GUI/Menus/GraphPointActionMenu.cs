using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointActionMenu : Menu 
{
	public static readonly string nullOption = "ACTIONS";
	public static readonly string deletePointOption = "Delete";
	public static readonly string addBeforeOption = "Add Before";
	public static readonly string addAfterOption = "Add After";

	public static readonly string addAllBeforeOption = "Add All Before";
	public static readonly string addAllAfterOption = "Add All After";

	private GraphPoint point_ = null;
	private GraphPointPanel panel_;

	public void Awake ()
	{
		AddOption ( nullOption, true );
		AddOption (deletePointOption, false);
		AddOption (addBeforeOption, false);
		AddOption (addAfterOption, false);
		AddOption (addAllBeforeOption, false);
		AddOption (addAllAfterOption, false);
	}

	public void SetPoint(GraphPointPanel panel)
	{
		panel_ = panel;
		point_ = panel_.Point;
		SetOptionActive(deletePointOption, point_!= null && false == point_.IsFixed && point_.IsFunctional);
		SetOptionActive(addBeforeOption, 
		                point_!= null  
		                && point_.PreviousPoint != null && point_.PreviousPoint.IsFunctional);
		SetOptionActive(addAfterOption, 
		                point_!= null && point_.IsFunctional 
		                && point_.PreviousPoint != null);
		SetOptionActive(addAllBeforeOption, 
		                point_!= null  
		                && point_.PreviousPoint != null && point_.PreviousPoint.IsFunctional);
		SetOptionActive(addAllAfterOption, 
		                point_!= null && point_.IsFunctional 
		                && point_.PreviousPoint != null);
	}
	
	public override void OnOptionSelected(string option)
	{
		if (point_ != null)
		{
			if (option == deletePointOption)
			{
				point_.graphPanel.DeletePoint(point_);
				panel_.SetPoint(null);
			}
			else if (option == addBeforeOption)
			{
				point_.graphPanel.AddPointBefore(point_);
			}
			else if (option ==  addAfterOption)
			{
				point_.graphPanel.AddPointAfter(point_);
			}
			else if (option == addAllBeforeOption)
			{
				point_.graphPanel.AddAllPointsBefore(point_);
			}
			else if (option ==  addAllAfterOption)
			{
				point_.graphPanel.AddAllPointsAfter(point_);
			}
		}
	}

}
