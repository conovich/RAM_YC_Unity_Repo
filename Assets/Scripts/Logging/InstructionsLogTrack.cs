using UnityEngine;
using System.Collections;

public class InstructionsLogTrack : MonoBehaviour, ILoggable {

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
		LogInstructions();
	}

	void LogInstructions(){

	}
}
