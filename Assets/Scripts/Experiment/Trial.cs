using UnityEngine;
using System.Collections;

//FOR USE IN TRIALCONTROLLER
public class Trial {
	Experiment exp { get { return Experiment.Instance; } }
	
	public Vector3 objectPosition;	//object position stays the same throughout the trial
	public Vector3 avatarPosition001;	//learning 1
	public Vector3 avatarPosition002;	//learning 2
	public Vector3 avatarPosition003;	//testing
	public Quaternion avatarRotation001;	//learning 1
	public Quaternion avatarRotation002;	//learning 2
	public Quaternion avatarRotation003;	//testing
	
	public Trial(){
		
	}
	
	//get reflected rotation
	public Quaternion GetReflectedRotation(Quaternion rot){
		Vector3 newRot = rot.eulerAngles;
		newRot = new Vector3(newRot.x, newRot.y + 180, newRot.z);
		return Quaternion.Euler(newRot);
	}
	
	//get reflected position from the environment controller!
	//Given the center of the environment, calculate a reflected position in the environment
	public Vector3 GetReflectedPositionXZ(Vector3 pos){
		
		Vector3 envCenter = exp.environmentController.GetEnvironmentCenter ();
		
		Vector3 distanceFromCenter = pos - envCenter;
		
		float reflectedPosX = envCenter.x - distanceFromCenter.x;
		float reflectedPosZ = envCenter.z - distanceFromCenter.z;
		
		Vector3 reflectedPos = new Vector3 ( reflectedPosX, pos.y, reflectedPosZ );
		
		return reflectedPos;
	}
	
	public Trial CounterSelf(){
		Trial counterTrial = new Trial ();
		
		counterTrial.objectPosition = GetReflectedPositionXZ(objectPosition);
		
		counterTrial.avatarPosition001 = GetReflectedPositionXZ (avatarPosition001);
		counterTrial.avatarPosition002 = GetReflectedPositionXZ (avatarPosition002);
		counterTrial.avatarPosition003 = GetReflectedPositionXZ (avatarPosition003);
		
		counterTrial.avatarRotation001 = GetReflectedRotation (avatarRotation001);
		counterTrial.avatarRotation002 = GetReflectedRotation (avatarRotation002);
		counterTrial.avatarRotation003 = GetReflectedRotation (avatarRotation003);
		
		return counterTrial;
	}
}