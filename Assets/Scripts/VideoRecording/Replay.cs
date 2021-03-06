﻿using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.UI;


public class Replay : MonoBehaviour {

	//image recording
	public ScreenRecorder MyScreenRecorder;
	public Toggle PNGToggle;
	static bool isRecording;

	//GUI
	public InputField LogFilePathInputField;
	public InputField FPSInputField; //Frames Per Second
	static int FPS = 30; //use 30 for default


	//I/O
	StreamReader fileReader;
	static string logFilePath;
	string currentLogFileLine;


	//keeping track of objects
	Dictionary<String, GameObject> objsInSceneDict;


	//a bool to determine if we should start the log file processing. replay should start once 
	static bool shouldStartProcessingLog = false;

	// Use this for initialization
	void Start () {
		objsInSceneDict = new Dictionary<String, GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (ExperimentSettings.isReplay && Application.loadedLevel == 1) { //if we're in the main scene, which in the case of YC1 corresponds to 1
			if( shouldStartProcessingLog ){
				//process log file;
				Debug.Log("PROCESSING LOG FILE OH HEYYYY");
				StartCoroutine(ProcessLogFile());
				
				shouldStartProcessingLog = false;
			}
		}
	}

	public void SetIsRecording(){
		isRecording = PNGToggle.isOn;
	}

