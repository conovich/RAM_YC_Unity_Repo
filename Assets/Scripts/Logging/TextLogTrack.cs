using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class TextLogTrack : MonoBehaviour, ILoggable {

	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

	Text myText;
	string currentText = "";

	bool firstLog = false; //should make an initial log

	// Use this for initialization
	void Start () {
		myText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(ExperimentSettings.isLogging && ( currentText != myText.text || !firstLog) ){ //if the text has changed, log it!
			firstLog = true;
			Log ();
		}
	}

	public void Log(){
		LogText();
	}

	void LogText(){
		currentText = myText.text;

		string textToLog = myText.text;

		if (myText.text == "") {
			textToLog = " "; //log a space -- makes it easier to read it during replay!
		}
		else {
			textToLog = textToLog.Replace (System.Environment.NewLine, "_NEWLINE_");
		}

		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), gameObject.name + ",TEXT," + textToLog );
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), gameObject.name + ",TEXT_COLOR," + myText.color.r + "," + myText.color.g + "," + myText.color.b + "," + myText.color.a);
	}
}