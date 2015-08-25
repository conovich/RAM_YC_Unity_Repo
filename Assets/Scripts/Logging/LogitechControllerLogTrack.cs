﻿using UnityEngine;
using System.Collections;


//LOG CUSTOM MESSAGES. should be accessed through some global class.
public class LogitechControllerLogTrack : MonoBehaviour {
	Logger_Threading experimentLog { get { return Experiment.Instance.log; } }

	int numButtons = 20;
	int numJoystickAxesUsed = 4; //DPAD, LEFT JOYSTICK

	// Use this for initialization
	void Start () {
		
	}
	
	void Update(){
		if(ExperimentSettings.isLogging){
			Log ();
		}
	}
	
	// LOGGING SHOULD BE INDEPENDENT OF FRAME RATE
	void FixedUpdate () {
		
	}
	
	public void Log(){ 
		LogJoystick ();
	}
	
	public void LogJoystick(){

		float[] joystickInput = GetLogitechControllerJoystickInput ();
		bool[] buttonInput = GetLogitechControllerButtonInput ();

		bool isJoystickInput = CheckAnyJoystickInput(joystickInput); //make sure there was actually joystick input before logging it!
		if (isJoystickInput) {
			for (int i = 0; i < numJoystickAxesUsed; i++) {
				experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount (), "Joystick" + i + "," + joystickInput [i]);
			}
		}

		bool isButtonInput = CheckAnyButtonInput(buttonInput); //make sure there was actually button input before logging it!
		if (isButtonInput) {
			for (int i = 0; i < numButtons; i++) {
				experimentLog.Log (Experiment.Instance.theGameClock.SystemTime_Milliseconds, experimentLog.GetFrameCount (), "Button" + i + "," + buttonInput [i]);
			}
		}
	}

	bool CheckAnyJoystickInput(float[] joystickInput){
		for(int i = 0; i < numJoystickAxesUsed; i++){
			if(joystickInput[i] != 0){
				return true;
			}
		}

		return false;
	}

	bool CheckAnyButtonInput(bool[] buttonInput){
		for (int i = 0; i < numButtons; i++) {
			if(buttonInput[i] == true){
				return true;
			}
		}

		return false;
	}

	bool[] GetLogitechControllerButtonInput(){
		bool[] buttonInput = new bool[numButtons];
		int button = 0;

		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton0);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton1);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton2);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton3);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton4);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton5);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton6);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton7);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton8);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton9);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton10);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton11);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton12);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton13);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton14);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton15);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton16);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton17);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton18);
		buttonInput[button++] = Input.GetKey(KeyCode.JoystickButton19);

		return buttonInput;
	}

	float[] GetLogitechControllerJoystickInput(){
		float[] axesInput = new float[numJoystickAxesUsed]; //will alternate: 
		int axis = 0;

		axesInput[axis++] = Input.GetAxis("Horizontal"); //DPAD X AXIS
		axesInput[axis++] = Input.GetAxis("Vertical"); //DPAD Y AXIS
		axesInput[axis++] = Input.GetAxis("HorizontalLeftJoystick"); //3rd AXIS
		axesInput[axis++] = Input.GetAxis("VerticalLeftJoystick"); //4th AXIS

		return axesInput;
	}

}