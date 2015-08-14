using UnityEngine;
using System.Collections;

public class Subject {
	
	public string name;
	public int score;
	public int blocks;

	public Subject(){

	}

	public Subject(string newName, int newScore, int newBlock){
		name = newName;
		score = newScore;
		blocks = newBlock;
	}

	public void IncrementBlock () {
		Debug.Log("incrementing session");
		blocks++;
	}
	
	public void AddScore ( int scoreToAdd ) {
		Debug.Log("adding to score");
		score += scoreToAdd;
	}

}
