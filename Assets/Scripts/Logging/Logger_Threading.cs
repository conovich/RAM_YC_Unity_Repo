﻿using UnityEngine;
using System.Collections;


using System;
using System.IO;
using System.Collections.Generic;
//using System.Runtime.InteropServices;
using System.Threading;



//CLASS BASED OFF OF: http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html


public class ThreadedJob
{
	private bool m_IsDone = false;
	private object m_Handle = new object();
	private System.Threading.Thread m_Thread = null;
	public bool IsDone
	{
		get
		{
			bool tmp;
			lock (m_Handle)
			{
				tmp = m_IsDone;
			}
			return tmp;
		}
		set
		{
			lock (m_Handle)
			{
				m_IsDone = value;
			}
		}
	}

	public virtual void Start()
	{
		m_Thread = new System.Threading.Thread(Run);
		m_Thread.Start();
	}
	public virtual void Abort()
	{
		m_Thread.Abort();
	}

	protected virtual void ThreadFunction() { }

	protected virtual void OnFinished() { }

	public virtual bool Update()
	{
		if (IsDone)
		{
			OnFinished();
			return true;
		}
		return false;
	}
	private void Run()
	{
		ThreadFunction();
		IsDone = true;
	}
}




public class LoggerQueue
{

	public Queue<String> logQueue;
	
	public LoggerQueue(){
		logQueue = new Queue<String> ();
	}
	
	public void AddToLogQueue(string newLogInfo){
		lock (logQueue) {
			logQueue.Enqueue (newLogInfo);
		}
	}
	
	public String GetFromLogQueue(){
		string toWrite = "";
		lock (logQueue) {
			toWrite = logQueue.Dequeue ();
			if (toWrite == null) {
					toWrite = "";
			}
		}
		return toWrite;
	}

}

public class LoggerWriter : ThreadedJob
{
	public bool isRunning = false;

	//LOGGING
	protected long microseconds = 1;
	protected string workingFile = "";
	private StreamWriter logfile;
	private LoggerQueue loggerQueue;
	
	public LoggerWriter(string filename, LoggerQueue newLoggerQueue) {
		workingFile = filename;
		logfile = new StreamWriter ( workingFile, true );
		
		loggerQueue = newLoggerQueue;
	}
	
	public LoggerWriter() {
		
	}
	
	protected override void ThreadFunction()
	{
		isRunning = true;
		// Do your threaded task. DON'T use the Unity API here
		while (isRunning) {
			while(loggerQueue.logQueue.Count > 0){
				log (loggerQueue.GetFromLogQueue());
			}
		}

		close ();

	}
	protected override void OnFinished()
	{
		// This is executed by the Unity main thread when the job is finished

	}

	public void End(){
		isRunning = false;
	}

	public virtual void close()
	{
		//logfile.WriteLine ("EOF");
		logfile.Flush ();
		logfile.Close();	
		Debug.Log ("flushing & closing");
	}
	
	
	public virtual void log(string msg) { //took out  ( ... , int level)

		long tick = DateTime.Now.Ticks;
		//long seconds = tick / TimeSpan.TicksPerSecond;
		long milliseconds = tick / TimeSpan.TicksPerMillisecond;
		microseconds = tick / 10;
		//Debug.Log(milliseconds);
		//Debug.Log(Time.frameCount + ": " + Event.current);
		
		//logfile.WriteLine( milliseconds + "\t0\t" + msg ); //not sure what the "\t0\t" was for.

		logfile.WriteLine (msg);
	}

}

public class Logger_Threading : MonoBehaviour{
	public static string LogTextSeparator = "\t";

	LoggerQueue myLoggerQueue;
	LoggerWriter myLoggerWriter;

	long frameCount;
	
	public string fileName;

	void Start ()
	{
		if (ExperimentSettings.isLogging) {
			myLoggerQueue = new LoggerQueue ();
			myLoggerWriter = new LoggerWriter (fileName, myLoggerQueue);
		
			myLoggerWriter.Start ();

			myLoggerWriter.log ("DATE: " + DateTime.Now.ToString ("M/d/yyyy")); //might not be needed
		}
	}

	public Logger_Threading(string file){
		fileName = file;
	}

	//logging itself can happen in regular update. the rate at which ILoggable objects add to the log Queue should be in FixedUpdate for framerate independence.
	void Update()
	{
		frameCount++;
		if (myLoggerWriter != null)
		{
			if (myLoggerWriter.Update())
			{
				// Alternative to the OnFinished callback
				myLoggerWriter = null;
			}
		}
	}

	public long GetFrameCount(){
		return frameCount;
	}


	public void Log(long timeLogged, long frame, string newLogInfo){
		if (myLoggerQueue != null) {
			myLoggerQueue.AddToLogQueue (timeLogged + LogTextSeparator + frame + LogTextSeparator + newLogInfo);
		}
	}

	//must be called by the experiment class OnApplicationQuit()
	public void close(){
		//Application stopped running -- close() was called
		//applicationIsRunning = false;
		myLoggerWriter.End ();
	}



}