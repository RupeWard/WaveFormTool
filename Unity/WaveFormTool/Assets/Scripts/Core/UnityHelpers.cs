using UnityEngine;

public class UnityHelpers
{
    public static void Destroy(UnityEngine.Object theObject)
    {
#if UNITY_EDITOR
        Object.DestroyImmediate(theObject);
#else
        Object.Destroy(theObject);
#endif
    }


	public static double DLerp(double from, double to, double fraction)
	{
		if ( fraction < 0 || fraction > 1 )
		{
			Debug.LogError ( "Out of range at "+fraction);
		}
		return from + ( to - from ) * fraction;
	}
	
	public static double DLerpFree(double from, double to, double fraction)
	{
		return from + ( to - from ) * fraction;
	}
	
	public static float LerpFree(float from, float to, float fraction)
	{
		return from + ( to - from ) * fraction;
	}

    /*
    /// <summary>
    /// Gets a gui style logging a warning if the style isn't present in the skin
    /// </summary>
    /// <param name="skin"></param>
    /// <param name="style"></param>
    /// <returns></returns>
    public static GUIStyle GetStyle(ref GUISkin skin, string styleName)
    {
        GUIStyle foundStyle = null;
        if (skin != null)
        {
            foundStyle = skin.FindStyle(styleName);
            DebugHelper.Warning(foundStyle != null, "Unable to find style with name " + styleName + " in skin " + skin.name);
        }
        else
        {
            Debug.LogError("Unable to find style as skin is null");
        }

        return foundStyle;
    }
    */
    /// <summary>
    /// Set the value of a vector2
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetVector2(ref Vector2 vector, float x, float y)
    {
        vector.x = x;
        vector.y = y;
    }

    /// <summary>
    /// Set the value of a vector2
    /// </summary>
    public static void SetVector3(ref Vector3 vector, float x, float y, float z)
    {
        vector.x = x;
        vector.y = y;
        vector.z = z;
    }

    public static void SetRect(ref Rect rect, float x, float y, float width, float height)
    {
        rect.x = x;
        rect.y = y;
        rect.width = width;
        rect.height = height;
    }

    public static void SetRectDimensionsFromNormalTex(ref GUIStyle style, ref Rect rect)
    {
        SetRectDimensionsFromTexture2D(style.normal.background, ref rect);
    }

    public static void SetRectDimensionsFromTexture2D(Texture2D tex, ref Rect rect)
    {
        rect.width = tex.width;
        rect.height = tex.height;
    }
}
