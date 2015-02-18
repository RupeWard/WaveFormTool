using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadGraphPanel : SingletonSceneLifetime< LoadGraphPanel >  
{
	private static readonly bool DEBUG_IO = true;

	public UILabel messageLabel;
	public UILabel confirmMessageLabel;

	public UISprite background_;

	public UILabel titleLabel;

	public GraphPanel graphPanel;

	public GameObject confirmPopUp;

	public UIPopupList filenameSelection;

	public void Start()
	{
//		HUDManager.Instance.AddPopup (gameObject);		

		this.gameObject.SetActive (false);
//		modeSelection.items.Add ( );
	}

	public void Init ( GraphPanel panel )
	{
		graphPanel = panel;
		SetUpFileList();
	}
	
	public Vector2 Size()
	{
		return new Vector2 ( background_.transform.localScale.x, background_.transform.localScale.y );
	}

	private void SetUpFileList()
	{
		string extn = "*."+graphPanel.FilenameExtension;

		System.Text.StringBuilder sb = new System.Text.StringBuilder ( );
		sb.Append ( "Files with extension \"" + extn + "\" in " + GraphIO.SaveFolder +"...");
		filenameSelection.items.Clear ( );
		filenameSelection.selection = "";
		System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo ( GraphIO.SaveFolder);
		if (dirInfo.Exists)
		{
			System.IO.FileInfo[] fileInfos = dirInfo.GetFiles(extn);
			foreach (System.IO.FileInfo f in fileInfos)
			{
				filenameSelection.items.Add(f.Name);
				sb.Append("\n"+f.FullName);
			}
		}
		Debug.Log(sb.ToString());
	}
	 
	public void SetActive(bool b)
	{
		if (b)
		{
			LoadGraphPanel.Instance.SetActive ( false );
			HUDManager.Instance.CloseAllPopupsExcept(gameObject);
		}
		gameObject.SetActive (b);
		confirmPopUp.SetActive ( false );
	}
	
	public void OnLoadButtonClicked()
	{
		// TODO do load
		if ( DEBUG_IO )
		{
			Debug.Log("Load");
		}
		string filename = filenameSelection.selection;
		if ( filename.Length > 0 )
		{
			graphPanel.LoadFromFile ( filename );
		}
	}

	public void OnDeleteButtonClicked()
	{
		if ( DEBUG_IO )
		{
			Debug.Log("Delete");
		}
		string msg = "Delete "+ filenameSelection.selection;
		msg = msg + "\nAre you sure?";
		confirmMessageLabel.text = msg;
		
		confirmPopUp.gameObject.SetActive ( true );
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
			Debug.Log("Confirm Delete");
		}
		StartCoroutine ( DeleteFileCR ( ) );
	}

	private IEnumerator DeleteFileCR()
	{
		System.IO.FileInfo fileInfo = new System.IO.FileInfo ( GraphIO.SaveFolder + filenameSelection.selection );
		if (fileInfo.Exists)
		{
			fileInfo.Delete();
			yield return null;
			SetUpFileList ( );
		}

		confirmPopUp.gameObject.SetActive(false);
		yield return null;
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
		if ( graphPanel != null )
		{
			SetUpFileList();
		}
	}

	public void OnDisable()
	{
		confirmPopUp.SetActive ( false );
	}

	private void SetTitleButtonLabel()
	{
		titleLabel.text = "Load " + graphPanel.SaveTypeString;
	}

}
