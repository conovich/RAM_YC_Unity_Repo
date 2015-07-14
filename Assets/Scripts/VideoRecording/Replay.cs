using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Replay : MonoBehaviour {

	//GUI
	public InputField LogFilePathInputField;


	//I/O
	StreamReader fileReader;
	static string logFilePath;
	string currentLogFileLine;


	//keeping track of time and objects
	long timeElapsedMS = 0;
	List<GameObject> currentObjectsInScene;


	// Use this for initialization
	void Start () {
		currentObjectsInScene = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		timeElapsedMS += (long)(Time.deltaTime * 100); //Time.deltaTime is in seconds, want to bring it into milliseconds

		//ProcessLogFile ();
	}

	public void ReplayScene(){
		SetLogFile (LogFilePathInputField.text);


		try 
		{
			// Create an instance of StreamReader to read from a file. 
			// The using statement also closes the StreamReader. 
			using (fileReader = new StreamReader (logFilePath)) 
			{
				//open scene
				ExperimentSettings.Instance.SetReplayTrue();
				SceneController.Instance.LoadExperiment(); //TODO: make sure that the typical experiment loop doesn't happen.......
				
				//process log file;
				Debug.Log("PROCESSING LOG FILE OH HEYYYY");
				ProcessLogFile();
			}
		}
		catch (Exception e) 
		{
			Debug.Log("Invalid log file path. Cannot replay.");
		}



	}

	void SetLogFile(string chosenLogFile){
		logFilePath = chosenLogFile;
		Debug.Log (logFilePath);
	}

	void ProcessLogFile(){
		if (logFilePath != "") { 

			fileReader = new StreamReader (logFilePath);
		
			currentLogFileLine = fileReader.ReadLine ();
		
			while (currentLogFileLine != null) {
				//check for (1) in instructions? (2) objs in scene (incl. avatar), pos & rot


			}
		}
	}


}
