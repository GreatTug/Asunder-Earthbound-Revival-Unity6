using System;

public class USequencerFriendlyNameAttribute : Attribute
{
	private string friendlyName;

	public string FriendlyName
	{
		get
		{
			return friendlyName;
		}
	}

	public USequencerFriendlyNameAttribute(string myFriendlyName)
	{
		friendlyName = myFriendlyName;
	}
}
