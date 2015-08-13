using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UIPanelLogTrack : MonoBehaviour, ILoggable {
	
	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

	Image myPanelImage;
	Color currentPanelColor;

	bool firstLog = false; //should do an initial log
	

	// Use this for initialization
	void Start () {
		myPanelImage = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(ExperimentSettings.shouldLog && (currentPanelColor != myPanelImage.color || !firstLog) ){ //if the color has changed, or it's the first log
			firstLog = true;
			Log ();
		}
	}
	
	public void Log(){
		LogPanel();
	}
	
	void LogPanel(){
		currentPanelColor = myPanelImage.color;
		experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount(), gameObject.name 
		                 + ",PANEL," + myPanelImage.color.r + "," + myPanelImage.color.g + "," + myPanelImage.color.b + "," + myPanelImage.color.a);
	}
}
