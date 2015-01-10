using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiManager : SingletonApplicationLifetime< GuiManager >
{
	public GraphPanel waveGraphPanel;

	public Transform mainMenuBarHeightProvider;
	public GameObject mainMenuBar;

	private float margin_ = 10f;
	public float Margin
	{
		get { return margin_; }
	}

	private List< GameObject > popUps_  = new List< GameObject >();

	public void Start()
	{
		Debug.Log ("GuiManager.Awake: Screen is " + Screen.width + " x " + Screen.height);
		mainMenuBar.transform.localPosition 
			= new Vector3(0f, -0.5f * mainMenuBarHeightProvider.localScale.y - margin_, 
				mainMenuBar.transform.localPosition.z);
		float graphTop = -1f *(mainMenuBarHeightProvider.localScale.y + 2 * margin_);
		float graphBottom = -1f * (Screen.height - 50f);
		waveGraphPanel.init ( new Vector2 (Screen.width - 2*margin_ ,graphTop - graphBottom), 
		                     new Vector2 (0f ,0.5f * (graphTop+graphBottom)) );

		float popUpTop = graphTop - 5;

		Vector2 popUpSize = ToneGeneratorPanel.Instance.Size ();
		float popUpBottom = popUpTop - popUpSize.y;
		ToneGeneratorPanel.Instance.transform.localPosition 
			= new Vector3 (0f, 0.5f * (popUpTop + popUpBottom), 
			               ToneGeneratorPanel.Instance.transform.localPosition.z);

		popUpSize = WaveGeneratorPanel.Instance.Size ();
		popUpBottom = popUpTop - popUpSize.y;
		WaveGeneratorPanel.Instance.transform.localPosition 
			= new Vector3 (0f, 0.5f * (popUpTop + popUpBottom), 
			               WaveGeneratorPanel.Instance.transform.localPosition.z);

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
}
