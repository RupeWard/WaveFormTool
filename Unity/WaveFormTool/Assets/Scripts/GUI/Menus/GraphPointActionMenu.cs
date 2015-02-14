using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointActionMenu : Menu 
{
	public static readonly string nullOption = "ACTIONS";
	public static readonly string deletePointOption = "Delete";

	private GraphPoint point_ = null;
	private GraphPointPanel panel_;

	public void Awake ()
	{
		AddOption ( nullOption, true );
		AddOption (deletePointOption, false);
	}

	public void SetPoint(GraphPointPanel panel)
	{
		panel_ = panel;
		point_ = panel_.Point;
		SetOptionActive(deletePointOption, point_!= null && false == point_.IsFixed && point_.IsFunctional);
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
		}
	}

}
