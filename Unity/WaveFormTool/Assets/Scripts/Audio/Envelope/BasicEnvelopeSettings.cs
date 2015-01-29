using UnityEngine;
using System.Collections;

public class BasicEnvelopeSettings : IDebugDescribable
{
	public float leadInLength = 0.3f;
	public float leadInPeakValue = 1.0f;
	public float leadInPeakTime = 0.15f;
	public float tailOutLength = 0.3f;
	public float midLength = 0.4f;

	public float midValue = 0.7f;

	public float TotalLength 
	{
		get {return leadInLength + midLength + tailOutLength; } 
	}

	public bool isInMid(float time)
	{
		return (time >= leadInLength && time <= leadInLength + midLength);
	}

	public bool isInTail(float time)
	{
		return (time > leadInLength + midLength);
	}

	public bool isInLeadIn(float time)
	{
		return (time < leadInLength);
	}

	public bool isInLeadInUpToPeak(float time)
	{
		return (time <= leadInPeakTime);
	}

	public AxisDefinition[] MakeAxisDefinitions()
	{
		AxisDefinition[] axisDefinitions = new AxisDefinition[3];
		
		AxisDefinition xAxis = new AxisDefinition ();
		xAxis.axisName = "Time";
		xAxis.eDirection = EXYDirection.X;
		xAxis.value = 0f;
		axisDefinitions [0] = xAxis;
		
		AxisDefinition leadInAxis = new AxisDefinition ();
		leadInAxis.axisName = "leadInEnd";
		leadInAxis.eDirection = EXYDirection.Y;
		leadInAxis.value = leadInLength;
		axisDefinitions [1] = leadInAxis;
		
		AxisDefinition tailOutAxis = new AxisDefinition ();
		tailOutAxis.axisName = "tailOutStart";
		tailOutAxis.eDirection = EXYDirection.Y;
		tailOutAxis.value = leadInLength + midLength;
		axisDefinitions [2] = tailOutAxis;

		return axisDefinitions;
	}

	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append("leadInLength "+leadInLength+"\n");
		sb.Append("leadInPeakValue "+leadInPeakValue+"\n");
		sb.Append("leadInPeakTime "+leadInPeakTime+"\n");
		sb.Append("tailOutLength "+tailOutLength+"\n");
		sb.Append("midLength "+midLength+"\n");
		sb.Append("midValue "+midValue+"\n");
	}

	public string DebugDescribe()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder ();
		DebugDescribe (sb);
		return sb.ToString ();
	}

}
