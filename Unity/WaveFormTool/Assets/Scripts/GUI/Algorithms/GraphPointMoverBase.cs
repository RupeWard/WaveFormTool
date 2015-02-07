using UnityEngine;
using System.Collections;

public abstract class GraphPointMoverBase : MonoBehaviour , IDebugDescribable
{
	public GraphPanel graphPanel;
	public string moverName;

	public abstract void MoveGraphPoint(GraphPoint pt, Vector2 newValues);
	public void MoveGraphPointX(GraphPoint pt, float newX)
	{
		MoveGraphPoint (pt, new Vector2 (newX, pt.Point.y));
	}
	public void MoveGraphPointY(GraphPoint pt, float newY)
	{
		MoveGraphPoint (pt, new Vector2 (pt.Point.x, newY));
	}

#region IDebugDescribable
	public abstract void DebugDescribe(System.Text.StringBuilder sb);
#endregion

}
