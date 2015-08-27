using UnityEngine;
using System.Collections;

public class CameraLogTrack : MonoBehaviour, ILoggable {

	Logger_Threading experimentLog { get { return Experiment.Instance.subjectLog; } }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(ExperimentSettings.isLogging){
			Log ();
		}
	}

	public void Log(){
		LogCamera ();
	}

	void LogCamera(){
		Camera myCamera = GetComponent<Camera>();
		if(myCamera != null){
			experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), gameObject.name + ",CAMERA_ENABLED," + myCamera.enabled);
		}
	}
}
