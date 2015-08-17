using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour {

	//ground plane -- used as spawn reference point
	public GameObject GroundPlane;


	//experiment singleton
	Experiment experiment { get { return Experiment.Instance; } }

	//object array & list
	List<GameObject> gameObjectList_Spawnable;
	//List<GameObject> gameObjectList_Spawned; //a list to keep track of the objects currently in the scene

	//TODO: incorporate into random spawning so that two objects don't spawn in super similar locations consecutively
	Vector3 lastSpawnPos;


	// Use this for initialization
	void Start () {
		gameObjectList_Spawnable = new List<GameObject> ();

		CreateObjectList (gameObjectList_Spawnable);
		lastSpawnPos = experiment.avatar.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateObjectList(List<GameObject> gameObjectListToFill){
		gameObjectListToFill.Clear();
		Object[] prefabs = Resources.LoadAll("Prefabs/Objects");
		for (int i = 0; i < prefabs.Length; i++) {
			gameObjectListToFill.Add((GameObject)prefabs[i]);
		}
	}

	void CreateSpawnableList (List<SpawnableObject> spawnableListToFill){
		spawnableListToFill.Clear();
		Object[] prefabs = Resources.LoadAll("Prefabs/Objects");
		for (int i = 0; i < prefabs.Length; i++) {
			SpawnableObject spawnable = ( (GameObject)prefabs[i] ).GetComponent<SpawnableObject>();
			spawnableListToFill.Add(spawnable);
		}
	}

	public GameObject ChooseSpawnableObject(string objectName){
		List<SpawnableObject> allSpawnables = new List<SpawnableObject>(); //note: this is technically getting instantiated twice now... as it's instantiated in CREATE as well.
		CreateSpawnableList (allSpawnables);

		for (int i = 0; i < allSpawnables.Count; i++) {
			if(allSpawnables[i].GetName() == objectName){
				return allSpawnables[i].gameObject;
			}
		}
		return null;
	}


	GameObject ChooseRandomObject(){
		if (gameObjectList_Spawnable.Count == 0) {
			Debug.Log ("No MORE objects to pick! Recreating object list.");
			CreateObjectList(gameObjectList_Spawnable); //IN ORDER TO REFILL THE LIST ONCE ALL OBJECTS HAVE BEEN USED
			if(gameObjectList_Spawnable.Count == 0){
				Debug.Log ("No objects to pick at all!"); //if there are still no objects in the list, then there weren't any to begin with...
				return null;
			}
		}


		int randomObjectIndex = Random.Range(0, gameObjectList_Spawnable.Count);
		GameObject chosenObject = gameObjectList_Spawnable[randomObjectIndex];
		gameObjectList_Spawnable.RemoveAt(randomObjectIndex);
		
		return chosenObject;
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
	public Vector3 GenerateRandomObjectLocation(){ //also rotates it randomly

		Vector3 origAvatarPos = experiment.avatar.transform.position;
		Quaternion origAvatarRot = experiment.avatar.transform.rotation;

		//NOT A PASS BY COPY. actually using the avatar's transform for the following method.
		//...thus, we store the original rot & pos to put it back.
		//this is a bit of a HACK. T_T
		Transform newTransform = experiment.avatar.transform;

		/*//move new object to avatar's x and z coordinates & rotation
		objectToMove.transform.position = new Vector3(experiment.avatar.transform.position.x, objectToMove.transform.position.y, experiment.avatar.transform.position.z);
		
		objectToMove.transform.forward = experiment.avatar.transform.forward;
		*/


		float bufferDistance = 0;
		float distance = -1.0f;
		//try to rotate several times to make sure there is enough space between the wall and the avatar for the object
		for(int i = 0; i < 15; i++){ //15 is SUPER ARBITRARY...

			//rotate object within heading offset threshold
			float randomAngle = Random.Range (Config.headingOffsetMin, Config.headingOffsetMax);
			int shouldBeNegative = Random.Range (0, 2); //will pick 1 or 0
			
			if (shouldBeNegative == 1) {
				randomAngle *= -1;
			}




			newTransform.RotateAround (experiment.avatar.transform.position, Vector3.up, randomAngle);
			
			//disable avatar for upcoming raycast
			experiment.avatar.enabled = false;
			
			//get distance between object and nearest wall
			Ray ray;
			RaycastHit hit;
			ray = new Ray (newTransform.position, newTransform.forward);
			if(Physics.Raycast(ray, out hit)){
				distance = hit.distance;
				if(hit.collider.gameObject.tag == "Wall"){
					bufferDistance = Config.bufferBetweenObjectsAndWall;
				}
				else{
					bufferDistance = Config.bufferBetweenObjects;
				}

				if(distance > bufferDistance + Config.bufferBetweenObjectsAndAvatar){
					//Debug.Log("Trying random object positioning again. Try #: " + (i));
					break; //GET OUT OF LOOP
				}


			}
			else{
				Debug.Log("Nothing in front of object!"); //this shouldn't happen if there are environment boundaries...
			}




		}

		
		//enable avatar again post raycast
		experiment.avatar.enabled = true;
		
		//move object a random distance in the appropriate direction
		if (distance != -1) {
			float randomDistance = Random.Range(Config.bufferBetweenObjectsAndAvatar, distance - bufferDistance);
			newTransform.position += newTransform.forward*randomDistance;
		}

		Vector3 newPos = newTransform.position;
		//put the avatar back to it's original location.
		newTransform.position = origAvatarPos;
		newTransform.rotation = origAvatarRot;

		return newPos;
		
	}


	//for more generic object spawning -- such as in Replay!
	public GameObject SpawnObject( GameObject objToSpawn, Vector3 spawnPos ){
		lastSpawnPos = spawnPos;
		GameObject spawnedObj = Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation) as GameObject;

		return spawnedObj;
	}

	//spawn random object at a specified location
	public GameObject SpawnRandomObjectXY (Vector3 spawnPosition){
		GameObject objToSpawn = ChooseRandomObject ();
		if (objToSpawn != null) {
			float spawnPosY = GetObjSpawnHeight(objToSpawn);

			spawnPosition.y = spawnPosY;

			GameObject newObject = Instantiate(objToSpawn, spawnPosition, objToSpawn.transform.rotation) as GameObject;

			lastSpawnPos = newObject.transform.position;
			
			MakeObjectFacePlayer(newObject);
			
			return newObject;
		}
		else{
			return null;
		}
	}

	//for spawning a random object at a random location
	public GameObject SpawnRandomObjectXY(){
		GameObject objToSpawn = ChooseRandomObject ();
		if (objToSpawn != null) {

			float spawnPosY = GetObjSpawnHeight(objToSpawn);
			Vector3 spawnPos = GenerateRandomObjectLocation();
			spawnPos = new Vector3(spawnPos.x, spawnPosY, spawnPos.z);

			GameObject newObject = Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation) as GameObject;

			//GenerateRandomObjectLocation(newObject);

			lastSpawnPos = newObject.transform.position;

			MakeObjectFacePlayer(newObject);

			return newObject;
		}
		else{
			return null;
		}
	}

	float GetObjSpawnHeight( GameObject obj ){
		//spawn object based on the size of its renderer
		Renderer objRenderer = obj.GetComponent<SpawnableObject>().myRenderer;
		if(objRenderer == null){
				Debug.Log("null spawnable object renderer on: " + obj.name);
		}
		
		//get the distance above the ground it should spawn at
		float spawnPosY = GroundPlane.transform.position.y + (objRenderer.bounds.extents.y);
		if(objRenderer.bounds.extents.y == 0){
				Debug.Log("zero size renderer: " + obj.name);
		}

		return spawnPosY;
	}

	void MakeObjectFacePlayer(GameObject obj){
		if (obj.transform.position == experiment.avatar.transform.position) { //make sure the object is not directly on top of the avatar.
			Debug.Log("Object is directly on top of the avatar! Cannot face avatar.");
			return;
		}

		//make object face the player
		Vector3 directionToAvatar = experiment.avatar.transform.position - obj.transform.position;
		
		float dotProd = Vector3.Dot(directionToAvatar, obj.transform.forward);
		float theta = Mathf.Acos( dotProd / ( directionToAvatar.magnitude*obj.transform.forward.magnitude ) );
		obj.transform.RotateAround(obj.transform.position, Vector3.up, theta);
		
		if(obj.transform.forward.normalized != directionToAvatar.normalized){
			obj.transform.RotateAround(obj.transform.position, Vector3.up, 180.0f);
		}
	}

}
