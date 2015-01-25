using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindowMenu : Menu 
{
	public static readonly string baseWaveOption = "BaseWave";
	public static readonly string envelopeOption = "Envelope";

	public void Awake ()
	{
		// Not needed
		AddOption (baseWaveOption, true);
		AddOption (envelopeOption, true);
	}
	
	public override void OnOptionSelected(string option)
	{
		if (option == baseWaveOption)
		{
			HUDManager.Instance.BaseWaveObject.SetActive(true);
			HUDManager.Instance.EnvelopeObject.SetActive(false);
			SetOptionActive(baseWaveOption, false);
			SetOptionActive(envelopeOption, true);
			HUDManager.Instance.HandleWindowChanged();
		}
		else if (option == envelopeOption)
		{
			HUDManager.Instance.BaseWaveObject.SetActive(false);
			HUDManager.Instance.EnvelopeObject.SetActive(true);
			SetOptionActive(baseWaveOption, true);
			SetOptionActive(envelopeOption, false);
			HUDManager.Instance.HandleWindowChanged();
		}
		else
		{
			Debug.LogWarning ("Menu '" + gameObject.name + "' doesn't know about option '" + option + "'");
		}
}

}
