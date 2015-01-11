﻿using UnityEngine;
using System.Collections;

public static class UnityExtensions
{
	public static void SetLocalXYPosition(this Transform t, float x, float y)
	{
		t.localPosition = new Vector3 (x, y, t.localPosition.z);
	}

	public static void SetLocalXYPosition(this Transform t, Vector2 v)
	{
		t.localPosition = new Vector3 (v.x, v.y, t.localPosition.z);
	}
	
	public static void SetLocalXYSize(this Transform t, float x, float y)
	{
		t.localScale = new Vector3 (x, y, t.localScale.z);
	}

	public static void SetLocalXYSize(this Transform t, Vector2 v)
	{
		t.localScale = new Vector3 (v.x, v.y, t.localScale.z);
	}

	public static void SetLocalXYSize(this Transform t, float x)
	{
		t.SetLocalXYSize(x, x);
	}
	
	public static Vector2 GetLocalXYSize(this Transform t)
	{
		return new Vector2(t.localScale.x, t.localScale.y);
	}

}
