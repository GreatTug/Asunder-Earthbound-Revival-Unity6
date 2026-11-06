using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class LoaderXML : MonoBehaviour
{
	public TextAsset _GameScriptXML;

	private XmlDocument xmlDoc = new XmlDocument();

	private List<Dictionary<string, string>> _characterList = new List<Dictionary<string, string>>();

	private Dictionary<string, string> characterElements;

	private void Start()
	{
		loadXML();
		getCharacters();
		MonoBehaviour.print(GetCharacterDialogue("harold", 2));
	}

	private void loadXML()
	{
		xmlDoc.LoadXml(_GameScriptXML.text);
	}

	private void getCharacters()
	{
		XmlNodeList elementsByTagName = xmlDoc.GetElementsByTagName("character");
		foreach (XmlNode item in elementsByTagName)
		{
			XmlNodeList childNodes = item.ChildNodes;
			characterElements = new Dictionary<string, string>();
			foreach (XmlNode item2 in childNodes)
			{
				if (item2.Name == "name")
				{
					characterElements.Add("name", item2.InnerText);
				}
				if (item2.Name == "dialogue")
				{
					characterElements.Add("dialogue", item2.InnerText);
				}
			}
			_characterList.Add(characterElements);
		}
	}

	public string GetValueByName(string aValue)
	{
		string value = string.Empty;
		_characterList[0].TryGetValue(aValue, out value);
		return value;
	}

	private string GetCharacterDialogue(string aCharacterName, int aDialogueID)
	{
		string value = string.Empty;
		_characterList[3].TryGetValue("dialogue", out value);
		return value;
	}

	private void Update()
	{
	}
}
