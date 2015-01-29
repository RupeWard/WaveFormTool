using UnityEngine;
using System.Collections;

public static class UnityExtensions
{
#region Transform
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

	public static Vector2 GetLocalXYPosition(this Transform t)
	{
		return new Vector2(t.localPosition.x, t.localPosition.y);
	}

#endregion

#region StringBuilder

	public static void Set(this Vector2 v, Vector2 other)
	{
		v.Set (other.x, other.y);
	}
#endregion

#region StringBuilder

	public static void DebugDescribe(this System.Text.StringBuilder sb, IDebugDescribable dd)
	{
		sb.Append ( (dd==null)?("NULL"):(dd.DebugDescribe()));
	}

	public static string DebugDescribe(this IDebugDescribable dd)
	{
		return ( (dd==null)?("NULL"):(dd.DebugDescribe()));
	}


#endregion




}
