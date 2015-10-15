using UnityEngine;
using System.Collections;

public class Loadgame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadGame()
	{
		Application.LoadLevel("Main Game");
	}
}
