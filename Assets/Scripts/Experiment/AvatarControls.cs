using UnityEngine;
using System.Collections;

public class AvatarControls : MonoBehaviour{

	Experiment exp  { get { return Experiment.Instance; } }


	public bool ShouldLockControls = false;



	//NEW STUFF NOT PHYSICS BASED -- DIDN'T WORK FOR FRAME RATE INDEPENDENCE, ALSO MESSES UP COLLISION DETECTION. DON'T USE.
	/*

	//MOVING
	float absMaxMoveSpeed = 1.0f;
	float moveAccMultiplier = 2.0f; //gets multiplied by Time.deltaTime later for a slower, framerate independent acceleration
	
	float currentMoveSpeed = 0.0f;

	//TURNING
	float absMaxRotSpeed = 1.5f;
	float rotAccMultiplier = 2.3f;

	float currentRotSpeed = 0.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (exp.currentState == Experiment.ExperimentState.inExperiment) {
			if(!ShouldLockControls){
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; // TODO: on collision, don't allow a change in angular velocity?
				
				GetInput ();
			}
			else{
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}
	
	void GetInput(){
		//moving
		float verticalAxisInput = Input.GetAxis ("Vertical");

		if(verticalAxisInput > 0.01f){
			MoveForward();
		}
		else if(verticalAxisInput < -0.01f){
			MoveBackward();
		}
		else{
			StopMoving();
		}

		//turning
		float horizontalAxisInput = Input.GetAxis ("Horizontal");
		
		if(horizontalAxisInput > 0.01f){
			TurnRight();
		}
		else if(horizontalAxisInput < -0.01f){
			TurnLeft();
		}
		else{
			StopTurning();
		}
	}

	//MOVING
	void MoveBackward(){
		if(currentMoveSpeed > 0){
			currentMoveSpeed = 0;
		}

		if(currentMoveSpeed > -absMaxMoveSpeed){
			currentMoveSpeed -= moveAccMultiplier*Time.deltaTime;
		}
		
		if(currentMoveSpeed < -absMaxMoveSpeed){
			currentMoveSpeed = -absMaxMoveSpeed;
		}
		
		Move ();

	}
	
	void MoveForward(){
		if(currentMoveSpeed < 0){
			currentMoveSpeed = 0;
		}

		if(currentMoveSpeed < absMaxMoveSpeed){
			currentMoveSpeed += moveAccMultiplier*Time.deltaTime;
		}
		
		if(currentMoveSpeed > absMaxMoveSpeed){
			currentMoveSpeed = absMaxMoveSpeed;
		}
		
		Move ();

	}

	void Move(){
		
		transform.position += ( Vector3.forward * currentMoveSpeed );
		
	}
	
	void StopMoving(){
		float moveIncrement = 2f * moveAccMultiplier * Time.deltaTime;
		
		if(currentMoveSpeed < 0){
			currentMoveSpeed += moveIncrement;
		}
		else if(currentMoveSpeed > 0){
			currentMoveSpeed -= moveIncrement;
		}
		
		if(currentMoveSpeed >= -moveIncrement && currentMoveSpeed < moveIncrement){
			currentMoveSpeed = 0;
		}
		
		Move();
	}



	//TURNING 

	void TurnLeft(){
		if(currentRotSpeed > 0){
			currentRotSpeed = 0;
		}
		
		if(currentRotSpeed > -absMaxRotSpeed){
			currentRotSpeed -= rotAccMultiplier*Time.deltaTime;
		}
		
		if(currentRotSpeed < -absMaxRotSpeed){
			currentRotSpeed = -absMaxMoveSpeed;
		}
		
		TurnNew ();
		
	}
	
	void TurnRight(){
		if(currentRotSpeed < 0){
			currentRotSpeed = 0;
		}
		
		if(currentRotSpeed < absMaxRotSpeed){
			currentRotSpeed += rotAccMultiplier*Time.deltaTime;
		}
		
		if(currentRotSpeed > absMaxRotSpeed){
			currentRotSpeed = absMaxRotSpeed;
		}
		
		TurnNew ();
		
	}

	void TurnNew(){
		transform.RotateAround( transform.position, Vector3.up, currentRotSpeed );
	}

	void StopTurning(){
		float rotIncrement = 2f * rotAccMultiplier * Time.deltaTime;
		
		if(currentRotSpeed < 0){
			currentRotSpeed += rotIncrement;
		}
		else if(currentRotSpeed > 0){
			currentRotSpeed -= rotIncrement;
		}
		
		if(currentRotSpeed >= -rotIncrement && currentRotSpeed < rotIncrement){
			currentRotSpeed = 0;
		}
		
		TurnNew ();
	}
*/



	float RotationSpeed = 1;


