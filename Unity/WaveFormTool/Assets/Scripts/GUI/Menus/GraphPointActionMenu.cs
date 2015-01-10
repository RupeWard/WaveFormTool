using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointActionMenu : Menu 
{
	static readonly string fixPointOption = "Fix";

	private GraphPoint point_ = null;

	public void Awake ()
	{
		AddOption (fixPointOption, true);
	}

	public void SetPoint(GraphPoint p)
	{
		point_ = p;
	}
	
	public override void OnOptionSelected(string option)
	{
		if (option == fixPointOption)
		{
//			Debug.Log ("Menu '" + gameObject.name + "' selected option '" + playGraphOption + "'");
			Debug.LogWarning ("fixPoint not implemented");
		}
		else
		{
			Debug.LogWarning ("Menu '" + gameObject.name + "' doesn't know about option '" + option + "'");
		}
	}

}
