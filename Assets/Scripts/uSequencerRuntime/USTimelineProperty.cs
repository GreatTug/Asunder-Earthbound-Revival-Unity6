using System.Collections.Generic;
using UnityEngine;

public class USTimelineProperty : USTimelineBase
{
	[SerializeField]
	private List<USPropertyInfo> propertyList = new List<USPropertyInfo>();

	public List<USPropertyInfo> Properties
	{
		get
		{
			return propertyList;
		}
	}

	public override void Reset()
	{
		foreach (USPropertyInfo property in propertyList)
		{
			if (property != null)
			{
				property.SetValue(0f);
			}
		}
	}

	public bool HasPropertiesForComponent(Component component)
	{
		foreach (USPropertyInfo property in propertyList)
		{
			if (component == property.Component)
			{
				return true;
			}
		}
		return false;
	}

	public USPropertyInfo GetProperty(string propertyName, Component component)
	{
		foreach (USPropertyInfo property in propertyList)
		{
			if (property == null || !(property.Component == component) || (!(property.propertyName == propertyName) && !(property.fieldName == propertyName)))
			{
				continue;
			}
			return property;
		}
		return null;
	}

	public bool ContainsProperty(string propertyName, Component component)
	{
		foreach (USPropertyInfo property in propertyList)
		{
			if (property == null || !(property.Component == component) || (!(property.propertyName == propertyName) && !(property.fieldName == propertyName)))
			{
				continue;
			}
			return true;
		}
		return false;
	}

	public void AddProperty(USPropertyInfo propertyInfo)
	{
		if ((propertyInfo.propertyName == null || !ContainsProperty(propertyInfo.propertyName, propertyInfo.Component)) && (propertyInfo.fieldName == null || !ContainsProperty(propertyInfo.fieldName, propertyInfo.Component)))
		{
			propertyList.Add(propertyInfo);
		}
	}

	public void RemoveProperty(USPropertyInfo propertyInfo)
	{
		propertyList.Remove(propertyInfo);
	}

	public override void SkipTimelineTo(float time)
	{
		Process(time, 1f);
	}

	public override void Process(float sequencerTime, float playbackRate)
	{
		if (!base.AffectedObject)
		{
			return;
		}
		foreach (USPropertyInfo property in propertyList)
		{
			if (property != null)
			{
				if (!property.Component)
				{
					property.Component = base.AffectedObject.GetComponent(property.ComponentType);
				}
				if ((bool)property.Component && property.Component.transform != base.AffectedObject)
				{
					property.Component = base.AffectedObject.GetComponent(property.ComponentType);
				}
				property.SetValue(sequencerTime);
			}
		}
	}
}
