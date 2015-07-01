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
	public int numSessions;

	//stimulation variables
	public bool shouldDoStim;
	public int stimFrequency;
	public float stimDuration;
	public bool shouldDoBreak;
	
	//test session variables
	//doTestSession (not implemented in the panda3d version )
	
	//practice variables
	public bool doPracticeBlock = true;
	
	//the following are set in INIT depending on isOneObjectVersion
	int numBlocks; 				//each block is two items
	int numEasyLearningTrials;	//per item, trials with object visible the entire time
	int numHardLearningTrials;	//per item, trials with object initially visible
	int numTestTrials;			//per itme, trials with object never visible
	
	//practice settings
	int numEasyLearningTrialsPract = 1;
	int numHardLearningTrialsPract = 1;
	int numTestTrialsPract = 2;





//SPECIFIC RAM-YC VARIABLES:

	public bool isOneObjectVersion;

	//autodrive variables
	public bool shouldAutodrive;
	public float waitAtObjTime = 1;
	public float driveTime = 3;
	public float driveSpeed = 6;
	public bool do360Spin;
	public float spinTime = 1;
	public bool isVisibleDuringSpin = true;
	public float pauseBeforeSpinTime = 2;

	//trial and object spawning variables
	public bool shouldMaximizeLearningAngle;
	public int minDegreeBetweenLearningTrials;
	public int maxDegreeBetweenLearningTrials;
	public bool shouldDoMassedObjects;
	public bool shouldRandomizeTestOrder;
	public bool shouldRandomizeLearnOrder;
	public float headingOffsetMin = 20;
	public float headingOffsetMax = 30;

	//object buffer variables
	public float bufferBetweenObjects;
	public float bufferBetweenObjectsAndWall;
	public float bufferBetweenObjectsAndAvatar;


	void Start(){

	}

	public void Init(){ //called in experiment.cs
		if (!isOneObjectVersion) {
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
		}
	}

}
