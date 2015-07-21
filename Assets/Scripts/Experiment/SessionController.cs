using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class SessionController : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

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
			int sessionCount = ExperimentSettings.currentSubject.session;
			Debug.Log ("starting session at: " + sessionCount);
			while (sessionCount < exp.config.numSessions) {
				sessionCount++;
				yield return StartCoroutine (RunSession ());
			}
		}

		yield return 0;

	}

	//INDIVIDUAL SESSIONS -- implement for repeating the same thing over and over again
	//could also create other IEnumerators for other types of sessions
	public IEnumerator RunSession(){

		exp.avatar.RotateToEnvCenter(); //want object to spawn in a reasonable location. for cases such as avatar facing a corner.

		GameObject newObject = exp.objectController.SpawnRandomObject();
		string newObjectName = newObject.name;

		//Debug.Log("orig name: " + name);
		newObjectName = Regex.Replace( newObjectName, "(Clone)", "" );
		newObjectName = Regex.Replace( newObjectName, "[()]", "" );
		
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
		yield return new WaitForSeconds (1.0f); //wait briefly before driving to object
		yield return exp.avatar.StartCoroutine(exp.avatar.MoveToTargetObject(newObject));




		//HIDE OBJECT

		//turn off visuals of object
		if(newObject.GetComponent<Renderer>() != null){
			newObject.GetComponent<Renderer>().enabled = false;
		}
		Renderer[] newObjectRenderers = newObject.GetComponentsInChildren<Renderer>();
		for(int i = 0; i < newObjectRenderers.Length; i++){
			newObjectRenderers[i].enabled = false;
		}
		
		
		//turn off all colliders of an object
		if(newObject.GetComponent<Collider>() != null){
			newObject.GetComponent<Collider>().enabled = false;
		}
		Collider[] newObjectColliders = newObject.GetComponentsInChildren<Collider>();
		for(int i = 0; i < newObjectColliders.Length; i++){
			newObjectColliders[i].enabled = false;
		}

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
