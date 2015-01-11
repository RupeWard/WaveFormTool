using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointActionMenu : Menu 
{
	static readonly string fixPointOption = "Fix";
	static readonly string freePointOption = "Free";

	private GraphPoint point_ = null;

	public void Awake ()
	{
		AddOption (fixPointOption, false);
		AddOption (freePointOption, false);
	}

	public void SetPoint(GraphPoint p)
	{
		point_ = p;
		SetOptionActive(freePointOption, point_.IsFixed);
		SetOptionActive(fixPointOption, !point_.IsFixed);
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
					Debug.LogWarning ("FixPoint");
				}
			}
			else if (option == freePointOption)
			{
				if (point_.IsFunctional)
				{
					point_.IsFixed = false;
					Debug.LogWarning ("FreePoint");
				}
			}
			else
			{
				Debug.LogWarning ("Menu '" + gameObject.name + "' doesn't know about option '" + option + "'");
			}
		}
	}

}
