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

	private GraphPoint point_ = null;
	public GraphPoint Point
	{
		get { return point_; }
	}

	public void Start()
	{
		GuiManager.Instance.AddPopup (this.gameObject);
		this.gameObject.SetActive (false);
	}
	
	public void SetPoint(GraphPoint p)
	{
		point_ = p;
		if (point_ != null)
		{
			xValueLabel.text = string.Format("{0:0.####}",point_.Point.x); 
			yValueLabel.text = string.Format("{0:0.####}",point_.Point.y);  
			stateLabel.text = (point_.IsFixed)?("Fixed"):("");
		}
		else
		{
			xValueLabel.text = "-"; 
			yValueLabel.text = "-";  
			stateLabel.text = "NULL";
		}
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
			GuiManager.Instance.CloseAllPopupsExcept(gameObject);
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
		}
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
