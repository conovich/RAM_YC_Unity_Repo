using UnityEngine;
using System.Collections;

public class Subject {
	
	public string name;
	public int score;
	public int session;

	public Subject(){

	}

	public Subject(string newName, int newScore, int newSession){
		name = newName;
		score = newScore;
		session = newSession;
	}

	public void IncrementSession () {
		Debug.Log("incrementing session");
		session++;
	}
	
	public void AddScore ( int scoreToAdd ) {
		Debug.Log("adding to score");
		score += scoreToAdd;
	}

}
