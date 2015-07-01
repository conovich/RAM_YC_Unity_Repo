using UnityEngine;
using System.Collections;

public class MouseLogTrack : MonoBehaviour, ILoggable {

	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!ExperimentSettings.isOculus) {
			Log ();
		}
	}

	public void Log(){
		LogMouse ();
	}

	void LogMouse(){
		experimentLog.Log ("MOUSE POSITION " + Input.mousePosition.x + " " + Input.mousePosition.y);

		if(Input.GetMouseButtonDown(0)){
			Ray ray;
			RaycastHit hit;
			if(Camera.main != null){
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit)){
					experimentLog.Log("MOUSE CLICKED " + hit.collider.gameObject);
				}
				else{
					experimentLog.Log("MOUSE CLICKED " + "EMPTY");
				}
			}
			else{
				Debug.Log("Camera.main is null! Can't raycast mouse position.");
			}
		}
	}
}
