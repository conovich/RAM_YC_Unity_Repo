using UnityEngine;
using System.Collections;

public class ObjectLogTrack : MonoBehaviour, ILoggable {

	//dbLog experimentLog { get { return Experiment.Instance.log; } }
	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

	SpawnableObject spawnedObj;
	string nameToLog;

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

		//log that object was spawned
		if (ExperimentSettings.shouldLog) {
			LogSpawned();
		}
	}
	
	// LOGGING SHOULD BE INDEPENDENT OF FRAME RATE
	void FixedUpdate () {
		if (ExperimentSettings.shouldLog) {
			Log ();
		}
	}


	public void Log ()
	{
		LogPosition();
		LogRotation();
		LogVisibility();
	}

	void LogSpawned(){
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, nameToLog + " SPAWNED ");
	}

	void LogPosition(){
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, nameToLog + " POSITION " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
	}
	
	void LogRotation(){
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, nameToLog + " ROTATION " + transform.rotation.eulerAngles.x + " " + transform.rotation.eulerAngles.y + " " + transform.rotation.eulerAngles.z);
	}
	
	void LogVisibility(){
		if (spawnedObj != null) {
			experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, nameToLog + " VISIBILITY " + spawnedObj.isVisible);
		}
	}

	void LogDestroy(){
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, nameToLog + " DESTROYED ");
	}

	void OnDestroy(){
		if (ExperimentSettings.shouldLog) {
			LogDestroy();
		}
	}

}