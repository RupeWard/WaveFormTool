using UnityEngine;
using System.Collections;

public static class UnityExtensions
{
	public static void SetLocalSize(this Transform t, float x)
	{
		t.localScale = new Vector3 (x, x, t.localScale.z);
	}
}
