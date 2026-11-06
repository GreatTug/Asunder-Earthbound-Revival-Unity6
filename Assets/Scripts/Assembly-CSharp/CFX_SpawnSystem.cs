using System.Collections.Generic;
using UnityEngine;

public class CFX_SpawnSystem : MonoBehaviour
{
	private static CFX_SpawnSystem instance;

	public GameObject[] objectsToPreload = new GameObject[0];

	public int[] objectsToPreloadTimes = new int[0];

	public bool hideObjectsInHierarchy;

	private bool allObjectsLoaded;

	private Dictionary<EntityId, List<GameObject>> instantiatedObjects = new Dictionary<EntityId, List<GameObject>>();

	private Dictionary<EntityId, int> poolCursors = new Dictionary<EntityId, int>();

	public static bool AllObjectsLoaded
	{
		get
		{
			return instance != null && instance.allObjectsLoaded;
		}
	}

	public static GameObject GetNextObject(GameObject sourceObj, bool activateObject = true)
	{
		if (sourceObj == null || instance == null)
		{
			return null;
		}
		EntityId instanceID = sourceObj.GetEntityId();
		if (!instance.poolCursors.ContainsKey(instanceID))
		{
			Debug.LogError("[CFX_SpawnSystem.GetNextPoolObject()] Object hasn't been preloaded: " + sourceObj.name + " (ID:" + instanceID + ")");
			return null;
		}
		int index = instance.poolCursors[instanceID];
		instance.poolCursors[instanceID] = index + 1;
		if (instance.poolCursors[instanceID] >= instance.instantiatedObjects[instanceID].Count)
		{
			instance.poolCursors[instanceID] = 0;
		}
		GameObject gameObject = instance.instantiatedObjects[instanceID][index];
		if (activateObject && gameObject != null)
		{
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	public static void PreloadObject(GameObject sourceObj, int poolSize = 1)
	{
		if (sourceObj == null || instance == null)
		{
			return;
		}
		instance.addObjectToPool(sourceObj, poolSize);
	}

	public static void UnloadObjects(GameObject sourceObj)
	{
		if (sourceObj == null || instance == null)
		{
			return;
		}
		instance.removeObjectsFromPool(sourceObj);
	}

	private void addObjectToPool(GameObject sourceObject, int number)
	{
		if (sourceObject == null)
		{
			return;
		}
		EntityId instanceID = sourceObject.GetEntityId();
		if (!instantiatedObjects.ContainsKey(instanceID))
		{
			instantiatedObjects.Add(instanceID, new List<GameObject>());
			poolCursors.Add(instanceID, 0);
		}
		for (int i = 0; i < number; i++)
		{
			GameObject gameObject = Object.Instantiate(sourceObject);
			if (gameObject != null)
			{
				gameObject.SetActive(false);
				WFX_AutoDestructShuriken[] componentsInChildren = gameObject.GetComponentsInChildren<WFX_AutoDestructShuriken>(true);
				foreach (WFX_AutoDestructShuriken wFX_AutoDestructShuriken in componentsInChildren)
				{
					if (wFX_AutoDestructShuriken != null)
					{
						wFX_AutoDestructShuriken.OnlyDeactivate = true;
					}
				}
				instantiatedObjects[instanceID].Add(gameObject);
				if (hideObjectsInHierarchy)
				{
					gameObject.hideFlags = HideFlags.HideInHierarchy;
				}
			}
		}
	}

	private void removeObjectsFromPool(GameObject sourceObject)
	{
		if (sourceObject == null)
		{
			return;
		}
		EntityId instanceID = sourceObject.GetEntityId();
		if (!instantiatedObjects.ContainsKey(instanceID))
		{
			Debug.LogWarning("[CFX_SpawnSystem.removeObjectsFromPool()] There aren't any preloaded object for: " + sourceObject.name + " (ID:" + instanceID + ")");
			return;
		}
		for (int num = instantiatedObjects[instanceID].Count - 1; num >= 0; num--)
		{
			GameObject obj = instantiatedObjects[instanceID][num];
			instantiatedObjects[instanceID].RemoveAt(num);
			if (obj != null)
			{
				Object.Destroy(obj);
			}
		}
		instantiatedObjects.Remove(instanceID);
		poolCursors.Remove(instanceID);
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogWarning("CFX_SpawnSystem: There should only be one instance of CFX_SpawnSystem per Scene!");
		}
		instance = this;
	}

	private void Start()
	{
		allObjectsLoaded = false;
		if (objectsToPreload != null)
		{
			for (int i = 0; i < objectsToPreload.Length; i++)
			{
				if (objectsToPreload[i] != null)
				{
					int count = (objectsToPreloadTimes != null && i < objectsToPreloadTimes.Length) ? objectsToPreloadTimes[i] : 1;
					PreloadObject(objectsToPreload[i], count);
				}
			}
		}
		allObjectsLoaded = true;
	}
}
