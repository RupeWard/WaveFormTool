using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphCreator : IDebugDescribable
{
	private List< GraphSectionDefinition > graphSections_ = new List< GraphSectionDefinition > ( );

	public void AddGraphSection( GraphSectionDefinition d)
	{
		graphSections_.Add(d);
	}

	#region IDebugDescribable
	public void DebugDescribe ( System.Text.StringBuilder sb )
	{
		sb.Append("[GraphCreator: ").Append (graphSections_.Count);
		foreach (GraphSectionDefinition d in graphSections_)
		{
			sb.Append (" ").Append (d.DebugDescribe());
		}
	}
	#endregion

}
