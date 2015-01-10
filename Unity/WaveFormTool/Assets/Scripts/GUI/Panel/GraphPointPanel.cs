using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointPanel : MonoBehaviour  
{
	public Menu actionMenu;

	public GraphPanel graphPanel;
	public UILabel messageLabel;
	public UISprite background_;

	public UILabel xValueLabel;
	public UILabel yValueLabel;

	private GraphPoint point_;

	public void Start()
	{
		GuiManager.Instance.AddPopup (this.gameObject);
		this.gameObject.SetActive (false);
	}

	public void SetPoint(GraphPoint p)
	{
		point_ = p;
		xValueLabel.text = string.Format("{0:0.####}",point_.Point.x); 
		yValueLabel.text = string.Format("{0:0.####}",point_.Point.y);  
	}

	public Vector2 Size()
	{
		return new Vector2 ( background_.transform.localScale.x, background_.transform.localScale.y );
	}

	// TODO BASE CLASS
	public void SetActive(bool b)
	{
		if (b)
		{
			GuiManager.Instance.CloseAllPopupsExcept(gameObject);
		}
		gameObject.SetActive (b);
	}

	public void OnCloseButtonClicked()
	{
//		Debug.Log ("Close clicked");
		this.gameObject.SetActive (false);
	}
}
