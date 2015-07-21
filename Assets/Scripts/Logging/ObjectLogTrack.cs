using UnityEngine;
using System.Collections;

public class ObjectLogTrack : MonoBehaviour, ILoggable {

	//dbLog experimentLog { get { return Experiment.Instance.log; } }
	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }


	// Use this for initialization
	void Start () {
	
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
		//experimentLog.log ("AVATAR POSITION: " + transform.position, Application.loadedLevel);
		experimentLog.Log (gameObject.name + " POSITION " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
	}
	
	void LogRotation(){
		//experimentLog.log ("AVATAR ROTATION: " + transform.rotation.eulerAngles.x + " " + transform.rotation.eulerAngles.y + " " + transform.rotation.eulerAngles.z, Application.loadedLevel);
		experimentLog.Log (gameObject.name + " ROTATION " + transform.rotation.eulerAngles.x + " " + transform.rotation.eulerAngles.y + " " + transform.rotation.eulerAngles.z);
	}

}
