using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiManager : SingletonApplicationLifetime< GuiManager >
{
	public GraphPanel waveGraphPanel;
	public UIRoot myRoot;

	public Transform mainMenuBarHeightProvider;
	public GameObject mainMenuBar;

	private Vector2 screenDims_;
	public Vector2 ScreenDims
	{
		get { return screenDims_; }
	}
	public float ScreenWidth 
	{
		get { return screenDims_.x; } 
	}
	public float ScreenHeight 
	{
		get { return screenDims_.y; } 
	}

	private float margin_ = 10f;
	public float Margin
	{
		get { return margin_; }
	}

	private List< GameObject > popUps_  = new List< GameObject >();

	protected override void PostAwake()
	{
		screenDims_.y = myRoot.activeHeight;
		screenDims_.x = myRoot.activeHeight * Screen.width / Screen.height;
		Debug.Log ("GuiManager.Awake: Screen is " + Screen.width + " x " + Screen.height
		           + " so root dims = "+screenDims_);
	}

	public void Start()
	{
		mainMenuBar.transform.SetLocalXYPosition(0f, -0.5f * mainMenuBarHeightProvider.localScale.y - margin_);
		float graphTop = -1f *(mainMenuBarHeightProvider.localScale.y + 2 * margin_);
		float graphBottom = -1f * (ScreenHeight - 50f);
		waveGraphPanel.init ( new Vector2 (ScreenWidth - 2*margin_ ,graphTop - graphBottom), 
		                     new Vector2 (0f ,0.5f * (graphTop+graphBottom)) );

		float popUpTop = graphTop - 5;

		Vector2 popUpSize = ToneGeneratorPanel.Instance.Size ();
		float popUpBottom = popUpTop - popUpSize.y;
		ToneGeneratorPanel.Instance.transform.SetLocalXYPosition(0f, 0.5f * (popUpTop + popUpBottom));

		popUpSize = WaveGeneratorPanel.Instance.Size ();
		popUpBottom = popUpTop - popUpSize.y;
		WaveGeneratorPanel.Instance.transform.SetLocalXYPosition(0f, 0.5f * (popUpTop + popUpBottom));

	}

	public void AddPopup(GameObject p)
	{
		if (!popUps_.Contains (p))
		{
			popUps_.Add(p);
		}
	}

	public void CloseAllPopupsExcept(GameObject popup)
	{
		foreach (GameObject p in popUps_)
		{
			if (p != popup)
			{
				p.SetActive(false);
			}
		}
	}

	public void Update()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
				return;
			}
			if (Input.GetKey(KeyCode.Menu))
			{
				return;
			}
		}

	}

}
