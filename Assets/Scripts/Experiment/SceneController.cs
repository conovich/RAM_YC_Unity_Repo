using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour { //there can be a separate scene controller in each scene


	//SINGLETON
	private static SceneController _instance;
	
	public static SceneController Instance{
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

	}

	void GetShortcutInput(){
		if(Input.GetKeyDown(KeyCode.P)){
			LoadMainMenu();
		}
		else if (Input.GetKeyDown(KeyCode.Q)){
			LoadExperiment();
		}
		else if (Input.GetKeyDown(KeyCode.E)){
			LoadEndMenu();
		}
	}

	// Update is called once per frame
	void Update () {
		GetShortcutInput();
	}

	public void LoadMainMenu(){
		if(Experiment.Instance != null){
			Experiment.Instance.OnExit();
		}

		Debug.Log("loading main menu!");
		SubjectReaderWriter.Instance.RecordSubjects();
		Application.LoadLevel(0);
	}

	public void LoadExperiment(){
		//should be no new data to record for the subject
		if(Experiment.Instance != null){
			Experiment.Instance.OnExit();
		}

		if (ExperimentSettings.currentSubject != null || ExperimentSettings.isReplay) {
			if(ExperimentSettings.currentSubject.trials < Config.GetTotalNumTrials()){
				Debug.Log ("loading experiment!");
				Application.LoadLevel (1);
			}
			else{
				Debug.Log ("Subject has already finished all blocks! Loading end menu.");
				Application.LoadLevel (2);
			}
		}
	}

	public void LoadEndMenu(){
		if(Experiment.Instance != null){
			Experiment.Instance.OnExit();
		}

		SubjectReaderWriter.Instance.RecordSubjects();
		Debug.Log("loading end menu!");
		Application.LoadLevel(2);
	}

	public void Quit(){
		SubjectReaderWriter.Instance.RecordSubjects();
		Application.Quit();
	}

	void OnApplicationQuit(){
		Debug.Log("On Application Quit!");
		SubjectReaderWriter.Instance.RecordSubjects();
	}
}
