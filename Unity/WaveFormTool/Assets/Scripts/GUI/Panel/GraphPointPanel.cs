using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointPanel : MonoBehaviour  
{
	public GraphPointActionMenu actionMenu;

	public GraphPanel graphPanel;
	public UILabel messageLabel;
	public UISprite background_;

	public UILabel xValueLabel;
	public UILabel yValueLabel;
	public UILabel stateLabel;

	public UIInput xInput;
	public UIInput yInput;

	public UIPopupList yChangeStrategy;

	private GraphPoint point_ = null;
	public GraphPoint Point
	{
		get { return point_; }
	}

	public void Start()
	{
		HUDManager.Instance.AddPopup (this.gameObject);
		this.gameObject.SetActive (false);
		xInput.enabled = false;

		EYChangeStrategy[] strategies = (EYChangeStrategy[])System.Enum.GetValues (typeof(EYChangeStrategy));
		foreach (EYChangeStrategy e in strategies)
		{
			if (e != EYChangeStrategy.None)
			{
				yChangeStrategy.items.Add (e.ToString());
			}
		}
		yChangeStrategy.enabled = true;
	}

	private EYChangeStrategy ParseYChangeStrategy(string s)
	{
		EYChangeStrategy result = EYChangeStrategy.None;
		EYChangeStrategy[] strategies = (EYChangeStrategy[])System.Enum.GetValues (typeof(EYChangeStrategy));
		foreach (EYChangeStrategy e in strategies)
		{
			if (s == e.ToString())
			{
				result = e;
				break;
			}
		}
		return result;
	}

	public void OnYChangeStrategyChange()
	{
		EYChangeStrategy e = ParseYChangeStrategy (yChangeStrategy.selection);
		Debug.Log ("Y change strategy changed to "+e);
	}

	private void SetXLabel()
	{
		xValueLabel.text = (point_ != null)?(string.Format("{0:0.####}",point_.Point.x)):(""); 
	}

	private void SetYLabel()
	{
		yValueLabel.text = (point_ != null)?(string.Format ("{0:0.####}", point_.Point.y)):(""); 
	}

	private void SetXYLabels()
	{
		SetXLabel ();
		SetYLabel ();
	}

	public void SetPoint(GraphPoint p)
	{
		point_ = p;
		SetXYLabels();
		SetStateLabel ();
		actionMenu.SetPoint (point_);
	}

	public Vector2 Size()
	{
		return background_.transform.GetLocalXYSize();
	}

	// TODO BASE CLASS
	public void SetActive(bool b)
	{
		if (b && !graphPanel.IsCreatingGraph)
		{
			HUDManager.Instance.CloseAllPopupsExcept(gameObject);
		}
		gameObject.SetActive (b);
	}

	private void SetStateLabel()
	{
		if (point_ == null)
		{
			stateLabel.text = "NULL";
		}
		else
		{
			stateLabel.text = (point_.IsFixed) ? ("Fixed") : ("");
			yInput.enabled = !point_.IsFixed;
		}
	}

	public void OnYValueChanged(string s)
	{
		float newY;
		if (float.TryParse (s, out newY))
		{
			graphPanel.MovePointY(point_, newY, ParseYChangeStrategy( yChangeStrategy.selection));
		}
		else
		{
			messageLabel.text = "Must be a number!";
		}

		SetYLabel ();
	}



	public void OnCloseButtonClicked()
	{
//		Debug.Log ("Close clicked");
		SetPoint (null);
		this.gameObject.SetActive (false);
	}

	public void Update()
	{
		if (point_ != null && point_.IsDataDirty)
		{
			SetStateLabel ();
			actionMenu.SetPoint(point_);
			point_.ClearDataDirty();
		}
	}
}
