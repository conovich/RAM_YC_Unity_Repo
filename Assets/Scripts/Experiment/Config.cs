using UnityEngine;
using System.Collections;

public class Config : MonoBehaviour {

//GENERIC EXPERIMENT VARIABLES

	/*
	number of sessions
	heading offsets (min, max)
	object to use vs obj to use
	one object version? (true/false)
	autodrive to destination
	wait at object time
	drive time
	drive speed
	spin time
	visible during spin
	pause before spin time
	should maximize learning angle
	minimum degree between learning trials
	do massed objects
	randomize test order
	randomize learn order
	object buffer variables
	distance between various objects (wallbuffer)
	object buffer ( distance objects in a block must be from each other)
	avatar object buffer
	stimulation variables (default to not doing stimulation)
	doStim
	stimFreq
	stimDuration
	doBreak
	Test session variables
	doTestSession (not yet implemented???)
	Practice variables
	doPracticeBlock
	*/

	//session information
	//public static int numSessions;

	//stimulation variables
	public static bool shouldDoStim;	//TODO
	public static int stimFrequency;	//TODO
	public static float stimDuration;	//TODO
	public static bool shouldDoBreak;	//TODO
	
	//test session variables
	//doTestSession (not implemented in the panda3d version )
	
	//practice variables
	public static bool doPracticeBlock = true;
	
	//the following are set in INIT depending on isOneObjectVersion
	public static int numBlocks = 2; 				//each block is two items
	public static int numEasyLearningTrials;	//per item, trials with object visible the entire time //TODO
	public static int numHardLearningTrials;	//per item, trials with object initially visible	//TODO
	public static int numTestTrials = 4;			//per item, trials with object never visible
	
	//practice settings
	public static int numEasyLearningTrialsPract = 1;	//TODO
	public static int numHardLearningTrialsPract = 1;	//TODO
	public static int numTestTrialsPract = 2;





//SPECIFIC RAM-YC VARIABLES:

	public static bool isOneObjectVersion;

	//autodrive variables
	public static bool shouldAutodrive = true;	//TODO
	public static float pauseBeforeSpinTime = 2;	//TODO
	public static bool isVisibleDuringSpin = true;	//TODO
	public static bool do360Spin;	//TODO
	public static float spinTime = 1;
	public static float driveTime = 3;
	public static float driveSpeed = 22;
	public static float waitAtObjTime = 1;

	public static float avatarToObjectDistance = 0; //The distance you will start from the object will be determine by the driveSpeed and driveTime.

	//trial and object spawning variables
	public static bool shouldMaximizeLearningAngle = true;
	public static int minDegreeBetweenLearningTrials = 20;
	public static int maxDegreeBetweenLearningTrials = 50;
	//public static bool shouldDoMassedObjects;
	//public static bool shouldRandomizeTestOrder;
	//public static bool shouldRandomizeLearnOrder;
	public static float headingOffsetMin = 30; //offset from object
	public static float headingOffsetMax = 60; //offset from object

	//object buffer variables
	public static float bufferBetweenObjects = 20; // for each block
	public static float bufferBetweenObjectsAndWall = 20;
	public static float bufferBetweenObjectsAndAvatar = 20;

	void Awake(){
		DontDestroyOnLoad(transform.gameObject);

		float speedUnitsPerSecond = Config.driveSpeed; //GetComponent<Rigidbody>().velocity = transform.forward*verticalAxisInput*Config.driveSpeed
		avatarToObjectDistance = speedUnitsPerSecond * driveTime; // dX = v*t
		Debug.Log("AVATAR TO OBJ CONFIG DIST: " + avatarToObjectDistance);
	}

	void Start(){

	}

	public static int GetTotalNumTrials(){
		if (!doPracticeBlock) {
			return numTestTrials;
		} 
		else {
			return numTestTrials + numTestTrialsPract;
		}
	}

	public static void Init(){ //called in experiment.cs
		/*if (!isOneObjectVersion) {
			//TODO: set instructions here?
			numBlocks = 8; 				//each block is two items
			numEasyLearningTrials = 1;	//per item, trials with object visible the entire time
			numHardLearningTrials = 2;	//per item, trials with object initially visible
			numTestTrials = 3;			//per itme, trials with object never visible
			
			//practice settings
			doPracticeBlock = true;
			numEasyLearningTrialsPract = 1;
			numHardLearningTrialsPract = 1;
			numTestTrialsPract = 2;
			
		}
		else{
			if(do360Spin){
				//TODO: set instructions here?
			}
			else{
				
			}

			//objectToUse = 'useAll';
			numBlocks = 24;            // each block is two items
			numEasyLearningTrials = 0; // per item, trials with object visible entire time
			numHardLearningTrials = 2; // per item, trials with object initially visible
			numTestTrials = 1;         // per item, trials with object never visible
					
			// practice settings
			doPracticeBlock = true;
			numEasyLearningTrialsPract = 0;
			numHardLearningTrialsPract = 2;
			numTestTrialsPract = 1;
		}*/
	}

}
