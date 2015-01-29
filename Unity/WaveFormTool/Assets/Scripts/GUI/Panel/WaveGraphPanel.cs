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

	public void CreateGraph(IWaveFormProvider wfp, int numSamples, bool visibleOnly)
	{
		ResetView ();
		StartCoroutine (CreateGraphCR(wfp, numSamples, visibleOnly));
	}

	public IEnumerator CreateGraphCR(IWaveFormProvider wfp, int numSamples, bool visibleOnly)
	{
		isCreatingGraph_ = true;
		
		if (DEBUG_GRAPH)
			Debug.Log ("CreateGraph( " + numSamples + " )");
		
		yield return StartCoroutine(ClearPointsCR ());
		if (DEBUG_GRAPH)
			Debug.Log ("Cleared points");

		AxisDefinition[] axisDefinitions = new AxisDefinition[3];

		AxisDefinition xAxis = new AxisDefinition ();
		xAxis.axisName = "X";
		xAxis.eDirection = EXYDirection.X;
		xAxis.value = 0f;
		axisDefinitions [0] = xAxis;

		AxisDefinition phaseStartAxis = new AxisDefinition ();
		phaseStartAxis.axisName = "Start";
		phaseStartAxis.eDirection = EXYDirection.Y;
		phaseStartAxis.value = 0f;
		axisDefinitions [1] = phaseStartAxis;

		AxisDefinition phaseEndAxis = new AxisDefinition ();
		phaseEndAxis.axisName = "Start";
		phaseEndAxis.eDirection = EXYDirection.Y;
		phaseEndAxis.value = 1f;
		axisDefinitions [2] = phaseEndAxis;

		settings.axisDefinitions = axisDefinitions;

		DrawAxes ();
		yield return null;
		
		float step = settings.XRangeLength / numSamples;
		
		
		float currentX = settings.xRange.x;
		if (settings.loop)
		{
			while (currentX > settings.xView.x)
			{
				currentX -= step;
			}
		}
		
		float finalX = settings.xRange.y;
		if (settings.loop)
		{
			while (finalX < settings.xView.y)
			{
				finalX += step;
			}
		}
		
		GraphPoint previous = null;
		
		float rangeEndTolerance = step / 10f;
		
		while (currentX <= finalX)
		{
			if (!visibleOnly || (settings.IsXInView(currentX) ))
			{
				GraphPoint newPoint = (GameObject.Instantiate ( Resources.Load<GameObject>( "GUI/Prefabs/GraphPoint"))as GameObject).GetComponent< GraphPoint>();
				newPoint.transform.parent = pointsContainer;
				newPoint.init(this, 
				              currentX, 
				              wfp.GetValueForPhase(currentX, WaveFormDataInterpolatorLinear.Instance),
				              (currentX >= 0f && currentX <= 1f)
				              );
				bool bIsRangeStart = false;
				if (Mathf.Abs( currentX - settings.xRange.x ) < rangeEndTolerance)
				{
					bIsRangeStart = true;
					rangeStart_ = newPoint;
				}
				bool bIsRangeEnd = false;
				if (!bIsRangeStart && (Mathf.Abs( currentX - settings.xRange.y ) < rangeEndTolerance))
				{
					bIsRangeEnd = true;
					rangeEnd_ = newPoint;
				}
				if (bIsRangeStart || bIsRangeEnd) // FIXME specific to wave loop
				{
					OnPointSelected(newPoint);
					pointPanel_.actionMenu.OnOptionSelected(GraphPointActionMenu.fixPointOption);
					
					yield return null; // yield allows point to pick up on it immediately
					//					newPoint.IsFixed = true;
				}
				currentX += step;
				
				//				Debug.Log ("Created Point : "+newPoint.DebugDescribe());
				
				if (firstPoint_ == null)
				{
					firstPoint_ = newPoint;
				}
				else
				{
					newPoint.PreviousPoint = previous;
					previous.NextPoint = newPoint;
				}
				previous = newPoint;
				yield return null;
			}
			OnPointSelected(null);
		}
		if (rangeEnd_ == null || rangeStart_ == null)
		{
			Debug.LogError ("Range ends not found, looking for closest");
			
			float minAbsXdist = float.MaxValue;
			GraphPoint pt = firstPoint_;
			while (pt != null)
			{
				float absDist = Mathf.Abs(pt.Point.x - settings.xRange.x);
				if (absDist < minAbsXdist)
				{
					minAbsXdist = absDist;
					rangeStart_ = pt;
				}
				pt = pt.NextPoint;
			}
			minAbsXdist = float.MaxValue;
			pt = firstPoint_;
			while (pt != null)
			{
				float absDist = Mathf.Abs(pt.Point.x - settings.xRange.y);
				if (absDist < minAbsXdist)
				{
					minAbsXdist = absDist;
					rangeEnd_ = pt;
				}
				pt = pt.NextPoint;
			}
			
		}
		GraphPoint earlyPoint = rangeStart_.PreviousPoint;
		GraphPoint followedPoint = rangeEnd_.PreviousPoint;
		
		int earlyPoints = 0;
		while (earlyPoint != null && followedPoint != null)
		{
			earlyPoints++;
			followedPoint.SetFollower(earlyPoint);
			earlyPoint = earlyPoint.PreviousPoint;
			followedPoint = followedPoint.PreviousPoint;
		}
		if (earlyPoints > 0 && !settings.loop)
		{
			Debug.LogError("Found "+earlyPoints+" early points when not looping");
		}
		
		GraphPoint latePoint = rangeEnd_.NextPoint;
		followedPoint = rangeStart_.NextPoint;
		
		int latePoints = 0;
		while (latePoint != null && followedPoint != null)
		{
			latePoints++;
			followedPoint.SetFollower(latePoint);
			latePoint = latePoint.NextPoint;
			followedPoint = followedPoint.NextPoint;
		}
		if (latePoints > 0 && !settings.loop)
		{
			Debug.LogError("Found "+latePoints+" late points when not looping");
		}
		
		if (DEBUG_GRAPH)
			Debug.Log ("Created points");
		
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