	// Use this for initialization
	void Start () {
		//when in replay, we don't want physics collision interfering with anything
		if(ExperimentSettings.isReplay){
			GetComponent<Collider>().enabled = false;
		}
		else{
			GetComponent<Collider>().enabled = true;
		}
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

		if ( Mathf.Abs(verticalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter

			GetComponent<Rigidbody>().velocity = transform.forward*verticalAxisInput*Config.driveSpeed; //should have no deltaTime framerate component -- given the frame, you should always be moving at a speed directly based on the input																								//NOTE: potential problem with this method: joysticks and keyboard input will have different acceleration calibration.

		}
		else{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}


		float horizontalAxisInput = Input.GetAxis ("Horizontal");

		if (Mathf.Abs (horizontalAxisInput) > 0.0f) { //EPSILON should be accounted for in Input Settings "dead zone" parameter

			//Turn( horizontalAxisInput*RotationSpeed*(Time.deltaTime) ); 
			GetComponent<Rigidbody> ().angularVelocity = Vector3.up * horizontalAxisInput * RotationSpeed;
			//Debug.Log("horizontal axis ANG VEL = " + GetComponent<Rigidbody>().angularVelocity);
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
	

	public IEnumerator MoveToTargetObject(GameObject target){

		yield return new WaitForSeconds(Config.pauseBeforeSpinTime);

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


		float moveRate = 1.0f / Config.driveTime;
		float tElapsed = 0.0f;
		float positionEpsilon = 0.01f;
		//stop when you have collided with something
		while(!CheckXZPositionsCloseEnough(transform.position, desiredPosition, positionEpsilon)){
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

		yield return new WaitForSeconds(Config.pauseBeforeSpinTime);

	}

	bool CheckXZPositionsCloseEnough(Vector3 position1, Vector3 position2, float epsilon){
		float xDiff = Mathf.Abs (position1.x - position2.x);
		float zDiff = Mathf.Abs (position1.z - position2.z);

		if (xDiff < epsilon && zDiff < epsilon) {
			return true;
		}
		else {
			return false;
		}
	}

	public void RotateTowards(Vector3 position){
		position = new Vector3(position.x, transform.position.y, position.z); //set the y coordinate to the avatar's -- should still look straight ahead!
		
		transform.LookAt(position);
	}

	public Vector3 SetLearningLocation001(Vector3 objectPosition){
		int numPositioningAttempts = 0;

		Vector3 newPos = objectPosition;
		while ( Vector3.Distance (newPos, objectPosition) < Config.bufferBetweenObjectsAndAvatar && numPositioningAttempts < 15) { //15 is arbitrary!
			newPos = exp.environmentController.GetRandomPositionWithinWallsXZ (Config.bufferBetweenObjectsAndWall);
			numPositioningAttempts++;
		}

		newPos = new Vector3 (newPos.x, transform.position.y, newPos.z);
		transform.position = newPos;

		return newPos;
	}

	public Vector3 SetLearningLocation002(Vector3 objectPosition, Vector3 lastAvatarPos){
		//Vector3 avObjDiff = lastAvatarPos - objectPosition;

		int numPositioningAttempts = 0;


		float distance = 0;
		float bufferDistance = 0;
		while ( numPositioningAttempts < 15 ) { //15 is arbitrary!
			numPositioningAttempts++;

			objectPosition = new Vector3(objectPosition.x, transform.position.y, objectPosition.z);
			transform.position = objectPosition; //put avatar in object position
			RotateTowards (lastAvatarPos);

			float randomRotation = Random.Range (Config.minDegreeBetweenLearningTrials, Config.maxDegreeBetweenLearningTrials);
			int shouldBeNegative = Random.Range (0, 2); //will pick 1 or 0
			
			if (shouldBeNegative == 1) {
				randomRotation *= -1;
			}


			transform.RotateAround (transform.position, Vector3.up, randomRotation); //rotate to face the line along which the avatar must be placed next!



			//get distance between object and nearest wall
			Ray ray;
			RaycastHit hit;
			ray = new Ray (transform.position, transform.forward); //the avatar is currently in the object's position
			if (Physics.Raycast (ray, out hit)) {
				distance = hit.distance;
				if (hit.collider.gameObject.tag == "Wall") {
					bufferDistance = Config.bufferBetweenObjectsAndWall;
				} else {
					bufferDistance = Config.bufferBetweenObjects;
				}

				//total buffer = wall buffer + avatar buffer
				//if the distance between the object and the wall is greater than the buffer distance, this is a good line on which to generate the next avatar position
				if (distance > bufferDistance + Config.bufferBetweenObjectsAndAvatar) {
					//Debug.Log("Trying random object positioning again. Try #: " + (i));
					break; //GET OUT OF LOOP
				}

			}
		}

		float randomBetweenDistance = Random.Range (Config.bufferBetweenObjectsAndAvatar, distance); 	//get a distance between the object and the wall
		Vector3 newAvatarPos = transform.position + randomBetweenDistance * transform.forward;		//set the new position somewhere along the line between the avatar and the wall

		newAvatarPos = new Vector3 (newAvatarPos.x, transform.position.y, newAvatarPos.z);
		transform.position = newAvatarPos;
		return newAvatarPos;
	}

	//assumes the avatar is in the correct location currently
	public Quaternion SetYRotationAwayFrom(Vector3 objectPosition, float minDegree, float maxDegree){
		float randomYRotation = Random.Range (Config.minDegreeBetweenLearningTrials, Config.maxDegreeBetweenLearningTrials);
		RotateTowards (objectPosition);

		transform.RotateAround (transform.position, Vector3.up, randomYRotation);

		return transform.rotation;
	}

	public float GenerateRandomRotationY(){
		float randomYRotation = Random.Range (0.0f, 360.0f);
		
		return randomYRotation;
	}
	
	//only in y axis
	public Quaternion SetRandomRotationY(){
		float randomYRotation = GenerateRandomRotationY ();
		
		transform.RotateAround(transform.position, Vector3.up, randomYRotation);
		
		return transform.rotation;
	}
	
}
