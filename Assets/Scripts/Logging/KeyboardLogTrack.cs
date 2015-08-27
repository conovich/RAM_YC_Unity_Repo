using UnityEngine;
using System.Collections;

public class KeyboardLogTrack : MonoBehaviour, ILoggable {

	Logger_Threading experimentLog { get { return Experiment.Instance.subjectLog; } }

	public string[] Keys;

	// Use this for initialization
	void Start () {
	
	}

	void Update(){
		if(ExperimentSettings.isLogging){
			Log ();
		}
	}

	// LOGGING SHOULD BE INDEPENDENT OF FRAME RATE
	void FixedUpdate () {

	}

	public void Log(){
		LogKeyboard ();
	}

	void LogKeyboard(){
		string keyName = "";
		for (int i = 0; i < Keys.Length; i++) {
			keyName = Keys[i];
			if (Input.GetKey (keyName.ToLower())) {
				experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), "Keyboard," + keyName);
			}
		}
	}
}
