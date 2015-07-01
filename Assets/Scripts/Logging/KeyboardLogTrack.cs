using UnityEngine;
using System.Collections;

public class KeyboardLogTrack : MonoBehaviour, ILoggable {

	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

	public string[] Keys;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Log ();
	}

	public void Log(){
		LogKeyboard ();
	}

	void LogKeyboard(){
		string keyName = "";
		for (int i = 0; i < Keys.Length; i++) {
			keyName = Keys[i];
			if (Input.GetKey (keyName.ToLower())) {
				experimentLog.Log ("KEYDOWN " + keyName);
			}
		}
	}
}
