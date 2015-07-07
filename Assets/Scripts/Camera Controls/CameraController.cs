using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	//Experiment exp { get { return Experiment.Instance; } }
	//ExperimentSettings expSettings { get { return ExperimentSettings.Instance; } }
	
	public GameObject AvatarStandardCameraRig;
	public GameObject OculusRig;

	public Transform AvatarOculusParent;
	public Transform InstructionsOculusParent;


	float oculusOrthoSize = 10;
	bool oculusInstructionsIsOrtho = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void SetInstructions(){
		Debug.Log("hallo instructions cameras");

		TurnOffAllCameras();

		if(ExperimentSettings.isOculus){
			//OculusRig.transform.position = InstructionsOculusParent.transform.position;
			//OculusRig.transform.parent = InstructionsOculusParent;

			//if(OculusInstructionsIsOrtho){
			//	SetOculusOrtho();
			//}
		}
		else{
			EnableCameras(AvatarStandardCameraRig, true);
		}
	}

	public void SetInGame(){

		Debug.Log("oh hey in game cameras");
		TurnOffAllCameras();

		if(ExperimentSettings.isOculus){
			//OculusRig.transform.position = AvatarOculusParent.transform.position;
			//OculusRig.transform.parent = AvatarOculusParent;

			SetOculusPerspective();
		}
		else{
			EnableCameras(AvatarStandardCameraRig, true);
		}


	}

	void TurnOffAllCameras(){
		EnableCameras(AvatarStandardCameraRig, false);

		if(!ExperimentSettings.isOculus){
			OculusRig.SetActive(false);
		}
	}

	void EnableCameras(GameObject cameraRig, bool setOn){
		Camera[] cameras = cameraRig.GetComponentsInChildren<Camera>();
		for(int i = 0; i < cameras.Length; i++){
			cameras[i].enabled = setOn;
		}
	}

	void SetOculusOrtho(){
		Camera[] cameras = OculusRig.GetComponentsInChildren<Camera>();
		for(int i = 0; i < cameras.Length; i++){
			cameras[i].orthographic = true;
			cameras[i].orthographicSize = oculusOrthoSize;

		}
	}

	void SetOculusPerspective(){
		Camera[] cameras = OculusRig.GetComponentsInChildren<Camera>();
		for(int i = 0; i < cameras.Length; i++){
			cameras[i].orthographic = false;
			//cameras[i].clearFlags = CameraClearFlags.Skybox;
		}
	}
}
