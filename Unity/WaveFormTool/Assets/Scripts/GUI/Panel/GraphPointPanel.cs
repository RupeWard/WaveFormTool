using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPointPanel : MonoBehaviour  
{
	public enum EPositions
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	private Dictionary< EPositions, GameObject > buttonsByPositions_ = new Dictionary<EPositions, GameObject> ();
	private Dictionary< EPositions, Vector2 > positionsByPositions_ = new Dictionary<EPositions, Vector2> ();

	public GraphPointActionMenu actionMenu;

	public GraphPanel graphPanel;
	public UILabel messageLabel;
	public UISprite background_;

	public UILabel xValueLabel;
	public UILabel yValueLabel;
	public UILabel fixFreeButtonLabel;

	public UIInput xInput;
	public UIInput yInput;

	public UIPopupList pointMoverSelection;

	public GameObject topLeftButton;
	public GameObject topRightButton;
	public GameObject bottomLeftButton;
	public GameObject bottomRightButton;
	public GameObject closeButton;

	private Vector2 topLeftPosition;
	private Vector2 topRightPosition;
	private Vector2 bottomLeftPosition;
	private Vector2 bottomRightPosition;

	public Vector2 TopLeftPosition
	{
		set 
		{ 
			topLeftPosition = value;
			addPosition(EPositions.TopLeft, topLeftPosition);
		}
	}

	public Vector2 TopRightPosition
	{
		set 
		{ 
			topRightPosition = value;
			addPosition(EPositions.TopRight, topRightPosition);
		}
	}

	public Vector2 BottomLeftPosition
	{
		set 
		{ 
			bottomLeftPosition = value;
			addPosition(EPositions.BottomLeft, bottomLeftPosition);
		}
	}
	public Vector2 BottomRightPosition
	{
		set 
		{ 
			bottomRightPosition = value;
			addPosition(EPositions.BottomRight, bottomRightPosition);
		}
	}

	private void addPosition(EPositions p, Vector2 v)
	{
		positionsByPositions_ [p] = v;
	}

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

		buttonsByPositions_ [EPositions.TopLeft] = topLeftButton;
		buttonsByPositions_ [EPositions.TopRight] = topRightButton;
		buttonsByPositions_ [EPositions.BottomLeft] = bottomLeftButton;
		buttonsByPositions_ [EPositions.BottomRight] = bottomRightButton;

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
		SetUpFixFreeButton ();
		actionMenu.SetPoint (point_);
		if (point_ != null)
		{
			if (point_.Point.y >= point_.graphPanel.graphSettings.YViewCentre)
			{ 
				// top
				if (point_.Point.x >= point_.graphPanel.graphSettings.XViewCentre)
				{
					SetPosition(EPositions.BottomLeft);
				}
				else
				{
					SetPosition(EPositions.BottomRight);
				}
			}
			else
			{
				// bottom
				if (point_.Point.x >= point_.graphPanel.graphSettings.XViewCentre)
				{
					SetPosition(EPositions.TopLeft);
				}
				else
				{
					SetPosition(EPositions.TopRight);
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

	private void SetUpFixFreeButton()
	{
		if (point_ == null)
		{
			fixFreeButtonLabel.text = "";
		}
		else
		{
			fixFreeButtonLabel.text = (point_.IsFixed) ? ("Free") : ("Fix");
			fixFreeButtonLabel.color = (point_.IsFixed) ? (Color.red) : (Color.black);
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
		//		Debug.Log ("Close clicked");
		SetPoint (null);
		this.gameObject.SetActive (false);
	}

	public void Update()
	{
		if (point_ != null && point_.IsDataDirty)
		{
			SetUpFixFreeButton ();
			actionMenu.SetPoint(point_);
			point_.ClearDataDirty();
		}
	}

	public void OnTopLeftButtonPressed()
	{
		SetPosition (EPositions.TopLeft);
	}
	public void OnTopRightButtonPressed()
	{
		SetPosition (EPositions.TopRight);
	}
	public void OnBottomLeftButtonPressed()
	{
		SetPosition (EPositions.BottomLeft);
	}
	public void OnBottomRightButtonPressed()
	{
		SetPosition (EPositions.BottomRight);
	}

	public void SetPosition(EPositions p)
	{
		this.transform.SetLocalXYPosition ( positionsByPositions_[p]);
		SetPositionButtons (p);
	}

	private void SetPositionButtons(EPositions p)
	{
		foreach (EPositions pos in buttonsByPositions_.Keys)
		{
			if (pos == p)
				buttonsByPositions_[pos].SetActive(false);
			else
				buttonsByPositions_[pos].SetActive(true);
		}
		closeButton.transform.SetLocalXYPosition ( buttonsByPositions_[p].transform.localPosition );
	}

	public void OnMoveRightButtonPressed()
	{
		if (point_ != null)
		{
			if (point_.graphPanel.RangeEnd == point_)
			{
				point_.graphPanel.RangeStart.OnSelected();
			}
			else if (point_.NextPoint != null && point_.NextPoint.IsFunctional)
			{
				point_.NextPoint.OnSelected();
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
			if (point_.PreviousPoint != null && point_.PreviousPoint.IsFunctional)
			{
				point_.PreviousPoint.OnSelected();
			}
		}
	}

	public void OnFixFreeButtonClicked()
	{
		if (point_ != null)
		{
			if (point_.IsFunctional)
			{
				if (!point_.graphPanel.IsCreatingGraph)
				{
					Debug.LogWarning ("FixPoint : " + point_.DebugDescribe ());
				}
				point_.IsFixed = !point_.IsFixed;
			}
		}
	}

}
