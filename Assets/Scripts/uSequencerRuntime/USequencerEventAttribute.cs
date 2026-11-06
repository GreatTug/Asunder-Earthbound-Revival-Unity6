using System;

public class USequencerEventAttribute : Attribute
{
	private string eventPath;

	public string EventPath
	{
		get
		{
			return eventPath;
		}
	}

	public USequencerEventAttribute(string myEventPath)
	{
		eventPath = myEventPath;
	}
}
