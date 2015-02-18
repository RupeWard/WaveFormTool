using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveGraphPanel : SingletonSceneLifetime< SaveGraphPanel >  
{
	private static readonly bool DEBUG_IO = true;

	public UIInput filenameInput;
	public UILabel filenameInputLabel;
	public UILabel messageLabel;
	public UILabel confirmMessageLabel;

	public UISprite background_;

	public UILabel titleLabel;

	public GraphPanel graphPanel;

	public GameObject confirmPopUp;

	private string filename_;

	public void Start()
	{
//		HUDManager.Instance.AddPopup (gameObject);		

		this.gameObject.SetActive (false);
//		modeSelection.items.Add ( );
	}

	public void Init ( GraphPanel panel )
	{
		graphPanel = panel;
		filenameInputLabel.text = panel.DefaultFilename;
	}
	
	public Vector2 Size()
	{
		return new Vector2 ( background_.transform.localScale.x, background_.transform.localScale.y );
	}

	public void SetActive(bool b)
	{
		if (b)
		{
			LoadGraphPanel.Instance.SetActive(false);
			HUDManager.Instance.CloseAllPopupsExcept(gameObject);
		}
		gameObject.SetActive (b);
		confirmPopUp.SetActive ( false );
	}

	public void OnFilenameInputChanged(string str)
	{
		char[] arr = str.ToCharArray();
		
		arr = System.Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
		                                      || c == '_'
		                                      || c == '-')));
		filenameInputLabel.text = new string(arr);
	}

	public void OnSaveButtonClicked()
	{
		bool bFoundFolder = false;
		
		System.IO.DirectoryInfo saveFolder = new System.IO.DirectoryInfo ( GraphIO.SaveFolder );
		if (!saveFolder.Exists)
		{
			saveFolder.Create();
			saveFolder = new System.IO.DirectoryInfo ( GraphIO.SaveFolder );
			if (!saveFolder.Exists)
			{
				Debug.LogError ("Couldn't create save folder '"+saveFolder+"'");
			}
			else
			{
				bFoundFolder = true;
				if (DEBUG_IO)
				{
					Debug.Log ("Created save folder '"+saveFolder+"'");
				}
			}
		}
		else 
		{
			if (DEBUG_IO)
			{
				Debug.Log ("Found save folder '"+saveFolder+"'");
			}
			bFoundFolder = true;
		}
		if (bFoundFolder)
		{
			filename_ = filenameInputLabel.text + "." + graphPanel.FilenameExtension;
			string filePath = GraphIO.SaveFolder + filename_;
			
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
		
			if (DEBUG_IO)
			{
				Debug.Log ("Filepath is '"+filePath+"'");
			}

			string msg = "Save graph to "+filename_;
			if (fileInfo.Exists)
			{
				msg = msg+ "\nIt already exists!";
			}
			msg = msg + "\nAre you sure?";
			confirmMessageLabel.text = msg;
			
			confirmPopUp.gameObject.SetActive ( true );
		}
	}
	
	public void OnCloseButtonClicked()
	{
		if ( DEBUG_IO )
		{
			Debug.Log("Close");
		}
		this.gameObject.SetActive (false);
	}

	public void OnConfirmButtonClicked()
	{
		if ( DEBUG_IO )
		{
			Debug.Log("Confirm");
		}
		graphPanel.SaveToFile ( filename_ );
		confirmPopUp.SetActive(false);
		this.SetActive(false);
	}

	public void OnCancelButtonClicked()
	{
		if ( DEBUG_IO )
		{
			Debug.Log("Cancel");
		}
		confirmPopUp.gameObject.SetActive ( false );
	}

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
		confirmPopUp.SetActive ( false );
	}

	private void SetTitleButtonLabel()
	{
		titleLabel.text = "Save " + graphPanel.SaveTypeString;
	}

}
