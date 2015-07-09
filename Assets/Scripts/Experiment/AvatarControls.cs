using UnityEngine;
using System.Collections;

public class AvatarControls : MonoBehaviour{

	Experiment exp  { get { return Experiment.Instance; } }

	public bool ShouldLockControls = false;


	float RotationSpeed = 1;


	GameObject collisionObject;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (exp.currentState == Experiment.ExperimentState.inExperiment) {
			if(!ShouldLockControls){
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY; // TODO: on collision, don't allow a change in angular velocity?
				
				GetInput ();
			}
			else{
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}

	void FixedUpdate(){

	}

	void GetInput()
	{
		float verticalAxisInput = Input.GetAxis ("Vertical");

		if ( Mathf.Abs(verticalAxisInput) > 0.03f) //for any hardware calibration errors
		{
			GetComponent<Rigidbody>().velocity = transform.forward*verticalAxisInput*exp.config.driveSpeed; //should have no deltaTime framerate component -- given the frame, you should always be moving at a speed directly based on the input
																											//NOTE: potential problem with this method: joysticks and keyboard input will have different acceleration calibration.

		}
		else{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}


		float horizontalAxisInput = Input.GetAxis ("Horizontal");

		if (Mathf.Abs (horizontalAxisInput) > 0.0f) { //for any hardware calibration errors

			//Turn( horizontalAxisInput*RotationSpeed*(Time.deltaTime) ); 
			GetComponent<Rigidbody> ().angularVelocity = Vector3.up * horizontalAxisInput * RotationSpeed;
			Debug.Log("horizontal axis ANG VEL = " + GetComponent<Rigidbody>().angularVelocity);
		}
		else {
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero * horizontalAxisInput * RotationSpeed;
		}

	}

	void Move( float amount ){
		transform.position += transform.forward * amount;
	}
	
	void Turn( float amount ){
		transform.RotateAround (transform.position, Vector3.up, amount );
	}
	
	void OnCollisionEnter(Collision collision){ //happens before the update loop and before the coroutine loop
		collisionObject = collision.gameObject;
		Debug.Log (collision.gameObject.name);
	}

	public IEnumerator MoveToTargetObject(GameObject target){

		yield return new WaitForSeconds(exp.config.pauseBeforeSpinTime);

		collisionObject = null;

		Quaternion origRotation = transform.rotation;
		Vector3 targetPosition = new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z);
		transform.LookAt(targetPosition);
		Quaternion desiredRotation = transform.rotation;


		//rotate to look at target
		transform.rotation = origRotation;
		//degrees left when avatar will start moving forward while completing the turn
		float degreesShouldMoveForward = 8.0f;
		//SLERP THE ROTATION?
		/*while (Mathf.Abs(transform.rotation.eulerAngles.y - desiredRotation.eulerAngles.y) > degreesShouldMoveForward){ 
			transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 2f*Time.deltaTime);
			yield return 0;
		}*/

		//Debug.Log("euler angles: " + (transform.rotation.eulerAngles.y - desiredRotation.eulerAngles.y));

		float rotationAngleDifference = transform.rotation.eulerAngles.y - desiredRotation.eulerAngles.y;

		int oneDegree = 1;
		while (Mathf.Abs(transform.rotation.eulerAngles.y - desiredRotation.eulerAngles.y) >= oneDegree){
			//transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, RotationSpeed*Time.deltaTime);

			//make sure to take the shorter rotation
			if((rotationAngleDifference > 0 && rotationAngleDifference < 180) || (rotationAngleDifference > -360 && rotationAngleDifference < -180)){
				Turn (-oneDegree);
			}
			else{
				Turn (oneDegree);
			}
			yield return 0;
		}
		transform.rotation = desiredRotation;


		//move to desired location
		Vector3 desiredPosition = new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z);
		Vector3 origPosition = transform.position;
		//float distance = (desiredPosition - transform.position).magnitude;
		//float invDistance = 1.0f/distance; //multiply by drive time so that the further you are from an object, the longer it takes to get there


		float moveRate = 1.0f / exp.config.driveTime;
		float tElapsed = 0.0f;
		//stop when you have collided with something
		while(collisionObject == null){
			//IF LERPING...
			//move forward!
			//transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime*invDistance*exp.config.driveSpeed);
			/*//finish the rotation while moving forward -- if SLERPING THE ROTATION
			transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 2f*Time.deltaTime);*/

			tElapsed += (Time.deltaTime * moveRate);

			//will linearly interpolate the position for config.driveTime seconds
			transform.position = Vector3.Lerp(origPosition, desiredPosition, tElapsed);
			

			yield return 0;
		}
		GetComponent<Rigidbody>().velocity = Vector3.zero;

		yield return new WaitForSeconds(exp.config.pauseBeforeSpinTime);

	}

	//only in x & z coordinates
	public void SetRandomLocation(){
		//based on the wall bounds, pick a location

		float wallBuffer = exp.config.bufferBetweenObjectsAndWall;

		float randomXPos = Random.Range(exp.environmentController.WallsXPos.position.x - wallBuffer, exp.environmentController.WallsXNeg.position.x + wallBuffer);
		float randomZPos = Random.Range(exp.environmentController.WallsZPos.position.z - wallBuffer, exp.environmentController.WallsZNeg.position.z + wallBuffer);

		Vector3 newPosition = new Vector3 (randomXPos, transform.position.y, randomZPos);


		if(randomXPos > exp.environmentController.WallsXPos.position.x || randomXPos < exp.environmentController.WallsXNeg.position.x){
			Debug.Log("avatar out of bounds in x!");
		}
		else if(randomZPos > exp.environmentController.WallsZPos.position.z || randomZPos < exp.environmentController.WallsZNeg.position.z){
			Debug.Log("avatar out of bounds in z!");
		}




		transform.position = newPosition;
	}

	//only in y axis
	public void SetRandomRotation(){
		float randomYRotation = Random.Range(exp.config.minDegreeBetweenLearningTrials, exp.config.maxDegreeBetweenLearningTrials);
		transform.RotateAround(transform.position, Vector3.up, randomYRotation);
	}
	
	//make avatar face the center of the environment
	public void RotateToEnvCenter(){
		Vector3 center = exp.environmentController.center;
		center = new Vector3(center.x, transform.position.y, center.z); //set the y coordinate to the avatar's -- should still look straight ahead!

		transform.LookAt(center);
	}

}
