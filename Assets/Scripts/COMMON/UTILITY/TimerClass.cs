using UnityEngine;

[AddComponentMenu( "Common/Timer class" )]

public class TimerClass : ScriptableObject
{
	public bool isTimerRunning= false;

	private float timeElapsed= 0.0f;
	private float currentTime= 0.0f;
	private float lastTime= 0.0f;	
	private float timeScaleFactor= 1.0f; // <-- If you need to scale time, change this!
	
	private string timeString;
	private string hour;
	private string minutes;
	private string seconds;
	private string mills;
	
	private int aHour;
	private int aMinute;
	private int aSecond;
	private int aMillis;
	private int tmp;
	private int aTime;
	
	private GameObject callback;
	
	public void UpdateTimer ()
	{
		// calculate the time elapsed since the last Update()
		timeElapsed=Mathf.Abs(Time.realtimeSinceStartup-lastTime);
	   	
		// if the timer is running, we add the time elapsed to the current time (advancing the timer)
		if(isTimerRunning)
		{
			currentTime+=timeElapsed*timeScaleFactor;
	    }
		
		// store the current time so that we can use it on the next update
		lastTime=Time.realtimeSinceStartup;
	}

	/// <summary>
	/// Starts the timer.
	/// </summary>
	public void StartTimer ()
	{
		// set up initial variables to start the timer
		isTimerRunning=true;
	    lastTime=Time.realtimeSinceStartup;
	}

	/// <summary>
	/// Stops the timer.
	/// </summary>
	public void StopTimer ()
	{
		// stop the timer
		isTimerRunning=false;
		
		// carry out an update to the timer
		UpdateTimer();
	}

	/// <summary>
	/// ResetTimer will set the timer back to zero
	/// </summary>
	public void ResetTimer ()
	{
	    timeElapsed=0.0f;
	    lastTime=0.0f;
	    currentTime=0.0f;
	    lastTime=Time.realtimeSinceStartup;
		
		// carry out an update to the timer
		UpdateTimer();
	}

	/// <summary>
	/// Gets the formatted time (##:##:##).
	/// </summary>
	/// <returns>The formatted time.</returns>
	/// <param name="val">Value.</param>
	public string GetFormattedTime (float val) {
		// grab hours
		aHour = (int)val/3600;
		aHour=aHour%24;

		// grab minutes
		aMinute=(int)val/60;
		aMinute=aMinute%60;

		// grab seconds
		aSecond=(int)val%60;

		// grab milliseconds
		aMillis=(int)(val*100)%100;

		// format strings for individual mm/ss/mills
		tmp=(int)aSecond;
		seconds=tmp.ToString();
		if (seconds.Length < 2)
			seconds = string.Format ("0{0}", seconds);

		tmp=(int)aMinute;
		minutes=tmp.ToString();
		if (minutes.Length < 2)
			minutes = string.Format ("0{0}", minutes);

		tmp=(int)aHour;
		hour=tmp.ToString();
		if (hour.Length < 2)
			hour = string.Format ("0{0}", hour);

		tmp=(int)aMillis;
		mills=tmp.ToString();
		if (mills.Length < 2)
			mills = string.Format ("0{0}", mills);

		// pull together a formatted string to return
		timeString = string.Format( "{0}:{1}:{2}", minutes, seconds, mills);

		return timeString;
	}

	/// <summary>
	/// Gets the formatted time (##:##:##).
	/// </summary>
	/// <returns>The formatted time.</returns>
	public string GetFormattedTime ()
	{	
		// carry out an update to the timer so it is 'up to date'
		UpdateTimer();

		return GetFormattedTime (currentTime);
	}

	/// <summary>
	/// GetTime. Call UpdateTimer() before trying to use this function, otherwise the time value will not be up to date.
	/// </summary>
	/// <returns>The time int.</returns>
	public int GetTime ()
	{
	    return (int)(currentTime);
	}
}