using System;
using System.Reflection;
using UnityEngine;

public class USSequencerUtility
{
	public static bool IsObserverTimeline(Transform transform)
	{
		return transform.GetComponent<USTimelineObserver>() != null;
	}

	public static bool IsTimelineContainer(Transform transform)
	{
		return transform.GetComponent<USTimelineContainer>() != null;
	}

	public static bool IsTimeline(Transform transform)
	{
		return transform.GetComponent<USTimelineBase>() != null;
	}

	public static bool IsEventTimeline(Transform transform)
	{
		return transform.GetComponent<USTimelineEvent>() != null;
	}

	public static bool IsPropertyTimeline(Transform transform)
	{
		return transform.GetComponent<USTimelineProperty>() != null;
	}

	public static bool IsEvent(Transform transform)
	{
		return transform.GetComponent<USEventBase>() != null;
	}

	public static bool HasPropertyTimeline(Transform transform)
	{
		if (!IsTimelineContainer(transform))
		{
			return false;
		}
		Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>();
		foreach (Transform transform2 in componentsInChildren)
		{
			if (IsPropertyTimeline(transform2))
			{
				return true;
			}
		}
		return false;
	}

	public static bool HasObserverTimeline(Transform transform)
	{
		if (!IsTimelineContainer(transform))
		{
			return IsObserverTimeline(transform);
		}
		Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>();
		foreach (Transform transform2 in componentsInChildren)
		{
			if (IsObserverTimeline(transform2))
			{
				return true;
			}
		}
		return false;
	}

	public static Type GetType(string aType)
	{
#pragma warning disable UAC0005
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
#pragma warning restore UAC0005
		foreach (Assembly assembly in assemblies)
		{
			Type[] types;
			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				types = ex.Types;
			}
			if (types == null)
			{
				continue;
			}
			foreach (Type type in types)
			{
				if (type != null && type.Name == aType)
				{
					return type;
				}
			}
		}
		return null;
	}
}
