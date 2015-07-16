using UnityEngine;
using System.Collections;

public class MouseLogTrack : MonoBehaviour, ILoggable {

	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!ExperimentSettings.isOculus && ExperimentSettings.shouldLog) {
			Log ();
		}
	}

	public void Log(){
		if (!ExperimentSettings.isReplay) {
			LogMouse ();
		}
	}

	void LogMouse(){
		//log the position
		//TODO: do a check if the mouse position is out of range.
		experimentLog.Log ("Mouse POSITION " + Input.mousePosition.x + " " + Input.mousePosition.y);

		//log a clicked object
		if(Input.GetMouseButtonDown(0)){
			Ray ray;
			RaycastHit hit;
			if(Camera.main != null){
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit)){
					experimentLog.Log("Mouse CLICKED " + hit.collider.gameObject);
				}
				else{
					experimentLog.Log("Mouse CLICKED " + "EMPTY");
				}
			}
			else{
				Debug.Log("Camera.main is null! Can't raycast mouse position.");
			}
		}
	}
}
