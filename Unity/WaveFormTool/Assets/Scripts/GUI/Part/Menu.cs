using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Menu : MonoBehaviour 
{
	private static bool DEBUG_MENU = false;

	public UIPopupList popUpList;

	private List< string > allOptions_ = new List< string> ();

	public void AddOption(string s, bool b)
	{
		if (!allOptions_.Contains (s))
		{
			if (DEBUG_MENU)
				Debug.LogWarning ("Menu '" + gameObject.name + "' adding option '" + s + "'");
			allOptions_.Add(s);
			SetOptionActive(s, b);
		}
		else
		{
			if (DEBUG_MENU)
				Debug.LogWarning ("Menu '" + gameObject.name + "' already has option '" + s + "'");
		}
	}

	public void SetOptionActive(string s, bool b)
	{
		if (allOptions_.Contains (s))
		{
			if (popUpList.items.Contains (s) == b)
			{
				Debug.LogWarning ("Menu '" + gameObject.name + "' option '" + s + "' is already "+b);
			}
			else
			{
				if (b)
				{
					if (DEBUG_MENU)
						Debug.Log ("Menu '" + gameObject.name + "' adding option '" + s + "'");
					popUpList.items.Add(s);
				}
				else
				{
					if (DEBUG_MENU)
						Debug.Log ("Menu '" + gameObject.name + "' removing option '" + s + "'");
					popUpList.items.Remove(s);
				}
			}
		}
		else
		{
			Debug.LogWarning ("Menu '" + gameObject.name + "' has no option '" + s + "'");
		}
	}

	public void ResetOptions()
	{
		popUpList.items.Clear ();
		foreach (string s in allOptions_)
		{
			AddOption(s, true);
		}
	}

	public abstract void OnOptionSelected(string s);


}
