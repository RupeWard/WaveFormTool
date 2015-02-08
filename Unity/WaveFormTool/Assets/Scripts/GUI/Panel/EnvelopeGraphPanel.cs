using UnityEngine;
using System.Collections;

public class EnvelopeGraphPanel : GraphPanel, IEnvelopeProvider
{
	/* TODO
	 * 
	 * subgraph concept 
	 * what needs overriding?
	 * 
	 */


//	private static readonly bool DEBUG_TONE_GEN = false;

	public int numSamples_ = 100;

	private BasicEnvelopeSettings envelopeSettings_;

	private static readonly bool DEBUG_ENVELOPE_GRAPH = true;
	 
	public void Start()
	{
		ToneGeneratorPanel.Instance.envelopeProvider_ = this;
	}

	public override void ResetView ()
	{
		graphSettings.ResetViewCentres();
		viewMinInput.text = graphSettings.xView.x.ToString();
		viewMaxInput.text = graphSettings.xView.y.ToString();
		AdjustAxes ();
		StartCoroutine (AdjustPointPositionsCR ());
	}

	public void CreateGraph(IEnvelopeProvider efp, int numSamples, BasicEnvelopeSettings envelopeSettings, bool visibleOnly)
	{
		StartCoroutine (CreateGraphCR(efp, numSamples, envelopeSettings, visibleOnly));
	}

	private IEnumerator CreateGraphCR(IEnvelopeProvider efp, int numSamples, BasicEnvelopeSettings envelopeSettings, bool visibleOnly)
	{
		envelopeSettings_ = envelopeSettings;
		isCreatingGraph_ = true;
		
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("CreateEnvelope( " + numSamples + " )\n"+envelopeSettings_.DebugDescribe());
		
		yield return StartCoroutine(ClearPointsCR ());
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("Cleared points");

		graphSettings.xRange.x = graphSettings.xView.x = 0f;
		graphSettings.xRange.y = graphSettings.xView.y = envelopeSettings_.TotalLength;
		graphSettings.yRange.x = graphSettings.yView.x = 0f;
		graphSettings.yRange.y = graphSettings.yView.y = envelopeSettings_.leadInPeakValue;

		graphSettings.allowCrossingXAxis = false;
		graphSettings.loop = false;
		
		graphSettings.axisDefinitions = envelopeSettings_.MakeAxisDefinitions();

		ResetView ();

		DrawAxes ();
		yield return null;

		float time = 0f;

		firstPoint_ = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
		firstPoint_.transform.parent = pointsContainer;
		firstPoint_.init(this, 
		              time, 
		              0f,
		              true
		              );
		rangeStart_ = firstPoint_;
		OnPointSelected(rangeStart_);
		pointPanel_.actionMenu.OnOptionSelected(GraphPointActionMenu.fixPointOption);
		firstPoint_.gameObject.name = "First";
		yield return null; // yield allows point to pick up on it immediately
		pointPanel_.gameObject.SetActive (false);

		GraphPoint previous = firstPoint_;
		GraphPoint newPoint = null;

		int numToPeak = Mathf.CeilToInt(((float)envelopeSettings_.leadInPeakTime / (float)envelopeSettings_.leadInLength) * (float)numSamples);
		float step = envelopeSettings_.leadInPeakTime / numToPeak;
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("To peak = "+numToPeak+" points with step = "+step);
		yield return null;

		for (int i = 1; i <= numToPeak; i++)
		{
			time = i*step;
			newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
			newPoint.transform.parent = pointsContainer;
			newPoint.init(this, 
			                 time, 
			                 efp.GetValueForTime(time, envelopeSettings_),
			                 true
			                 );
			newPoint.gameObject.name = "1";
			newPoint.PreviousPoint = previous;
			previous.NextPoint = newPoint;
			previous = newPoint;
		}
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At peak = "+previous.DebugDescribe());
		yield return null;

		int numToEnd = numSamples - numToPeak;
		if (numToEnd > 0)
		{
			step = (envelopeSettings_.leadInLength - envelopeSettings_.leadInPeakTime) / numToEnd;
			if (DEBUG_ENVELOPE_GRAPH)
				Debug.Log ("To leadInEnd = "+numToEnd+" points with step = "+step);
			yield return null;

			for (int i = 1; i < numToEnd; i++)
			{
				time = envelopeSettings_.leadInPeakTime + i*step;
				newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
				newPoint.transform.parent = pointsContainer;
				newPoint.init(this, 
				              time, 
				              efp.GetValueForTime(time, envelopeSettings_),
				              true
				              );
				newPoint.gameObject.name = "2";
				newPoint.PreviousPoint = previous;
				previous.NextPoint = newPoint;
				previous = newPoint;
			}
		}
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At leadInEnd = "+previous.DebugDescribe());
		yield return null;

		time = envelopeSettings_.leadInLength;
		newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
		newPoint.transform.parent = pointsContainer;
		newPoint.init(this, 
		              time, 
		              envelopeSettings_.midValue,
		              true
		              );
		newPoint.gameObject.name = "3";
		newPoint.PreviousPoint = previous;
		previous.NextPoint = newPoint;
		previous = newPoint;
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At mid start = "+previous.DebugDescribe());
		yield return null;

		time = envelopeSettings_.leadInLength + envelopeSettings_.midLength;

		newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
		newPoint.transform.parent = pointsContainer;
		newPoint.init(this, 
		              time, 
		              envelopeSettings_.midValue,
		              true
		              );
		newPoint.gameObject.name = "4";
		newPoint.PreviousPoint = previous;
		previous.NextPoint = newPoint;
		previous = newPoint;
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At mid end = "+previous.DebugDescribe());
		yield return null;

		int numToFinish = Mathf.CeilToInt(((float)envelopeSettings_.tailOutLength / envelopeSettings_.leadInLength) * numSamples);
		if (numToFinish > 0)
		{
			step = envelopeSettings_.tailOutLength / numToFinish;
			if (DEBUG_ENVELOPE_GRAPH)
				Debug.Log ("To finish = "+numToFinish+" points with step = "+step);
			yield return null;

			for (int i = 1; i <= numToFinish; i++)
			{
				time = envelopeSettings_.leadInLength + envelopeSettings_.midLength + i*step;
				newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
				newPoint.transform.parent = pointsContainer;
				newPoint.init(this, 
				              time, 
				              efp.GetValueForTime(time, envelopeSettings_),
				              true
				              );
				newPoint.gameObject.name = "5";
				newPoint.PreviousPoint = previous;
				previous.NextPoint = newPoint;
				previous = newPoint;
			}
		}
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At finish = "+previous.DebugDescribe());
		yield return null;

		rangeEnd_ = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
		rangeEnd_.transform.parent = pointsContainer;
		rangeEnd_.init(this, 
		              envelopeSettings_.TotalLength, 
		              0f,
		              true
		              );
		rangeEnd_.gameObject.name = "6";
		rangeEnd_.PreviousPoint = previous;
		previous.NextPoint = rangeEnd_;

		OnPointSelected(rangeEnd_);
		pointPanel_.actionMenu.OnOptionSelected(GraphPointActionMenu.fixPointOption);
		yield return null; // yield allows point to pick up on it immediately
		pointPanel_.gameObject.SetActive (false);

		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("Created points");
		yield return null;
		
		isCreatingGraph_ = false;
		HandleDataChange ();
		
		yield return null;
	}

