using UnityEngine;
using System.Collections;

public class ObjectLogTrack : MonoBehaviour, ILoggable {

	//dbLog experimentLog { get { return Experiment.Instance.log; } }
	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Log ();
	}


	public void Log ()
	{
		LogPosition();
		LogRotation();
	}
	
	void LogPosition(){
		//experimentLog.log ("AVATAR POSITION: " + transform.position, Application.loadedLevel);
		experimentLog.Log (gameObject.name + " POSITION " + transform.position);
	}
	
	void LogRotation(){
		//experimentLog.log ("AVATAR ROTATION: " + transform.rotation.eulerAngles.x + " " + transform.rotation.eulerAngles.y + " " + transform.rotation.eulerAngles.z, Application.loadedLevel);
		experimentLog.Log (gameObject.name + " ROTATION " + transform.rotation.eulerAngles.x + " " + transform.rotation.eulerAngles.y + " " + transform.rotation.eulerAngles.z);
	}

}
