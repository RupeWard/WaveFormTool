using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphCreator : IDebugDescribable
{
	private string creatorName_ = "GraphCreator";
	public string CreatorName 
	{
		get { return creatorName_; }
	}

	private List< GraphSectionDefinition > graphSections_ = new List< GraphSectionDefinition > ( );
	public List < GraphSectionDefinition > GraphSections
	{
		get { return graphSections_; }
	}

	public GraphCreator(string n)
	{
		creatorName_ = n;
	}

	public void AddGraphSection( GraphSectionDefinition d)
	{
		graphSections_.Add(d);
	}

	public float Length
	{
		get 
		{
			float result = 0f;
			foreach (GraphSectionDefinition d in graphSections_)
			{
				result += d.EnvelopeLength();
			}
			return result;
		}
	}

	public float MaxValue
	{
		get 
		{
			float result = float.MinValue;
			foreach (GraphSectionDefinition d in graphSections_)
			{
				float f = d.maxValue();
				if (f > result)
				{
					result = f;
				}
			}
			return result;
		}
	}

	public float MinValue
	{
		get 
		{
			float result = float.MaxValue;
			foreach (GraphSectionDefinition d in graphSections_)
			{
				float f = d.minValue();
				if (f < result)
				{
					result = f;
				}
			}
			return result;
		}
	}
	

	#region IDebugDescribable
	public void DebugDescribe ( System.Text.StringBuilder sb )
	{
		sb.Append("[GraphCreator: ").Append (creatorName_);
		sb.Append(" n=").Append (graphSections_.Count);
		foreach (GraphSectionDefinition d in graphSections_)
		{
			sb.Append (" ").Append (d.DebugDescribe());
		}
	}
	#endregion

}
