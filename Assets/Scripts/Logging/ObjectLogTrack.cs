using UnityEngine;
using System.Collections;

public class ObjectLogTrack : MonoBehaviour, ILoggable {

	//dbLog experimentLog { get { return Experiment.Instance.log; } }
	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

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
		if (ExperimentSettings.shouldLog) {
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
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), nameToLog + "," + "SPAWNED");
	}

	void LogPosition(){
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), nameToLog + ",POSITION," + transform.position.x + "," + transform.position.y + "," + transform.position.z);
	}
	
	void LogRotation(){
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), nameToLog + ",ROTATION," + transform.rotation.eulerAngles.x + "," + transform.rotation.eulerAngles.y + "," + transform.rotation.eulerAngles.z);
	}
	
	void LogVisibility(){
		if (spawnedObj != null) {
			experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), nameToLog + ",VISIBILITY," + spawnedObj.isVisible);
		}
	}

	void LogDestroy(){
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), nameToLog + ",DESTROYED");
	}

	void OnDestroy(){
		if (ExperimentSettings.shouldLog) {
			LogDestroy();
		}
	}

}