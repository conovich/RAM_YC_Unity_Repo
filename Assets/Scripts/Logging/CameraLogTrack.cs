﻿using UnityEngine;
using System.Collections;

public class CameraLogTrack : MonoBehaviour, ILoggable {

	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(ExperimentSettings.shouldLog){
			Log ();
		}
	}

	public void Log(){
		LogCamera ();
	}

	void LogCamera(){
		Camera myCamera = GetComponent<Camera>();
		if(myCamera != null){
			experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, gameObject.name + ",CAMERA_ENABLED," + myCamera.enabled);
		}
	}
}