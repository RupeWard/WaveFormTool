using UnityEngine;
using System.Collections;
using UnityEditor;

public static class NGUIExtensions
{
	public static void LookLikeControls(float f)
	{
		EditorGUIUtility.labelWidth = f;
		EditorGUIUtility.fieldWidth = f;
	}
	/*
	static public string DebugDescribe<T>( this T t) where T : IDebugDescribable
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder ();
		t.DebugDescribe (sb);
		return sb.ToString ();
	}*/
}
