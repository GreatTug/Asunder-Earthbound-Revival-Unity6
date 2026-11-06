using UnityEngine;

public class OVRPresetManager
{
	private static string PresetName = string.Empty;

	public bool SetCurrentPreset(string presetName)
	{
		PresetName = presetName;
		return true;
	}

	public bool SetPropertyInt(string name, ref int v)
	{
		string key = PresetName + name;
		PlayerPrefs.SetInt(key, v);
		return true;
	}

	public bool GetPropertyInt(string name, ref int v)
	{
		string key = PresetName + name;
		if (!PlayerPrefs.HasKey(key))
		{
			return false;
		}
		v = PlayerPrefs.GetInt(key);
		return true;
	}

	public bool SetPropertyFloat(string name, ref float v)
	{
		string key = PresetName + name;
		PlayerPrefs.SetFloat(key, v);
		return true;
	}

	public bool GetPropertyFloat(string name, ref float v)
	{
		string key = PresetName + name;
		if (!PlayerPrefs.HasKey(key))
		{
			return false;
		}
		v = PlayerPrefs.GetFloat(key);
		return true;
	}

	public bool SetPropertyString(string name, ref string v)
	{
		string key = PresetName + name;
		PlayerPrefs.SetString(key, v);
		return true;
	}

	public bool GetPropertyString(string name, ref string v)
	{
		string key = PresetName + name;
		if (!PlayerPrefs.HasKey(key))
		{
			return false;
		}
		v = PlayerPrefs.GetString(key);
		return true;
	}

	public bool DeleteProperty(string name)
	{
		string key = PresetName + name;
		PlayerPrefs.DeleteKey(key);
		return true;
	}

	public bool SaveAll()
	{
		PlayerPrefs.Save();
		return true;
	}

	public bool DeleteAll()
	{
		PlayerPrefs.DeleteAll();
		return true;
	}
}
