using UnityEngine;
using System.Collections;

public class Experiment : MonoBehaviour {

	//configuration
	public Config config;

	//clock!
	public GameClock theGameClock;

	//instructions
	public InstructionsController instructionsController;
	//public InstructionsController inGameInstructionsController;
	public CameraController cameraController;

	//logging
	private string logfile;// = "Assets/TextFiles/testLog.txt"; //TODO: specify path with config file or text field or something!
	[HideInInspector] public Logger_Threading log;

	//session controller
	public SessionController sessionController;

	//score controller
	public ScoreController scoreController;

	//object controller
	public ObjectController objectController;

	//environment controller
	public EnvironmentController environmentController;

	//environment map visuals
	public EnvironmentMap environmentMap;

	//avatar
	public AvatarControls avatar;

	//public bool isOculus = false;


	//state enum
	public ExperimentState currentState = ExperimentState.instructionsState;

	public enum ExperimentState
	{
		instructionsState,
		inExperiment,
		inExperimentOver
	}

	//bools for whether we have started the state coroutines
	bool isRunningInstructions = false;
	bool isRunningExperiment = false;


	//EXPERIMENT IS A SINGLETON
	private static Experiment _instance;

	public static Experiment Instance{
		get{
			return _instance;
		}
	}

	void Awake(){
		if (_instance != null) {
			Debug.Log("Instance already exists!");
			return;
		}
		_instance = this;

		logfile = "Assets/TextFiles/" + ExperimentSettings.currentSubject.name + "Log.txt";

		log = GetComponent<Logger_Threading>();
		Logger_Threading.fileName = logfile;

	}

	// Use this for initialization
	void Start () {
		config.Init();
		//inGameInstructionsController.DisplayText("");
	}

	// Update is called once per frame
	void Update () {
		if(ExperimentSettings.currentSubject.session >= config.numSessions){

			StartCoroutine(RunOutOfSessions());

		}
		else{
			if (currentState == ExperimentState.instructionsState && !isRunningInstructions) {
				Debug.Log("running instructions");

				StartCoroutine(RunInstructions());

			}
			else if (currentState == ExperimentState.inExperiment && !isRunningExperiment) {
				Debug.Log("running experiment");
				StartCoroutine(BeginExperiment());
			}
		}
	}

	public IEnumerator RunOutOfSessions(){
		while(environmentMap.IsActive){
			yield return 0; //thus, should wait for the button press before ending the experiment
		}

		cameraController.SetInstructions(); //TODO: might be unecessary? evaluate for oculus...? 
		
		yield return StartCoroutine(ShowSingleInstruction("You have finished your sessions! \nPress the action button to proceed.", true));
		instructionsController.SetInstructionsColorful(); //want to keep a dark screen before transitioning to the end!
		instructionsController.DisplayText("...loading end screen...");
		EndExperiment();

		yield return 0;
	}

	public IEnumerator RunInstructions(){
		isRunningInstructions = true;

		cameraController.SetInstructions();

		instructionsController.RunInstructions ();

		while (!instructionsController.isFinished) { //wait until instructions parser has finished showing the instructions
			yield return 0;
		}

		currentState = ExperimentState.inExperiment;
		isRunningInstructions = false;

		yield return 0;

	}


	public IEnumerator BeginExperiment(){
		isRunningExperiment = true;
		
		cameraController.SetInGame();

		yield return StartCoroutine(sessionController.RunExperiment());

		//TODO: should take this out. check that player doesn't get stuck moving forward when experiment ends.
		/*while (true) {
			yield return 0;
		}*/
		
		yield return StartCoroutine(RunOutOfSessions()); //calls EndExperiment()

		yield return 0;

	}

	public void EndExperiment(){
		Debug.Log ("Experiment Over");
		currentState = ExperimentState.inExperimentOver;
		isRunningExperiment = false;
		
		SceneController.Instance.LoadEndMenu();
	}


	public IEnumerator ShowSingleInstruction(string line, bool isDark){
		if(isDark){
			instructionsController.SetInstructionsColorful();
		}
		else{
			instructionsController.SetInstructionsTransparentOverlay();
		}
		cameraController.SetInstructions();
		instructionsController.DisplayText(line);
		yield return StartCoroutine(WaitForActionButton());

		instructionsController.SetInstructionsTransparentOverlay();
		instructionsController.SetInstructionsBlank();
		cameraController.SetInGame();
	}
	
	public IEnumerator WaitForActionButton(){
		bool hasPressedButton = false;
		while(Input.GetAxis("ActionButton") != 0f){
			yield return 0;
		}
		while(!hasPressedButton){
			if(Input.GetAxis("ActionButton") == 1.0f){
				hasPressedButton = true;
			}
			yield return 0;
		}
	}
	

	public void OnExit(){ //call in scene controller when switching to another scene!
		log.close ();
	}


}
