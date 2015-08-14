using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class SessionController : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	bool isStimTrial = false; 

	// Use this for initialization
	void Start () {
	
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
		
		
			//execute the number of sessions
			int totalTrialCount = ExperimentSettings.currentSubject.session;
			Debug.Log ("starting session at: " + totalTrialCount);

			//run practice trials
			bool hasDonePractice = false;

			if(!hasDonePractice && Config.doPracticeBlock){
				int practiceCount = totalTrialCount;
				while( practiceCount < Config.numTestTrialsPract ){
					yield return StartCoroutine( RunTrial(false) );
					practiceCount++;
					Debug.Log("PRACTICE TRIALS COMPLETED: " + practiceCount);
				}
				hasDonePractice = true;
			}

			//run regular trials
			while (totalTrialCount < Config.numTestTrials + Config.numTestTrialsPract) {
				yield return StartCoroutine (RunTrial (isStimTrial));
				totalTrialCount++;
				Debug.Log("TRIALS COMPLETED: " + totalTrialCount);
			}
		}

		yield return 0;

	}

	//INDIVIDUAL SESSIONS -- implement for repeating the same thing over and over again
	//could also create other IEnumerators for other types of sessions
	public IEnumerator RunTrial(bool isStim){

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
		yield return StartCoroutine(exp.ShowSingleInstruction("You will now be driven to the " + newObjectName + " from another location.", true));

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

		//add point and increment subject session
		ExperimentSettings.currentSubject.AddScore(pointsReceived);
		ExperimentSettings.currentSubject.IncrementSession();

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
