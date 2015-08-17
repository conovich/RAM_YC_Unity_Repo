using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TrialController : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	bool isPracticeTrial = false;
	bool isStimTrial  = false;
	int numRealTrials = 0; //used for logging ID's

	//contains strings -- "regular trial" , "counterbalanced trial"
	//a "counterbalanced trial" is added when a regular trial finishes, as it can now be chosen.
	//a trial type is chosen from this list at random, then removed from the list.
	List<string> TrialTypes;
	string regTrialString = "regular trial";
	string counterTrialString = "counterbalanced trial";

	//keeps track of the trials that must be counterbalanced.
	//chosen from at random when a "counterbalanced trial" is chosen from the TialTypes list.
	List<Trial> ListOfCounterTrials;



	// Use this for initialization
	void Start () {
		InitTrialTypes ();
		ListOfCounterTrials = new List<Trial> ();
	}
	
	void InitTrialTypes(){
		TrialTypes = new List<string> ();
		for (int i = 0; i < Config.numTestTrials/2; i++) { //there should always be an even number of test trials, as each block is a pair of trials - trial & counter trial
			TrialTypes.Add(regTrialString);
		}
	}

	void AddCounterTrial(Trial regTrial){
		Trial counterTrial = regTrial.CounterSelf ();
		ListOfCounterTrials.Add (counterTrial);
		TrialTypes.Add (counterTrialString);
	}

	string PickAndRemoveTrialType(){
		int randomIndex = Random.Range (0, TrialTypes.Count);
		
		string trialType = TrialTypes [randomIndex];
		TrialTypes.RemoveAt (randomIndex);
		
		return trialType;
	}

	Trial PickAndRemoveCounterTrial(){
		int randomIndex = Random.Range (0, ListOfCounterTrials.Count);
		Trial counterTrial = ListOfCounterTrials [randomIndex];
		ListOfCounterTrials.RemoveAt (randomIndex);

		return counterTrial;
	}
	
	
	Trial GenerateNewTrial(){
		
		Trial newTrial = new Trial ();
		
		newTrial.avatarPosition001 = exp.avatar.SetRandomLocationXZ();
		exp.avatar.RotateToEnvCenter(); //want object to spawn in a reasonable location. for cases such as avatar facing a corner.

		newTrial.objectPosition = exp.objectController.GenerateRandomObjectLocation ();
		newTrial.avatarRotation001 = exp.avatar.SetRandomRotationY();

		newTrial.avatarPosition002 = exp.avatar.SetRandomLocationXZ();
		newTrial.avatarRotation002 = exp.avatar.SetRandomRotationY();

		newTrial.avatarPosition003 = exp.avatar.SetRandomLocationXZ();
		newTrial.avatarRotation003 = exp.avatar.SetRandomRotationY();
		
		return newTrial;
	}

	//FILL THIS IN DEPENDING ON EXPERIMENT SPECIFICATIONS
	public IEnumerator RunExperiment(){
		if (!ExperimentSettings.isReplay) {
			//show instructions for exploring
			yield return StartCoroutine (exp.ShowSingleInstruction ("Use the arrow keys to explore the environment. When finished exploring, press the button.", true));
		
			//let player explore
			yield return StartCoroutine (exp.WaitForActionButton ());
		
		
			//get the number of blocks so far -- floor half the number of trials recorded
			int totalTrialCount = ExperimentSettings.currentSubject.trials;
			numRealTrials = totalTrialCount;
			if(Config.doPracticeBlock){
				if(numRealTrials >= 2){ //otherwise, leave numRealTrials at zero.
					numRealTrials -= Config.numTestTrialsPract;
				}
			}

			Debug.Log ("starting at trial " + totalTrialCount);

			//run practice trials
			isPracticeTrial = true;

			if(isPracticeTrial && Config.doPracticeBlock){
				int practiceCount = totalTrialCount;
				while( practiceCount < Config.numTestTrialsPract ){
					Trial newTrial = GenerateNewTrial();
					yield return StartCoroutine( RunTrial( false, newTrial ) );
					practiceCount++;
					Debug.Log("PRACTICE TRIALS COMPLETED: " + practiceCount);
					totalTrialCount++;
				}
				isPracticeTrial = false;
				ExperimentSettings.currentSubject.IncrementTrial();
			}

			//run regular trials
			int maxNumTrials = Config.GetTotalNumTrials();
			while (totalTrialCount < maxNumTrials) {
				//TODO: pick a trial type
				string nextTrialType = PickAndRemoveTrialType();

				Debug.Log("NEXT TRIAL TYPE: " + nextTrialType);
				if(nextTrialType == regTrialString){
					Trial newTrial = GenerateNewTrial();

					yield return StartCoroutine(RunTrial ( false, newTrial ) );

					AddCounterTrial(newTrial);
				}
				else if(nextTrialType == counterTrialString){
					Trial counterTrial = PickAndRemoveCounterTrial();
 					yield return StartCoroutine(RunTrial( true, counterTrial ) ); //counterbalanced trials should have stim
				}

				totalTrialCount++;
				ExperimentSettings.currentSubject.IncrementTrial();
				Debug.Log("TRIALS COMPLETED: " + totalTrialCount);
			}
		}

		yield return 0;

	}

	//INDIVIDUAL TRIALS -- implement for repeating the same thing over and over again
	//could also create other IEnumerators for other types of trials
	IEnumerator RunTrial(bool isStim, Trial trial){

		if (isPracticeTrial) {
			GetComponent<TrialLogTrack> ().Log (-1, isStim);
			Debug.Log("Logged practice trial.");
		} 
		else {
			GetComponent<TrialLogTrack> ().Log (numRealTrials, isStim);
			numRealTrials++;
			Debug.Log("Logged trial #: " + numRealTrials);
		}

		Debug.Log ("IS STIM: " + isStim);

		//move player to random location & rotation
		//exp.avatar.SetRandomLocationXZ();
		//exp.avatar.RotateToEnvCenter(); //want object to spawn in a reasonable location. for cases such as avatar facing a corner.
		exp.avatar.transform.position = trial.avatarPosition001;
		exp.avatar.transform.rotation = trial.avatarRotation001;


		//GameObject newObject = exp.objectController.SpawnRandomObjectXY();
		GameObject newObject = exp.objectController.SpawnRandomObjectXY (trial.objectPosition);
		string newObjectName = newObject.GetComponent<SpawnableObject>().GetName();

		//exp.avatar.SetRandomRotationY();
		
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
		//exp.avatar.SetRandomLocationXZ();
		exp.avatar.transform.position = trial.avatarPosition002;
		//exp.avatar.RotateToEnvCenter();
		exp.avatar.transform.rotation = trial.avatarRotation002;
		yield return new WaitForSeconds (Config.waitAtObjTime); //wait briefly before driving to object
		yield return exp.avatar.StartCoroutine(exp.avatar.MoveToTargetObject(newObject));

		//yield return StartCoroutine (DriveAvatar (newObject));



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
		//exp.avatar.SetRandomLocationXZ();
		exp.avatar.transform.position = trial.avatarPosition003;
		//exp.avatar.SetRandomRotationY();
		exp.avatar.transform.rotation = trial.avatarRotation003;

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
		
	}
}
