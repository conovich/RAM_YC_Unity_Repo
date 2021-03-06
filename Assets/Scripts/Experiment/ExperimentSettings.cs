using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExperimentSettings : MonoBehaviour { //should be in main menu AND experiment


	//public static string fileName; //log file name




	private static Subject _currentSubject;

	public static Subject currentSubject{ 
		get{ return _currentSubject; } 
		set{ 
			_currentSubject = value;
			//fileName = "TextFiles/" + _currentSubject.name + "Log.txt";
		}
	}

	public static bool isOculus = false;
	public static bool isJoystickInput = false;
	public static bool isReplay = false;
	public static bool isLogging = true; //if not in replay mode, should log things! or can be toggled off in main menu.

	public Toggle oculusToggle; //only exists in main menu -- make sure to null check
	public Toggle joystickInputToggle; //only exists in main menu -- make sure to null check
	public Toggle loggingToggle; //only exists in main menu -- make sure to null check

	public Text endCongratsText;
	public Text endScoreText;
	public Text endSessionText;


	//SINGLETON
	private static ExperimentSettings _instance;
	
	public static ExperimentSettings Instance{
		get{
			return _instance;
		}
	}
	
	void Awake(){
		
		if (_instance != null) {
			Debug.Log("Instance already exists!");
			Destroy(transform.gameObject);
			return;
		}
		_instance = this;
	}
	// Use this for initialization
	void Start () {
		SetOculus();
		if(Application.loadedLevelName == "EndMenu"){
			if(currentSubject != null){
				endCongratsText.text = "Congratulations " + currentSubject.name + "!";
				endScoreText.text = currentSubject.score.ToString();
				endSessionText.text = currentSubject.trials.ToString();
			}
			else{
				Debug.Log("Current subject is null!");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetReplayTrue(){
		isReplay = true;
		isLogging = false;
		loggingToggle.isOn = false;
	}

	public void SetReplayFalse(){
		isReplay = false;
		//shouldLog = true;
	}

	public void SetLogging(){
		if(isReplay){
			isLogging = false;
		}
		else{
			if(loggingToggle){
				isLogging = loggingToggle.isOn;
				Debug.Log("should log?: " + isLogging);
			}
		}

	}

	public void SetJoystickInput(){
		if (joystickInputToggle) {
			isJoystickInput = joystickInputToggle.isOn;
		}
	}

	public void SetOculus(){
		if(oculusToggle){
			isOculus = oculusToggle.isOn;
		}
	}
	
}
