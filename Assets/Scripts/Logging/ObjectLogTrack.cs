using UnityEngine;
using System.Collections;

public class ObjectLogTrack : MonoBehaviour, ILoggable {

	//dbLog experimentLog { get { return Experiment.Instance.log; } }
	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

	SpawnableObject spawnedObj;
	string nameToLog;

	// Use this for initialization
	void Start () {
		spawnedObj = GetComponent<SpawnableObject> ();
		if (spawnedObj != null) {
			nameToLog = spawnedObj.GetName (); //important, because otherwise the logged name will have "(Clone)" attached to it.
		}
		else {
			nameToLog = gameObject.name;
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
	}
	
	void LogPosition(){
		experimentLog.Log (nameToLog + " POSITION " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
	}
	
	void LogRotation(){
		experimentLog.Log (nameToLog + " ROTATION " + transform.rotation.eulerAngles.x + " " + transform.rotation.eulerAngles.y + " " + transform.rotation.eulerAngles.z);
	}

	void LogVisibility(){
		if (spawnedObj != null) {
			experimentLog.Log (nameToLog + " VISIBILITY " + spawnedObj.isVisible);
		}
	}

}