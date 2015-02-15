using UnityEngine;
using System.Collections;

public class EnvelopeGraphPanel : GraphPanel
{
	/* TODO
	 * 
	 * subgraph concept 
	 * what needs overriding?
	 * 
	 */


	private static readonly bool DEBUG_ENVELOPE_GEN = true;

	public int numSamples_ = 100;

	private BasicEnvelopeSettings envelopeSettings_;

	private BasicEnvelopeProvider envelopeProvider_ = null;

	private static readonly bool DEBUG_ENVELOPE_GRAPH = true;
	 
	public void Start()
	{
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
		              GraphPointDef.EFunctionalState.Functional
		              );
		rangeStart_ = firstPoint_;
		OnPointSelected(rangeStart_);

		rangeStart_.SetFixed ();
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
			                 GraphPointDef.EFunctionalState.Functional
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
				              GraphPointDef.EFunctionalState.Functional
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
		              GraphPointDef.EFunctionalState.Functional
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
		              GraphPointDef.EFunctionalState.Functional
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
				              GraphPointDef.EFunctionalState.Functional
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

		if (previous.Point.x < envelopeSettings_.TotalLength)
		{
			rangeEnd_ = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
			rangeEnd_.transform.parent = pointsContainer;
			rangeEnd_.init(this, 
			               envelopeSettings_.TotalLength, 
			               0f,
			               GraphPointDef.EFunctionalState.Functional
			               );
			rangeEnd_.gameObject.name = "6";
			rangeEnd_.PreviousPoint = previous;
			previous.NextPoint = rangeEnd_;
		}
		else
		{
			rangeEnd_ = previous;
		}

		OnPointSelected(rangeEnd_);
		rangeEnd_.SetFixed ();
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
		if (isDirty_)
		{
			if (isCreating_)
			{
				if (DEBUG_ENVELOPE_GEN)
				{
					Debug.Log("EGP: restarting");
				}
				isCreating_ = false;
				messageLabel.color = Color.red;
				messageLabel.text = "Restarted envelope creation";
				StopCoroutine ("CreateDataCR");
			}
			else
			{
				if (DEBUG_ENVELOPE_GEN)
				{
					Debug.Log("EGP: starting");
				}
				messageLabel.color = Color.red;
				messageLabel.text = "Started envelope creation";
			}
			isDirty_ = false;
			StartCoroutine("CreateDataCR");
		}
	}

	private bool isDirty_ = false;
	public override void HandleDataChange ()
	{
		isDirty_ = true;
	}

	private bool isCreating_ = false;

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
		if ( NumGraphPoints() < 3 )
		{
			Debug.LogWarning ( "Not enough points!" );
		}
		else
		{
			BasicEnvelopeProvider provider = new BasicEnvelopeProvider ( );

			System.Text.StringBuilder sb = null;
			int numGraphPoints = NumGraphPoints ();

			if (DEBUG_ENVELOPE_GEN)
			{
				sb = new System.Text.StringBuilder();
				Debug.Log("EGP: START. num = "+numSamples_+", with "+numGraphPoints+" graph points");
			}

			GraphPoint p = RangeStart;

			while ( p!= null)
			{
				provider.AddPoint(p.Point);
				sb.Append("\nEnvPt "+p.Point);
				p = p.NextPoint;

			}	//	while (p!= null && p != RangeEnd && numDone < numSamples_)
			yield return null;

			Debug.Log ("Finished Creation: ");
			messageLabel.text = "Created envelope";
			envelopeProvider_ = provider;
			ToneGeneratorPanel.Instance.envelopeProvider_ = envelopeProvider_;
			if (sb != null)
			{
				Debug.Log(sb.ToString());
			}

		}
		yield return null;
	}

}
