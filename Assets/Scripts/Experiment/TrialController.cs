using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TrialController : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	bool isStimTrial  = false;
	int numRealTrials = 0;


	List<Trial> ListOfTrialsToBeCounterbalanced;

	private class Trial{
		public Vector3 objectPosition;	//object position stays the same throughout the trial
		public Vector3 avatarPosition001;	//learning 1
		public Vector3 avatarPosition002;	//learning 2
		public Vector3 avatarPosition003;	//testing
		public Quaternion avatarRotation001;	//learning 1
		public Quaternion avatarRotation002;	//learning 2
		public Quaternion avatarRotation003;	//testing

		public Trial(){

		}

		//THE ENVIRONMENT MUST BE CENTERED AT 0,0,0 FOR THIS TO WORK
		public Vector3 GetReflectedPosition(Vector3 pos){
			if (exp.environmentController.transform.position != Vector3.zero) {
				Debug.Log("Environment is not centered! Reflected position may be incorrect.");
			}
			return new Vector3 (-pos.x, pos.y, -pos.z);
		}

		public Quaternion GetReflectedRotation(Quaternion rot){
			Vector3 newRot = rot.eulerAngles;
			newRot = new Vector3(newRot.x, newRot.y + 180, newRot.z);
			return Quaternion.Euler(newRot);
		}
	}



	// Use this for initialization
	void Start () {
		ListOfTrialsToBeCounterbalanced = new List<Trial> ();
	}
	

	// Update is called once per frame
	void Update () {
	
	}

	//FILL THIS IN DEPENDING ON EXPERIMENT SPECIFICATIONS
	public IEnumerator RunExperiment(){
		if (!ExperimentSettings.isReplay) {
			//show instructions for exploring
			yield return StartCoroutine (exp.ShowSingleInstruction ("Use the arrow keys to explore the environment. When finished exploring, press the button.", true));
		
			//let player explore
			yield return StartCoroutine (exp.WaitForActionButton ());
		
		
			//get the number of blocks so far -- floor half the number of trials recorded
			int totalBlockCount = ExperimentSettings.currentSubject.blocks;
			numRealTrials = ExperimentSettings.currentSubject.blocks*2;
			if(Config.doPracticeBlock){
				if(numRealTrials >= 2){ //otherwise, leave numRealTrials at zero.
					numRealTrials -= Config.numTestTrialsPract;
				}
			}

			Debug.Log ("starting at block " + totalBlockCount);

			//run practice trials
			bool isPractice = true;

			if(isPractice && Config.doPracticeBlock){
				int practiceCount = totalBlockCount;
				while( practiceCount < Config.numTestTrialsPract ){
					yield return StartCoroutine( RunTrial( false, isPractice ) );
					practiceCount++;
					Debug.Log("PRACTICE TRIALS COMPLETED: " + practiceCount);
				}
				isPractice = false;
				totalBlockCount++;
				ExperimentSettings.currentSubject.IncrementBlock();
			}

			//run regular trials
			int maxNumTrials = Config.GetTotalNumBlocks();
			while (totalBlockCount < maxNumTrials) {
				yield return StartCoroutine (RunTrial (false , isPractice));
				yield return StartCoroutine (RunTrial (true , isPractice));	//counterbalanced stim block TODO: counterbalance!
				totalBlockCount++;
				ExperimentSettings.currentSubject.IncrementBlock();
				Debug.Log("BLOCKS COMPLETED: " + totalBlockCount);
			}
		}

		yield return 0;

	}

	//INDIVIDUAL TRIALS -- implement for repeating the same thing over and over again
	//could also create other IEnumerators for other types of trials
	public IEnumerator RunTrial(bool isStim, bool isPractice){
		if (isPractice) {
			GetComponent<TrialLogTrack> ().Log (-1, isStim);
			Debug.Log("Logged practice trial.");
		} 
		else {
			GetComponent<TrialLogTrack> ().Log (numRealTrials, isStim);
			numRealTrials++;
			Debug.Log("Logged trial #: " + numRealTrials);
		}

		Debug.Log ("IS STIM: " + isStim);

		exp.avatar.RotateToEnvCenter(); //want object to spawn in a reasonable location. for cases such as avatar facing a corner.

		GameObject newObject = exp.objectController.SpawnRandomObject();
		string newObjectName = newObject.GetComponent<SpawnableObject>().GetName();
		
		//show instruction for "press the button to be driven to the OBJECT_NAME".
		yield return StartCoroutine(exp.ShowSingleInstruction("Press the button to be driven to the " + newObjectName + ".", true));

		//override player input and drive the player to the object
		exp.avatar.ShouldLockControls = true;
		yield return exp.avatar.StartCoroutine(exp.avatar.MoveToTargetObject(newObject));
		
		//show instruction for "you will now be driven to the OBJECT_NAME from another location.
		yield return StartCoroutine(exp.ShowSingleInstruction("Press the button to be driven to the " + newObjectName + 
		                                                      "\n from another location.", true));

		//override player input (already done above)
		//move player to random location
		//drive player to object
		exp.avatar.SetRandomLocation();
		exp.avatar.RotateToEnvCenter();
		yield return new WaitForSeconds (Config.waitAtObjTime); //wait briefly before driving to object
		yield return exp.avatar.StartCoroutine(exp.avatar.MoveToTargetObject(newObject));




		//HIDE OBJECT

		//turn off visuals of object
		SpawnableObject newSpawnedObject = newObject.GetComponent<SpawnableObject> ();
		newSpawnedObject.TurnVisible (false); //important function to turn off the object without setting it inactive -- because we want to keep logging on

		//show instruction for "the OBJECT_NAME is now hidden. you will now drive to the OBJECT_NAME on your own."
		//+"Press the button to continue, and then drive to the locaiton of the cactus and press the button when you are in the correct location."
		yield return StartCoroutine(exp.ShowSingleInstruction("The " + newObjectName + " is now hidden. " +
														"\nYou will now drive to the " + newObjectName + " on your own." +
		                                                "\nPress the button to continue, and then drive to the location of the "+ newObjectName +
		                                                " and press the button when you are in the correct location.", true));
		
		//show black text across top of screen: "press the button at the location of the OBJECT_NAME"
		//exp.inGameInstructionsController.DisplayText("press the button at the location of the " + newObjectName);
		exp.instructionsController.DisplayText("press the button at the location of the " + newObjectName);
		
		//move player to random location & rotation
		exp.avatar.SetRandomLocation();
		exp.avatar.SetRandomRotation();

		//enable player movement
		//wait for player to press the button, then move on
		exp.avatar.ShouldLockControls = false;
		yield return StartCoroutine(exp.WaitForActionButton());
		//exp.inGameInstructionsController.DisplayText("");


		//show overhead view of player's position vs. desired object position
		//with text: "Nice job. You earned X points. Press the button to continue."
		//"Response Location" -- in color of response location icon

		exp.environmentMap.SetAvatarVisualPosition(exp.avatar.transform.position);
		exp.environmentMap.SetObjectVisualPosition(newObject.transform.position);
		exp.environmentMap.TurnOn();

		//calculate points
		int pointsReceived = exp.scoreController.CalculatePoints(newObject);

		//add points
		ExperimentSettings.currentSubject.AddScore(pointsReceived);

		//show aforementioned text
		yield return StartCoroutine(exp.ShowSingleInstruction("You received " + pointsReceived.ToString() + " points! \n Score: " + exp.scoreController.score,false));

		//turn off the environment map
		exp.environmentMap.TurnOff();
		
		GameObject.Destroy(newObject);
		
		//move player to random location & rotation - for next iteration
		exp.avatar.SetRandomLocation();
		exp.avatar.SetRandomRotation();
		
	}
}
