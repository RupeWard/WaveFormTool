using UnityEngine;
using System.Collections;

public class WaveGraphPanel : GraphPanel 
{
	private static readonly bool DEBUG_TONE_GEN = false;

	public int numSamples_ = 100;

	public Transform playButton;

	private static readonly bool DEBUG_GENERATION = true;

	private WaveFormDataRegular waveFormData_ = null;

	private bool isDirty_ = false;
	public override void HandleDataChange ()
	{
		isDirty_ = true;
	}

	private bool isCreating_ = false;

	protected override void postInit()
	{
		Vector2 pos = viewMinInput.transform.GetLocalXYPosition ();
		pos.y += 60f;
		pos.x += 40f;
		playButton.SetLocalXYPosition ( pos );
	}

	public void Update()
	{
		if (isDirty_)
		{
			if (isCreating_)
			{
				if (DEBUG_GENERATION)
				{
					Debug.Log("WGP: restarting");
				}
				isCreating_ = false;
				messageLabel.color = Color.red;
				messageLabel.text = "Restarted tone creation";
				StopCoroutine ("CreateDataCR");
			}
			else
			{
				if (DEBUG_GENERATION)
				{
					Debug.Log("WGP: starting");
				}
				messageLabel.color = Color.red;
				messageLabel.text = "Started tone creation";
			}
			isDirty_ = false;
			StartCoroutine("CreateDataCR");
		}
	}

	private IEnumerator CreateDataCR()
	{
		System.Text.StringBuilder sb = null;
		int numGraphPoints = NumGraphPoints ();

		if (DEBUG_TONE_GEN)
		{
			sb = new System.Text.StringBuilder();
			Debug.Log("WGP: START. num = "+numSamples_+", with "+numGraphPoints+" graph points");
		}

		// validate points
		// validate settings

		float[] newSamples = new float[ numSamples_ ];

		float phase = 0f;
		float step = 1f / numSamples_;

		GraphPoint p = RangeStart;
		int numDone = 0;
		newSamples [0] = p.Point.y;

		int numSkipped = 0;

		System.Text.StringBuilder ptssb = null;
		if (DEBUG_TONE_GEN)
		{
			ptssb = new System.Text.StringBuilder ();
			ptssb.Append(""+phase+","+ newSamples[0]+"\n");
		}

		phase += step;

		GraphPoint nextPt = null;


		while (p!= null && numDone < numSamples_ && numSkipped < numGraphPoints)
		{
			if (DEBUG_TONE_GEN)
			{
				Debug.Log("PHASE is "+ phase);
			}

			nextPt = p.NextPoint;
			if (DEBUG_TONE_GEN)
			{
				Debug.Log("NextPoint is "+ UnityExtensions.DebugDescribe(nextPt));
			}
			float nextX = nextPt.Point.x;
			float nextY = nextPt.Point.y;

			if (nextPt == null || nextPt == RangeEnd)
			{
				nextX = RangeEnd.Point.x;
				if (DEBUG_TONE_GEN)
				{
					Debug.Log("Found the penultimate graph point, with numDone =  "+numDone+", so nextX = "+nextX);
				}

			}

			if (p.Point.x <= phase && nextX > phase && p != RangeEnd)
			{
				float x1 = p.Point.x;
				float y1 = p.Point.y;

				float val;
				if (phase == x1)
				{
					val = y1;
				}
				else if (nextX == x1)
				{
					val = y1;
					Debug.LogError("x1 == x2");
				}
				else
				{
					val = y1 + (nextY-y1) * (phase - x1)/(nextX-x1);
				}

				numDone++;
				newSamples[numDone] = val;
				if (DEBUG_TONE_GEN)
				{
					Debug.Log("Set point at "+phase+", "+ val);
				}
				if (ptssb != null)
				{
					ptssb.Append(""+phase+","+ val+"\n");
				}
			}
			else
			{
				p = nextPt;
				if (DEBUG_TONE_GEN)
				{
					Debug.Log("Moved to next point "+nextPt.DebugDescribe());
				}
				numSkipped++;
			}

			phase += step;

			yield return null;
		}	//	while (p!= null && p != RangeEnd && numDone < numSamples_)
		Debug.Log ("Finished Creation: "
			+ "\nNumSamples = " + numSamples_
			+ "\nNumDone = " + numDone
			+ "\nNumSkipped = " + numSkipped
			+ "\np = " + UnityExtensions.DebugDescribe (p)
		    + "\nn = " + UnityExtensions.DebugDescribe (nextPt));


		waveFormData_ = new WaveFormDataRegular("Graph", newSamples);

		if (waveFormData_ != null)
		{
			Debug.Log ("Created: " + waveFormData_.DebugDescribe ());
			ToneGeneratorPanel.Instance.SetWaveFormProvider ("Graph", waveFormData_);
			ToneGeneratorPanel.Instance.SetActive (true);
			messageLabel.color = Color.black;
			messageLabel.text = "Changed base tone";
		}
		else
		{
			messageLabel.color = Color.black;
			messageLabel.text = "!! Failed to create base tone";
		}
		if (ptssb != null)
		{
			Debug.Log("Points at...\n"+ptssb.ToString());
		}
		yield return null;
	}

	public void OnPlayButtonClicked()
	{
		if (waveFormData_ == null)
		{
			Debug.Log ("Not ready");
		}
		else
		{
			ToneGeneratorPanel.Instance.SetWaveFormProvider ("Graph", waveFormData_);
			ToneGeneratorPanel.Instance.SetActive(true);
		}
	}
}
