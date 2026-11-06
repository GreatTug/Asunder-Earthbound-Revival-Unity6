using System.Collections;
using System.Xml;
using UnityEngine;

public class LoadCharacterXML
{
	public TextAsset GameAsset;

	private XmlDocument _xmlDoc = new XmlDocument();

	public LoadCharacterXML(TextAsset aTextAsset)
	{
		GameAsset = aTextAsset;
		if (GameAsset != null && !string.IsNullOrEmpty(GameAsset.text))
		{
			_xmlDoc.LoadXml(GameAsset.text);
		}
	}

	public void processCharacter(Character aCharacter)
	{
		if (aCharacter == null || _xmlDoc == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = _xmlDoc.SelectNodes("characters/" + aCharacter.name);
		if (xmlNodeList == null)
		{
			return;
		}
		foreach (XmlNode item in xmlNodeList)
		{
			aCharacter.dialogues = new ArrayList();
			XmlNodeList sequences = item.SelectNodes("dialoguesequence");
			if (sequences == null)
			{
				continue;
			}
			foreach (XmlNode item2 in sequences)
			{
				string audioFile = getAttributeValueByName(item2, "audioFile");
				if (audioFile != null)
				{
					aCharacter.audioPaths.Add(audioFile);
				}
				ArrayList arrayList = new ArrayList();
				if (item2.ChildNodes != null)
				{
					foreach (XmlNode childNode in item2.ChildNodes)
					{
						arrayList.Add(childNode.InnerText);
					}
				}
				aCharacter.dialoguesequences.Add(arrayList);
			}
		}
	}

	private void LoadCharacterByName(Character aCharacter)
	{
		if (aCharacter == null)
		{
			return;
		}
		GameObject gameObject = GameObject.Find(aCharacter.name + aCharacter.id);
		if (gameObject != null)
		{
			gameObject.SendMessage("LoadCharacter", aCharacter, SendMessageOptions.DontRequireReceiver);
		}
	}

	private string getAttributeValueByName(XmlNode node, string attributeName)
	{
		if (node != null && node.Attributes != null)
		{
			XmlNode item = node.Attributes.GetNamedItem(attributeName);
			if (item != null)
			{
				return item.Value;
			}
		}
		return string.Empty;
	}
}
