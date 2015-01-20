using UnityEngine;
using System.Collections;

public class GraphPointMoverSolo : GraphPointMoverBase // FIXME refact to soloY
{
	public override void MoveGraphPoint(GraphPoint pt, Vector2 newValues)
	{
		pt.SetXY(newValues);
	}

	public override void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append ("[SoloY]");
	}

}
