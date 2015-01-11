using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointActionMenu : Menu 
{
	public static readonly string fixPointOption = "Fix";
	public static readonly string freePointOption = "Free";

	private GraphPoint point_ = null;

	public void Awake ()
	{
		AddOption (fixPointOption, false);
		AddOption (freePointOption, false);
	}

	public void SetPoint(GraphPoint p)
	{
		point_ = p;
		SetOptionActive(freePointOption, point_!= null && point_.IsFixed);
		SetOptionActive(fixPointOption, point_!= null && !point_.IsFixed);
	}
	
	public override void OnOptionSelected(string option)
	{
		if (point_ != null)
		{
			if (option == fixPointOption)
			{
				if (point_.IsFunctional)
				{
					point_.IsFixed = true;
					Debug.LogWarning ("FixPoint : "+point_.DebugDescribe());
				}
			}
			else if (option == freePointOption)
			{
				if (point_.IsFunctional)
				{
					point_.IsFixed = false;
					Debug.LogWarning ("FreePoint"+point_.DebugDescribe());
				}
			}
			else
			{
				Debug.LogWarning ("Menu '" + gameObject.name + "' doesn't know about option '" + option + "'");
			}
		}
	}

}
