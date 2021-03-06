using UnityEngine;
using System.Collections;

public class ObjectLogTrack : LogTrack {
	SpawnableObject spawnedObj;
	string nameToLog;

	bool firstLog = false; //should log spawned on the first log

	//if we want to only log objects when something has changed... should start with keep track of last positions/rotations.
	//or I could set up some sort of a delegate system.
	//Vector3 lastPosition;
	//Vector3 lastRotation;
	//bool lastVisibility;

	// Use this for initialization
	void Start () {
		//NOTE: be wary of logging objects with the same name. might be an issue for things like replaying the scene.
		spawnedObj = GetComponent<SpawnableObject> ();
		if (spawnedObj != null) {
			nameToLog = spawnedObj.GetName (); //important, because otherwise the logged name will have "(Clone)" attached to it.
		}
		else {
			nameToLog = gameObject.name;
		}
	}

	void Update(){
		if (ExperimentSettings.isLogging) {
			if(!firstLog){
				firstLog = true;
			}
			Log ();
		}
	}

	// LOGGING SHOULD BE INDEPENDENT OF FRAME RATE
	void FixedUpdate () {

	}


	public void Log ()
	{
		LogPosition();
		LogRotation();
		LogVisibility();
	}

	void LogSpawned(){
		subjectLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "SPAWNED");
	}

	void LogPosition(){
		subjectLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "POSITION" + separator + transform.position.x + separator + transform.position.y + separator + transform.position.z);
	}
	
	void LogRotation(){
		subjectLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "ROTATION" + separator + transform.rotation.eulerAngles.x + separator + transform.rotation.eulerAngles.y + separator + transform.rotation.eulerAngles.z);
	}
	
	void LogVisibility(){
		if (spawnedObj != null) {
			subjectLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "VISIBILITY" + separator + spawnedObj.isVisible);
		}
	}

	void LogDestroy(){
		subjectLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "DESTROYED");
	}

	void OnDestroy(){
		if (ExperimentSettings.isLogging) {
			LogDestroy();
		}
	}

}