	public void ReplayScene(){ //gets called via replay button in the main menu scene
		objsInSceneDict.Clear ();

		if (LogFilePathInputField != null) {
			SetLogFile (LogFilePathInputField.text);
		}

		if (FPSInputField != null) {
			SetFPS (int.Parse(FPSInputField.text));
		}

		
		try 
		{
			// Create an instance of StreamReader to read from a file. 
			// The using statement also closes the StreamReader. 
			using (fileReader = new StreamReader (logFilePath)) 
			{
				//open scene
				ExperimentSettings.Instance.SetReplayTrue();
				SceneController.Instance.LoadExperiment(); //will load the experiment scene. Experiment.cs will not run the experiment because ExperimentSettings.isReplay = true!
				
				shouldStartProcessingLog = true;
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

	void SetFPS(int newFPS){
		FPS = newFPS;
	}


	//TODO: make log file just log the time elapsed????
	long GetMillisecondDifference(long baseMS, long newMS){
		return (newMS - baseMS); 
	}

	void RecordScreenShot(){
		if(MyScreenRecorder != null){
			//will check if it's supposed to record or not
			//also will wait until endofframe in order to take the shot
			MyScreenRecorder.TakeNextContinuousScreenShot();
		}
		else{
			Debug.Log("No screen recorder attached!");
		}
	}
	
	//THIS PARSING DEPENDS GREATLY ON THE FORMATTING OF THE LOG FILE.
	//IF THE FORMATTING OF THE LOG FILE IS CHANGED, THIS WILL VERY LIKELY HAVE TO CHANGE AS WELL.
	IEnumerator ProcessLogFile(){

		long currentFrame = 0;
		long currentTimeStamp = 0;
		long lastTimeRecorded = 0;
		long timeDifference = 0;


		//if (logFilePath != "") { 

		if (FPS == 0) {
			Debug.Log("ERROR: SET FRAMES PER SECOND TO SOMETHING NOT ZERO.");
		}
		
		float secondsPerFrame = 1.0f / (float)FPS;
		int millisecondsPerFrame = Mathf.RoundToInt( secondsPerFrame * 1000 );
		Debug.Log ("MS per frame!: " + millisecondsPerFrame + "FPS: " + FPS);


		fileReader = new StreamReader (logFilePath);
	
		currentLogFileLine = fileReader.ReadLine (); //the first line in the file should be the date.
		currentLogFileLine = fileReader.ReadLine (); //the second line should be the first real line with logged data

		string[] splitLine;

		bool hasFinishedSettingFrame = false;

		char splitCharacter = Logger_Threading.LogTextSeparator.ToCharArray () [0];

		//PARSE
		while (currentLogFileLine != null) {

			splitLine = currentLogFileLine.Split(splitCharacter);

			if(splitLine.Length > 0){
				for (int i = 0; i < splitLine.Length; i++){

					//0 -- timestamp
					if (i == 0){

						//Debug.Log(currentFrame + " " + splitLine[0] + " " + splitLine[1] + " " + splitLine[2]);

						currentTimeStamp = long.Parse(splitLine[i]);
						timeDifference = currentTimeStamp - lastTimeRecorded; //gets time between log file lines
					}

					//1 -- frame
					else if(i == 1){
						long readFrame = long.Parse(splitLine[i]);
						
						while(readFrame != currentFrame){
							currentFrame++;
							hasFinishedSettingFrame = true;
							
							//Debug.Log(currentFrame);
						}
						
						//first frame case -- need to set the last time recorded as the current time stamp
						if (currentFrame == 1 && hasFinishedSettingFrame){ //record frame 0 before reading and setting the rest of frame 1...
							lastTimeRecorded = currentTimeStamp;
							
							if(isRecording){
								RecordScreenShot();
							}
							yield return 0; //advance the game a frame before continuing
							
						}
						else if(timeDifference > millisecondsPerFrame){
							int numFramesToCapture = Mathf.FloorToInt( (float)timeDifference / millisecondsPerFrame ); //EXAMPLE: if time passed is 30 milliseconds and the required time per frame is 15 milliseconds, you should capture 2 frames
							
							//record and wait the appropriate number of frames
							for(int j = 0; j < numFramesToCapture; j++){
								if(isRecording){
									RecordScreenShot();
								}
								yield return 0; //advance the game a frame before continuing
							}
							
							long timeToAddToLastTimeStamp = numFramesToCapture*millisecondsPerFrame; //EXAMPLE: if you capture 2 frames, add 2 frames worth of time to the last time that we recorded a frame to a PNG
							lastTimeRecorded += timeToAddToLastTimeStamp;
							
							//DEBUG TO CHECK THAT THE TIME INCREMENT IS WORKING PROPERLY
							//Debug.Log("Time to record screenshot. Frame: " + currentFrame + " Time Stamp: " + currentTimeStamp + " Last Time Recorded: " + lastTimeRecorded + " Time Difference: " + timeDifference + " Num Frames Recorded: " + numFramesToCapture);
						}
					}

					//2 -- name of object
					else if (i == 2){
						string objName = splitLine[i];
						
						if(objName != "Mouse" && objName != "Keyboard" && objName != "Trial Info"){

							GameObject objInScene;

							if(objsInSceneDict.ContainsKey(objName)){
								
								objInScene = objsInSceneDict[objName];

							}
							else{

								objInScene = GameObject.Find(objName);

								if(objInScene != null){
									objsInSceneDict.Add(objName, objInScene);
								}
								else{ //if the object is not in the scene, but is in the log file, we should instantiate it!
										//we could also check for the SPAWNED keyword
									objInScene = Experiment.Instance.objectController.ChooseSpawnableObject(objName);
									if(objInScene != null){
										objInScene = Experiment.Instance.objectController.SpawnObject(objInScene, Vector3.zero); //position and rotation should be set next...

										objsInSceneDict.Add(objName, objInScene);
									}
								}
							}
							if(objInScene != null){
								//NOW MOVE & ROTATE THE OBJECT.
								string loggedProperty = splitLine[i+1];
								
								if(loggedProperty == "POSITION"){
									
									float posX = float.Parse(splitLine[i+2]);
									float posY = float.Parse(splitLine[i+3]);
									float posZ = float.Parse(splitLine[i+4]);
									
									objInScene.transform.position = new Vector3(posX, posY, posZ);
									
								}
								else if(loggedProperty == "ROTATION"){
									
									float rotX = float.Parse(splitLine[i+2]);
									float rotY = float.Parse(splitLine[i+3]);
									float rotZ = float.Parse(splitLine[i+4]);

									objInScene.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ); //TODO: TEST THIS.

								}
								else if(loggedProperty == "VISIBILITY"){
									SpawnableObject spawnedObj = objInScene.GetComponent<SpawnableObject>();
									if(spawnedObj != null){
										bool visibleState = true;
										if(splitLine[i+2] == "false" || splitLine[i+2] == "False"){
											visibleState = false;
										}
										spawnedObj.TurnVisible(visibleState);
									}
									else{
										Debug.Log("no spawnable object!");
									}
								}

								else if(loggedProperty == "CAMERA_ENABLED"){
									Camera objCamera = objInScene.GetComponent<Camera>();
									if(objCamera != null){
										if(splitLine[i+2] == "true" || splitLine[i+2] == "True"){
											objCamera.enabled = true;
										}
										else{
											objCamera.enabled = false;
										}
									}
								}
								else if(loggedProperty == "DESTROYED"){
									Debug.Log("Destroying object! " + objInScene.name);
									GameObject.Destroy(objInScene);
								}

								//UI - TEXT
								else if(loggedProperty == "TEXT"){
									Text text = objInScene.GetComponent<Text>();
									for(int j = i+2; j < splitLine.Length; j++){ //the text may have been split unnecessarily if there is a splitCharacter in the text
										text.text += splitLine[j]; //add each piece of the split text
										if(j+1 < splitLine.Length){
											text.text += Logger_Threading.LogTextSeparator; //add back the split characters into the text!
										}
									}

									text.text = text.text.Replace("_NEWLINE_", "\n");
								}
								else if(loggedProperty == "TEXT_COLOR"){
									Text text = objInScene.GetComponent<Text>();
									float r = float.Parse(splitLine[i+2]);
									float g = float.Parse(splitLine[i+3]);
									float b = float.Parse(splitLine[i+4]);
									float a = float.Parse(splitLine[i+5]);
									text.color = new Color(r, g, b, a);
								}

								//UI - PANEL
								else if (loggedProperty == "PANEL"){
									Image image = objInScene.GetComponent<Image>();
									float r = float.Parse(splitLine[i+2]);
									float g = float.Parse(splitLine[i+3]);
									float b = float.Parse(splitLine[i+4]);
									float a = float.Parse(splitLine[i+5]);
									image.color = new Color(r, g, b, a);
								}

							}
							else{
								Debug.Log("REPLAY: No obj in scene named " + objName);
							}
							
						}
					}

				}


			}

			//read the next line at the end of the while loop
			currentLogFileLine = fileReader.ReadLine ();

			if(hasFinishedSettingFrame){ //
				//yield return new WaitForFixedUpdate();	 //REPLAY BASED ON FIXEDUPDATE FOR FRAMERATE INDEPENDENCE (logging was also logged via FixedUpdate())
				yield return 0; //WHILE LOGGED ON FIXED UPDATE, REPLAY ON UPDATE TO GET A CONSTANT #RENDERED FRAMES

				hasFinishedSettingFrame = false;

			}
			//}
		}

		//take the last screenshot
		if (isRecording) {
			RecordScreenShot ();
		}
		yield return 0;
		Application.LoadLevel(0); //return to main menu
	}

}
