using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AxisDefinition : MonoBehaviour
{
	public string axisName = "Axis";
	public EXYDirection eDirection = EXYDirection.X;
	public float value = 0f;	
	public float tickLabelSize = 12f;
	public float tickBase = 0f;
	public float tickSpacing = 0.2f;
	public float axisLineWidth = 2f;
}

