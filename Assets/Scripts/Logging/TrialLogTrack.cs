using UnityEngine;
using System.Collections;

public class TrialLogTrack : MonoBehaviour, ILoggable {

	Logger_Threading experimentLog { get { return Experiment.Instance.subjectLog; } }
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Log(){ //well... the interface template isn't working so well for this right now. might have to refactor later.

	}

	public void Log(int trialNumber, bool isStim){
		if (ExperimentSettings.isLogging) {
			LogTrial (trialNumber, isStim);
		}
	}
	
	void LogTrial(int trialNumber, bool isStim){
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), "Trial Info" + ",NUM_TRIALS," + trialNumber + ",IS_STIM," + isStim);
	}
}