	public void Update()
	{
		/*
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
		*/
	}

	private bool isDirty_ = false;
	public override void HandleDataChange ()
	{
		isDirty_ = true;
	}

//	private bool isCreating_ = false;

	protected override void postInit()
	{
		/*
		Vector2 pos = viewMinInput.transform.GetLocalXYPosition ();
		pos.y += 60f;
		pos.x += 40f;
		playButton.SetLocalXYPosition ( pos );
		*/
	}

	private IEnumerator CreateDataCR()
	{
		Debug.LogError ("Not implmeneted");
		yield return null;
		/*
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
			//ToneGeneratorPanel.Instance.SetActive (true);
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
	*/
	}

#region IEnvelopeProvider
	GraphPoint currentProvidingPoint = null;

	public float GetValueForTime(float time, BasicEnvelopeSettings settings)
	{
		float f = 0f;
		currentProvidingPoint = FindPointBeforeTime ( currentProvidingPoint, time );
		if (currentProvidingPoint != null)
		{
			GraphPoint nextPoint = currentProvidingPoint.NextPoint;
			if (nextPoint == null)
			{
				f = currentProvidingPoint.Point.y;
			}
			else
			{
				float timeFraction = (time - currentProvidingPoint.Point.x)/(nextPoint.Point.x - currentProvidingPoint.Point.x);
				f = Mathf.Lerp ( currentProvidingPoint.Point.y, nextPoint.Point.y, timeFraction);
			}
		}
		return f;
	}

	private GraphPoint FindPointBeforeTime(GraphPoint p, float t)
	{
		GraphPoint result = null;
		if ( firstPoint_ == null )
		{
			Debug.LogWarning ( "No first point!" );
		}
		else
		{
			if ( p == null )
			{
				Debug.LogWarning("null passed, startig at first");
				p = firstPoint_;
			}
			if ( p.Point.x > t )
			{
				Debug.LogWarning ("Current point is too late, going back");
				while (p != null && p.Point.x > t)
				{
					p = p.PreviousPoint;
				}
				if (p == null)
				{
					Debug.LogWarning ("Went back to the start and didn't find a point before time" +t);
				}
				else
				{
					result = p;
				}
			}
			else
			{
				while (p.NextPoint != null && p.NextPoint.Point.x < t)
				{
					p = p.NextPoint;
				}
				if (p.NextPoint == null && p.Point.x < t)
				{
					Debug.LogWarning ("Got to the end and still earlier than time "+t);
				}
				else
				{
					result = p;
				}
			}

		}
		return result;
	}

	public string EnvelopeName() 
	{
		return "EnvelopeGraph";
	}	

	public bool IsReady()
	{
		return ( NumGraphPoints ( ) > 1 );
	}

	
	public float EnvelopeLength(BasicEnvelopeSettings unused)
	{
		return envelopeSettings_.TotalLength;
	}




#endregion
}
