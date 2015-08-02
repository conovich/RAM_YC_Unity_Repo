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

	public void ReplayScene(){ //gets called via replay button in the main menu scene
		objsInSceneDict.Clear ();

		if (LogFilePathInputField != null) {
			SetLogFile (LogFilePathInputField.text);
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


	//TODO: make log file just log the time elapsed????
	long GetMillisecondDifference(long baseMS, long newMS){
		return (newMS - baseMS); 
	}


	//THIS PARSING DEPENDS GREATLY ON THE FORMATTING OF THE LOG FILE.
	//IF THE FORMATTING OF THE LOG FILE IS CHANGED, THIS WILL VERY LIKELY HAVE TO CHANGE AS WELL.
	IEnumerator ProcessLogFile(){
		long currentFrame = 0;

		if (logFilePath != "") { 

			fileReader = new StreamReader (logFilePath);
		
			currentLogFileLine = fileReader.ReadLine (); //the first line in the file should be the date.
			currentLogFileLine = fileReader.ReadLine (); //the second line should be the first real line with logged data

			string[] splitLine;

			bool hasFinishedSettingFrame = false;
		
			//PARSE
			while (currentLogFileLine != null) {

				splitLine = currentLogFileLine.Split(' ');

				if(splitLine.Length > 0){
					for (int i = 0; i < splitLine.Length; i++){

						//0 -- timestamp
						if (i == 0){

						}
						//1 -- name of object
						else if (i == 1){
							string objName = splitLine[i];
							
							if(objName != "Mouse" && objName != "Keyboard"){

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

										//TODO: when logging more than just the avatar, the frame has only been finished if and only if:
											//all objects have had all of their properties logged!
										//hasFinishedSettingFrame = true;

									}
								}
								else{
									Debug.Log("REPLAY: No obj in scene named " + objName);
								}
								
							}
						}

						else if(i == 6){ //fixed update frame count
							long newFrame = long.Parse(splitLine[6]);
							if(newFrame != currentFrame){
								currentFrame = newFrame;
								hasFinishedSettingFrame = true;


								Debug.Log(currentFrame);
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
			}
		}

		yield return 0;
	}


}
