using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
	public int id;

	public new string name;

	public string image;

	[NonSerialized]
	public ArrayList audioPaths = new ArrayList();

	[NonSerialized]
	public ArrayList dialoguesequences = new ArrayList();

	[NonSerialized]
	public ArrayList dialogues;

	public string description;

	private ArrayList _currentDialogueSequence = new ArrayList();

	private int _lastSequenceID = -1;

	private int _LastDialogueID = -1;

	public string playDialogue(int anID)
	{
		_LastDialogueID = anID;
		if (_currentDialogueSequence != null && _LastDialogueID >= 0 && _LastDialogueID < _currentDialogueSequence.Count)
		{
			object val = _currentDialogueSequence[_LastDialogueID];
			return val != null ? val.ToString() : string.Empty;
		}
		return string.Empty;
	}

	public string playNextDialogue()
	{
		_LastDialogueID++;
		if (_currentDialogueSequence != null && _LastDialogueID >= 0 && _LastDialogueID < _currentDialogueSequence.Count)
		{
			object val = _currentDialogueSequence[_LastDialogueID];
			return val != null ? val.ToString() : string.Empty;
		}
		_LastDialogueID = -1;
		return "DIALOGUE_COMPLETE";
	}

	public void switchSequence(int anID)
	{
		_lastSequenceID = anID;
		if (dialoguesequences != null && anID >= 0 && anID < dialoguesequences.Count)
		{
			_currentDialogueSequence = (ArrayList)dialoguesequences[anID];
		}
		else
		{
			_currentDialogueSequence = new ArrayList();
		}
	}

	public void nextSequence()
	{
		_lastSequenceID++;
		if (dialoguesequences != null && _lastSequenceID >= 0 && _lastSequenceID < dialoguesequences.Count)
		{
			_currentDialogueSequence = (ArrayList)dialoguesequences[_lastSequenceID];
		}
		else
		{
			_currentDialogueSequence = new ArrayList();
		}
	}

	public void PlayCurrentAudioFile()
	{
		string relPath = (_lastSequenceID >= 0 && _lastSequenceID < audioPaths.Count && audioPaths[_lastSequenceID] != null) ? audioPaths[_lastSequenceID].ToString() : string.Empty;
		if (!string.IsNullOrEmpty(relPath))
		{
			AudioSource src = base.gameObject.GetComponent<AudioSource>();
			if (src != null)
			{
				AudioClip clip = Resources.Load("Audio/Dialogue/" + relPath) as AudioClip;
				AppManager.ApplyUnity4AudioSpatialBlend(src, clip);
				src.clip = clip;
				src.Play();
			}
		}
	}

	public int getCurrentDialogueID()
	{
		return _LastDialogueID;
	}

	public int getNextSubtitleLength()
	{
		if (dialoguesequences != null && _lastSequenceID >= 0 && _lastSequenceID < dialoguesequences.Count)
		{
			ArrayList arrayList = (ArrayList)dialoguesequences[_lastSequenceID];
			return arrayList != null ? arrayList.Count : 0;
		}
		return 0;
	}

	public override string ToString()
	{
		string text = string.Empty;
		if (dialogues != null)
		{
			foreach (string dialogue in dialogues)
			{
				text = text + dialogue + ", ";
			}
		}
		string text3 = "id: " + Convert.ToInt16(id);
		text3 = text3 + "\r\nTitle: " + name;
		text3 = text3 + "\r\nImage: " + image;
		text3 = text3 + "\r\nDialogues: " + text;
		return text3 + "\r\nDescription: " + description;
	}
}
