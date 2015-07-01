using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour {

	//ground plane -- used as spawn reference point
	public GameObject GroundPlane;


	//experiment singleton
	Experiment experiment { get { return Experiment.Instance; } }

	//object array & list
	List<GameObject> gameObjectList;

	//TODO: incorporate into random spawning so that two objects don't spawn in super similar locations consecutively
	Vector3 lastSpawnPos;


	// Use this for initialization
	void Start () {
		CreateObjectList ();
		lastSpawnPos = experiment.avatar.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateObjectList(){
		gameObjectList = new List<GameObject>();
		Object[] prefabs = Resources.LoadAll("Prefabs/Objects");
		for (int i = 0; i < prefabs.Length; i++) {
			gameObjectList.Add((GameObject)prefabs[i]);
		}
	}


	GameObject ChooseRandomObject(){
		if (gameObjectList.Count == 0) {
			Debug.Log ("No more objects to pick!");
			return null;
		}
		else{
			int randomObjectIndex = Random.Range(0, gameObjectList.Count);
			GameObject chosenObject = gameObjectList[randomObjectIndex];
			gameObjectList.RemoveAt(randomObjectIndex);
			
			return chosenObject;
		}
	}
	
	//FROM CONFIG FILE, FOR REFERENCE
	//trial and object spawning variables
	/*public bool shouldMaximizeLearningAngle;
	public int minDegreeBetweenLearningTrials;
	public bool shouldDoMassedObjects;
	public bool shouldRandomizeTestOrder;
	public bool shouldRandomizeLearnOrder;
	public float headingOffsetMin;
	public float headingOffsetMax;

	//object buffer variables
	public float bufferBetweenObjects;
	public float bufferBetweenObjectsAndWall;
	public float bufferBetweenObjectsAndAvatar;*/
	
	//TODO: consider last spawn position???
	void MoveObjectToRandomLocation(GameObject objectToMove){ //also rotates it randomly
		
		//move new object to avatar's x and z coordinates & rotation
		objectToMove.transform.position = new Vector3(experiment.avatar.transform.position.x, objectToMove.transform.position.y, experiment.avatar.transform.position.z);
		
		objectToMove.transform.forward = experiment.avatar.transform.forward;



		float bufferDistance = 0;
		float distance = -1.0f;
		//try to rotate several times to make sure there is enough space between the wall and the avatar for the object
		for(int i = 0; i < 15; i++){ //15 is SUPER ARBITRARY...

			//rotate object within heading offset threshold
			float randomAngle = Random.Range (experiment.config.headingOffsetMin, experiment.config.headingOffsetMax);
			int shouldBeNegative = Random.Range (0, 2); //will pick 1 or 0
			
			if (shouldBeNegative == 1) {
				randomAngle *= -1;
			}




			objectToMove.transform.RotateAround (experiment.avatar.transform.position, Vector3.up, randomAngle);
			
			//disable avatar for upcoming raycast
			experiment.avatar.enabled = false;
			
			//get distance between object and nearest wall
			Ray ray;
			RaycastHit hit;
			ray = new Ray (objectToMove.transform.position, objectToMove.transform.forward);
			if(Physics.Raycast(ray, out hit)){
				distance = hit.distance;
				if(hit.collider.gameObject.tag == "Wall"){
					bufferDistance = experiment.config.bufferBetweenObjectsAndWall;
				}
				else{
					bufferDistance = experiment.config.bufferBetweenObjects;
				}

				if(distance < bufferDistance + experiment.config.bufferBetweenObjectsAndAvatar){
					Debug.Log("Trying random object positioning again. Try #: " + (i));
					continue; //TRY AGAIN.
				}


			}
			else{
				Debug.Log("Nothing in front of object!"); //this shouldn't happen if there are environment boundaries...
			}




		}


		/*
		//rotate object within heading offset threshold
		float randomAngle = Random.Range (experiment.config.headingOffsetMin, experiment.config.headingOffsetMax);
		int shouldBeNegative = Random.Range (0, 2); //will pick 1 or 0
		
		if (shouldBeNegative == 1) {
			randomAngle *= -1;
		}
		
		objectToMove.transform.RotateAround (experiment.avatar.transform.position, Vector3.up, randomAngle);
		
		//disable avatar for upcoming raycast
		experiment.avatar.enabled = false;
		
		//get distance between object and nearest wall
		Ray ray;
		RaycastHit hit;
		float distance = -1.0f;
		float bufferDistance = 0;
		ray = new Ray (objectToMove.transform.position, objectToMove.transform.forward);
		if(Physics.Raycast(ray, out hit)){
			distance = hit.distance;
			if(hit.collider.gameObject.tag == "Wall"){
				bufferDistance = experiment.config.bufferBetweenObjectsAndWall;
			}
			else{
				bufferDistance = experiment.config.bufferBetweenObjects;
			}
		}
		else{
			Debug.Log("Nothing in front of object!");
		}

*/
		
		//enable avatar again post raycast
		experiment.avatar.enabled = true;
		
		//move object a random distance in the appropriate direction
		if (distance != -1) {
			float randomDistance = Random.Range(experiment.config.bufferBetweenObjectsAndAvatar, distance - bufferDistance);
			objectToMove.transform.position += objectToMove.transform.forward*randomDistance;
		}
		
	}


	//for more generic object spawning
	public void SpawnObject( GameObject objToSpawn, Vector3 spawnPos ){
		lastSpawnPos = spawnPos;
		Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation);
	}

	//for spawning a random object at a random location
	public GameObject SpawnRandomObject(){
		GameObject objToSpawn = ChooseRandomObject ();
		if (objToSpawn != null) {

			//spawn object based on the size of its renderer

			Renderer objRenderer = objToSpawn.GetComponent<SpawnableObject>().myRenderer;
			if(objRenderer == null){
				Debug.Log("null spawnable object renderer on: " + objToSpawn.name);
			}

			//get the distance above the ground it should spawn at
			float spawnPosY = GroundPlane.transform.position.y + (objRenderer.bounds.extents.y);
			if(objRenderer.bounds.extents.y == 0){
				Debug.Log("zero size renderer: " + objToSpawn.name);
			}

			Vector3 spawnPosition = new Vector3(0.0f, spawnPosY, 0.0f);
			GameObject newObject = Instantiate(objToSpawn, spawnPosition, objToSpawn.transform.rotation) as GameObject;

			MoveObjectToRandomLocation(newObject);

			lastSpawnPos = newObject.transform.position;

			//make object face the player
			Vector3 directionToAvatar = experiment.avatar.transform.position - newObject.transform.position;

			float dotProd = Vector3.Dot(directionToAvatar, newObject.transform.forward);
			float theta = Mathf.Acos( dotProd / ( directionToAvatar.magnitude*newObject.transform.forward.magnitude ) );
			newObject.transform.RotateAround(newObject.transform.position, Vector3.up, theta);

			if(newObject.transform.forward.normalized != directionToAvatar.normalized){
				newObject.transform.RotateAround(newObject.transform.position, Vector3.up, 180.0f);
			}

			return newObject;
		}
		else{
			return null;
		}
	}

}
