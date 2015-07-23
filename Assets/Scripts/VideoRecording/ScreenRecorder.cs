using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScreenRecorder : MonoBehaviour {
	public InputField RecordingPathInputField;

	static bool isRecording;
	static string path = "";

	//Timing for continuous recording
	int framesPerSecond = 60; //want a constant framerate -- otherwise, frames that actually took longer won't render that way...
	float timeBetweenFrames;
	float timeCounter = 0;
	int numFrames = 0;

	public enum RecordingType{
		screenshot,
		continuousVideo
	}

	void Start(){
		timeBetweenFrames = 1.0f / (float)framesPerSecond;
		SetRecordPath ("");
	}

	public void SetIsRecording(bool shouldRecord){
		isRecording = shouldRecord;
	}

	public void SetRecordPath(string emptyString){ //the emptyString variable is a hack and not used because I'm having trouble getting the darn InputField to pass it's value to a function T_T
		if (RecordingPathInputField != null) {
 			path = RecordingPathInputField.text;
		} 
		else {
			Debug.Log("No input field to get recording path!");
		}
	}
	
	void FixedUpdate(){ //make recording independent of actual framerate
		if (isRecording && Application.loadedLevel == 1) { //only record in the main game scene. could change this if someone wished to record in other scenes.
			if(CheckTimeForScreenShot()){
				StartCoroutine(TakeScreenshot(RecordingType.continuousVideo));
			}
		}
	}

	void LateUpdate() {
		if (Input.GetKeyDown ("k")) { //arbitrary key...
			StartCoroutine (TakeScreenshot (RecordingType.screenshot));
		}
	}

	bool CheckTimeForScreenShot(){

		timeCounter += Time.deltaTime;

		if (timeCounter > timeBetweenFrames) {
			timeCounter = 0;
			return true;
		}

		return false;
	}

	public string ScreenShotName(int width, int height, RecordingType recordingType) {
		string name = "";

		if (recordingType == RecordingType.screenshot) {
			name = string.Format ("{0}/screenshots/screen_{1}x{2}_{3}.png", 
		                     Application.dataPath, 
		                     width, height, 
		                     System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss"));
		}
		else if (recordingType == RecordingType.continuousVideo) {
			name = string.Format ("{0}/screen_{1}x{2}_{3}.png", 
			                      path, //change to path variable? 
			                      width, height, 
			                      numFrames);

			numFrames++;
		}

		return name;
	}
	
	//despite waiting for the end of frame, this coroutine will be started every fixed update, resulting in an image for every fixedupdate call. which is good, because video will have a constant framerate, whereas the game will not.
	IEnumerator TakeScreenshot(RecordingType recordingType){
		// We should only read the screen buffer after rendering is complete

		yield return new WaitForEndOfFrame();
		
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		
		// Encode texture into PNG
		byte[] bytes = tex.EncodeToPNG();
		Destroy(tex);
		
		// For testing purposes, also write to a file in the project folder
		string filename = ScreenShotName(width, height, recordingType);
		System.IO.File.WriteAllBytes(filename, bytes);

		/*
		yield return new WaitForEndOfFrame ();

		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
		GetComponent<Camera>().targetTexture = rt;
		Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
		GetComponent<Camera>().Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		GetComponent<Camera>().targetTexture = null;
		RenderTexture.active = null; // JC: added to avoid errors
		Destroy(rt);
		byte[] bytes = screenShot.EncodeToPNG();
		string filename = ScreenShotName(resWidth, resHeight);
		System.IO.File.WriteAllBytes(filename, bytes);
		Debug.Log(string.Format("Took screenshot to: {0}", filename));
		takeHiResShot = false;
*/

		yield return null;
	}

}
