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
			if (graphPanel.settings.IsYInRange(newY))
			{
				point_.SetY(newY);
			}
			else
			{
				messageLabel.text = "Out of range "+graphPanel.settings.yRange;
			}
		}
		else
		{
			messageLabel.text = "Must be a number!";
		}

		SetXLabel ();
	}



	public void OnCloseButtonClicked()
	{
//		Debug.Log ("Close clicked");
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
