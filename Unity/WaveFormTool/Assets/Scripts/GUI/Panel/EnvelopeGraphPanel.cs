﻿using UnityEngine;
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

//	private BasicEnvelopeSettings envelopeSettings_;

	private BasicEnvelopeProvider envelopeProvider_ = null;

	private static readonly bool DEBUG_ENVELOPE_GRAPH = true;
	 
	public void Start()
	{
		saveTypeString_ = "Envelope";
		filenameExtension_ = "env";
		defaultFilename_ = "Envelope";
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

	private BasicEnvelopeSettings tmpEnvelopeSettings;

	protected override IEnumerator SetUpAxesCR ( )
	{
		if ( tmpEnvelopeSettings != null )
		{
			graphSettings.axisDefinitions = tmpEnvelopeSettings.MakeAxisDefinitions ( );
		}
		else
		{
			AxisDefinition[] axisDefinitions = new AxisDefinition[3];
			
			AxisDefinition xAxis = new AxisDefinition ();
			xAxis.axisName = "Time";
			xAxis.eDirection = EXYDirection.X;
			xAxis.value = 0f;
			axisDefinitions [0] = xAxis;
			
			AxisDefinition startAxis = new AxisDefinition ();
			startAxis.axisName = "start";
			startAxis.eDirection = EXYDirection.Y;
			startAxis.value = graphSettings.xRange.x;
			axisDefinitions [1] = startAxis;
			
			AxisDefinition endAxis = new AxisDefinition ();
			endAxis.axisName = "tailOutStart";
			endAxis.eDirection = EXYDirection.Y;
			endAxis.value = graphSettings.xRange.y;
			axisDefinitions [2] = endAxis;
			
			graphSettings.axisDefinitions = axisDefinitions;
		}
				
		DrawAxes ();
		yield return null;
	}

	private IEnumerator CreateGraphCR(IEnvelopeProvider efp, int numSamples, BasicEnvelopeSettings envelopeSettings, bool visibleOnly)
	{
		tmpEnvelopeSettings = envelopeSettings;
		isCreatingGraph_ = true;
		
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("CreateEnvelope( " + numSamples + " )\n"+envelopeSettings.DebugDescribe());
		
		yield return StartCoroutine(ClearPointsCR ());
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("Cleared points");

		graphSettings.xRange.x = graphSettings.xView.x = 0f;
		graphSettings.xRange.y = graphSettings.xView.y = envelopeSettings.TotalLength;
		graphSettings.yRange.x = graphSettings.yView.x = 0f;
		graphSettings.yRange.y = graphSettings.yView.y = envelopeSettings.leadInPeakValue;

		graphSettings.allowCrossingXAxis = false;
		graphSettings.loop = false;

		ResetView ();

		yield return StartCoroutine ( SetUpAxesCR ( ) );
		tmpEnvelopeSettings = null;

		float time = 0f;

		firstGraphSection_ = GraphSection.CreateGraphSection(this);

		GraphPoint newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
		newPoint.init(firstGraphSection_, 
		              time, 
		              0f,
		              GraphPointDef.EFunctionalState.Functional
		              );
		rangeStart_ = newPoint;
		OnPointSelected(rangeStart_);

		firstGraphSection_.FirstPoint = newPoint;

		rangeStart_.SetFixed ();
		newPoint.gameObject.name = "First";
		yield return null; // yield allows point to pick up on it immediately
		pointPanel_.gameObject.SetActive (false);

		GraphPoint previous = newPoint;
		newPoint = null;

		int numToPeak = Mathf.CeilToInt(((float)envelopeSettings.leadInPeakTime / (float)envelopeSettings.leadInLength) * (float)numSamples);
		float step = envelopeSettings.leadInPeakTime / numToPeak;
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("To peak = "+numToPeak+" points with step = "+step);
		yield return null;

		for (int i = 1; i <= numToPeak; i++)
		{
			time = i*step;
			newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
			newPoint.init(firstGraphSection_, 
			                 time, 
			                 efp.GetValueForTime(time, envelopeSettings),
			                 GraphPointDef.EFunctionalState.Functional
			                 );
			newPoint.gameObject.name = "1";
			newPoint.PreviousPointInternal = previous;
			previous.NextPointInternal = newPoint;
			previous = newPoint;
		}
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At peak = "+previous.DebugDescribe());
		yield return null;

		int numToEnd = numSamples - numToPeak;
		if (numToEnd > 0)
		{
			step = (envelopeSettings.leadInLength - envelopeSettings.leadInPeakTime) / numToEnd;
			if (DEBUG_ENVELOPE_GRAPH)
				Debug.Log ("To leadInEnd = "+numToEnd+" points with step = "+step);
			yield return null;

			for (int i = 1; i < numToEnd; i++)
			{
				time = envelopeSettings.leadInPeakTime + i*step;
				newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
				newPoint.init(firstGraphSection_, 
				              time, 
				              efp.GetValueForTime(time, envelopeSettings),
				              GraphPointDef.EFunctionalState.Functional
				              );
				newPoint.gameObject.name = "2";
				newPoint.PreviousPointInternal = previous;
				previous.NextPointInternal = newPoint;
				previous = newPoint;
			}
		}
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At leadInEnd = "+previous.DebugDescribe());
		yield return null;

		time = envelopeSettings.leadInLength;
		newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
		newPoint.init(firstGraphSection_, 
		              time, 
		              envelopeSettings.midValue,
		              GraphPointDef.EFunctionalState.Functional
		              );
		newPoint.gameObject.name = "3";
		newPoint.PreviousPointInternal = previous;
		previous.NextPointInternal = newPoint;
		previous = newPoint;
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At mid start = "+previous.DebugDescribe());
		yield return null;

		time = envelopeSettings.leadInLength + envelopeSettings.midLength;

		newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
		newPoint.init(firstGraphSection_, 
		              time, 
		              envelopeSettings.midValue,
		              GraphPointDef.EFunctionalState.Functional
		              );
		newPoint.gameObject.name = "4";
		newPoint.PreviousPointInternal = previous;
		previous.NextPointInternal = newPoint;
		previous = newPoint;
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At mid end = "+previous.DebugDescribe());
		yield return null;

		int numToFinish = Mathf.CeilToInt(((float)envelopeSettings.tailOutLength / envelopeSettings.leadInLength) * numSamples);
		if (numToFinish > 0)
		{
			step = envelopeSettings.tailOutLength / numToFinish;
			if (DEBUG_ENVELOPE_GRAPH)
				Debug.Log ("To finish = "+numToFinish+" points with step = "+step);
			yield return null;

			for (int i = 1; i <= numToFinish; i++)
			{
				time = envelopeSettings.leadInLength + envelopeSettings.midLength + i*step;
				newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
				newPoint.init(firstGraphSection_, 
				              time, 
				              efp.GetValueForTime(time, envelopeSettings),
				              GraphPointDef.EFunctionalState.Functional
				              );
				newPoint.gameObject.name = "5";
				newPoint.PreviousPointInternal = previous;
				previous.NextPointInternal = newPoint;
				previous = newPoint;
			}
		}
		if (DEBUG_ENVELOPE_GRAPH)
			Debug.Log ("At finish = "+previous.DebugDescribe());
		yield return null;

		if (previous.Point.x < envelopeSettings.TotalLength)
		{
			rangeEnd_ = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
			rangeEnd_.init(firstGraphSection_, 
			               envelopeSettings.TotalLength, 
			               0f,
			               GraphPointDef.EFunctionalState.Functional
			               );
			rangeEnd_.gameObject.name = "6";
			rangeEnd_.PreviousPointInternal = previous;
			previous.NextPointInternal = rangeEnd_;
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
				p = p.NextPointAbsolute;

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
