using UnityEngine;

internal class CharacterPlane : MonoBehaviour
{
	private Character character;

	private void Start()
	{
	}

	public void LoadCharacter(Character aCharacter)
	{
		character = aCharacter;
		if (character != null)
		{
			Renderer rend = GetComponent<Renderer>();
			if (rend != null)
			{
				rend.material.mainTexture = loadTextureResource(character.image);
			}
		}
	}

	private void OnMouseDown()
	{
		if (character != null)
		{
			Debug.Log("Clicked on character: " + character.name);
			Debug.Log("Dialogue #" + (character.getCurrentDialogueID() + 1) + ": " + character.playNextDialogue() + " -" + character.name);
		}
	}

	private Texture loadTextureResource(string aPath)
	{
		if (string.IsNullOrEmpty(aPath))
		{
			return null;
		}
		return Resources.Load(aPath) as Texture;
	}
}
