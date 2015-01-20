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

	public UIPopupList pointMoverSelection;

	public Vector2 topLeftPosition;
	public Vector2 topRightPosition;
	public Vector2 bottomLeftPosition;
	public Vector2 bottomRightPosition;

	private GraphPoint point_ = null;
	public GraphPoint Point
	{
		get { return point_; }
	}

	public GraphPointMoverBase[] pointMovers_ = new GraphPointMoverBase[0]{};
	private Dictionary< string, GraphPointMoverBase > moversDB_ = new Dictionary<string, GraphPointMoverBase> ();

	public void Start()
	{
//		HUDManager.Instance.AddPopup (this.gameObject);

		this.gameObject.SetActive (false);
		xInput.enabled = false;

		foreach (GraphPointMoverBase mover in pointMovers_)
		{
			if (moversDB_.ContainsKey(mover.moverName))
			{
				Debug.LogError ("Duplicate mover name");
			}
			else
			{
				moversDB_.Add( mover.moverName, mover);
				pointMoverSelection.items.Add (mover.moverName);			}
		}
		pointMoverSelection.enabled = true;
	}

	private GraphPointMoverBase GetPointMover()
	{
		string s = pointMoverSelection.selection;
		GraphPointMoverBase result = null;
		if (moversDB_.ContainsKey (s))
		{
			result = moversDB_[s];
		}
		else
		{
			Debug.LogError ("No such mover: '"+s+"'");
		}
		return result;
	}

	public void OnYChangeStrategyChange()
	{
		GraphPointMoverBase e = GetPointMover ();
		Debug.Log ("Y change strategy changed to "+e.DebugDescribe());
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
		if (point_ != null)
		{
			if (point_.Point.y >= point_.graphPanel.settings.YViewCentre)
			{ 
				// top
				if (point_.Point.x >= point_.graphPanel.settings.XViewCentre)
				{
					this.transform.SetLocalXYPosition(bottomLeftPosition);
				}
				else
				{
					this.transform.SetLocalXYPosition(bottomRightPosition);
				}
			}
			else
			{
				// bottom
				if (point_.Point.x >= point_.graphPanel.settings.XViewCentre)
				{
					this.transform.SetLocalXYPosition(topLeftPosition);
				}
				else
				{
					this.transform.SetLocalXYPosition(topRightPosition);
				}
			}
		}
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
			GraphPointMoverBase mover = GetPointMover();
			if (mover != null)
			{
				graphPanel.MovePointY(point_, GetPointMover(), newY);
			}
			else
			{
				messageLabel.text = "No PointMover type is selected";
			}
		}
		else
		{
			messageLabel.text = "Must be a number!";
		}

		SetYLabel ();
	}



	public void OnCloseButtonClicked()
	{
		if (!HUDManager.Instance.clickHandled)
		{
			HUDManager.Instance.clickHandled = true;
			//		Debug.Log ("Close clicked");
			SetPoint (null);
			this.gameObject.SetActive (false);
		}
		else
		{
			Debug.LogWarning("Not closing because clickHandled");
		}
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

	public void OnTopLeftButtonPressed()
	{
		this.transform.SetLocalXYPosition (topLeftPosition);
	}
	public void OnTopRightButtonPressed()
	{
		this.transform.SetLocalXYPosition (topRightPosition);
	}
	public void OnBottomLeftButtonPressed()
	{
		this.transform.SetLocalXYPosition (bottomLeftPosition);
	}
	public void OnBottomRightButtonPressed()
	{
		this.transform.SetLocalXYPosition (bottomRightPosition);
	}

	public void OnMoveRightButtonPressed()
	{
		if (point_ != null)
		{
			if (point_.graphPanel.RangeEnd == point_)
			{
				point_.graphPanel.RangeStart.OnSelected();
			}
			else if (point_.nextPoint_ != null && point_.nextPoint_.IsFunctional)
			{
				point_.nextPoint_.OnSelected();
			}
		}
	}

	public void OnMoveLeftButtonPressed()
	{
		if (point_ != null)
		{
			if (point_.graphPanel.RangeStart == point_)
			{
				point_.graphPanel.RangeEnd.OnSelected();
			}
			if (point_.previousPoint_ != null && point_.previousPoint_.IsFunctional)
			{
				point_.previousPoint_.OnSelected();
			}
		}
	}
}
