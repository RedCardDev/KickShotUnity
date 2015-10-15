using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Texture[] CardTextures;

	public GameObject Dice;
	
	public GameObject Dice1;
	public GameObject Dice2;
	public GameObject Token;
	public GameObject Card1;
	public GameObject Card2;
	public GameObject Card3;
	public GameObject Card4;
	public GameObject Card5;
	public GameObject Card6;

	int[] hand;

	public GameObject DisplayDice1;
	public GameObject DisplayDice2;

	public float speed;
	public float tumble;
	public float DisplayTime;

	public bool Home;

	float HideDiceTime;

	bool rolling;
	bool DiceShown;

	int Dice1Val;
	int Dice2Val;

	// Use this for initialization
	void Start () {
		Home = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Dice1.GetComponent<Rigidbody> ().velocity == Vector3.zero && Dice2.GetComponent<Rigidbody> ().velocity == Vector3.zero && rolling == true) 
		{
			ResolveDiceRoll();
			HideDiceTime = Time.time + DisplayTime;
		}

		if (rolling == false && DiceShown == true & Time.time > HideDiceTime) 
		{
			HideDice();
		}

	}

	public void RandomHand()
	{
		Card1.GetComponent<Renderer>().material.mainTexture  = CardTextures [Random.Range(0,14)];
		Card2.GetComponent<Renderer>().material.mainTexture  = CardTextures [Random.Range(0,14)];
		Card3.GetComponent<Renderer>().material.mainTexture  = CardTextures [Random.Range(0,14)];
		Card4.GetComponent<Renderer>().material.mainTexture  = CardTextures [Random.Range(0,14)];
		Card5.GetComponent<Renderer>().material.mainTexture  = CardTextures [Random.Range(0,14)];
		Card6.GetComponent<Renderer>().material.mainTexture  = CardTextures [Random.Range(0,14)];
	}

	public void ShowCards()
	{
		Vector3 CardPosistion;

		CardPosistion = new Vector3 (3.5f, 0.06f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Card1));

		CardPosistion = new Vector3 (2.1f, 0.05f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Card2));

		CardPosistion = new Vector3 (0.7f, 0.04f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Card3));

		CardPosistion = new Vector3 (-0.7f, 0.03f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Card4));

		CardPosistion = new Vector3 (-2.1f, 0.02f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Card5));

		CardPosistion = new Vector3 (-3.5f, 0.01f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Card6));
	}

	public void HideCards()
	{
		Vector3 CardPosistion;
		
		CardPosistion = new Vector3 (3.5f, 0.06f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Card1));
		
		CardPosistion = new Vector3 (2.1f, 0.05f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Card2));
		
		CardPosistion = new Vector3 (0.7f, 0.04f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Card3));
		
		CardPosistion = new Vector3 (-0.7f, 0.03f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Card4));
		
		CardPosistion = new Vector3 (-2.1f, 0.02f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Card5));
		
		CardPosistion = new Vector3 (-3.5f, 0.01f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Card6));
	}

	public void MoveTokenToGoal()
	{
		Vector3 GoalPosistion = new Vector3 (0.0f, 0.1f, 6.5f);
		StartCoroutine (SmoothMovement (GoalPosistion, Token));
	}

	public void MoveTokenToCenter()
	{
		Vector3 CenterPosistion = new Vector3 (0.0f, 0.1f, 0.0f);
		StartCoroutine (SmoothMovement (CenterPosistion,Token));
	}

	public void FlipToken()
	{
		Quaternion newrotation;
		Vector3 FlipPosistion = new Vector3 (Token.transform.position.x, 0.5f, Token.transform.position.z);
		StartCoroutine (SmoothMovement (FlipPosistion, Token));
		if (Home == true) 
		{
			newrotation = Quaternion.Euler (90, 0, 0);
			Home = false;
		} 
		else 
		{
			newrotation = Quaternion.Euler (-90, 0, 0);
			Home = true;
		}
		StartCoroutine (SmoothRotation (newrotation));
	}

	void HideDice()
	{
		Vector3 SpawnPosistion;

		DisplayDice1.SetActive(false);
		DisplayDice2.SetActive(false);

		SpawnPosistion = new Vector3 (0f, -10f, 0f);
		Dice1.transform.position = SpawnPosistion;

		SpawnPosistion = new Vector3 (0f, -10f, 0f);
		Dice2.transform.position = SpawnPosistion;

		Dice1.SetActive(false);
		Dice2.SetActive(false);
		DiceShown = false;
	}

	public void RollDiceFunc()
	{
		if (rolling == true) {
		} else {
			DiceShown = true;

			Dice1.SetActive(true);
			Dice2.SetActive(true);
			DisplayDice1.SetActive(false);
			DisplayDice2.SetActive(false);

			Vector3 SpawnPosistion = new Vector3 (0.4f, 7f, 5f);
			Quaternion SpawnRotation = Quaternion.identity;
			Dice1.GetComponent<Rigidbody> ().position = SpawnPosistion;
			Dice1.GetComponent<Rigidbody> ().rotation = SpawnRotation;
			Dice1.GetComponent<Rigidbody> ().velocity = transform.forward * speed;
			Dice1.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
			
			Vector3 SpawnPosistion2 = new Vector3 (-0.4f, 7f, 5f);
			Quaternion SpawnRotation2 = Quaternion.identity;
			Dice2.GetComponent<Rigidbody> ().position = SpawnPosistion2;
			Dice2.GetComponent<Rigidbody> ().rotation = SpawnRotation2;
			Dice2.GetComponent<Rigidbody> ().velocity = transform.forward * speed;
			Dice2.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;

			rolling = true;
		}
	}

	public void AIRollDiceFunc()
	{
		if (rolling == true) {
		} else {
			DiceShown = true;
			
			Dice1.SetActive(true);
			Dice2.SetActive(true);
			DisplayDice1.SetActive(false);
			DisplayDice2.SetActive(false);
			
			Vector3 SpawnPosistion = new Vector3 (0.4f, 7f, -5f);
			Quaternion SpawnRotation = Quaternion.identity;
			Dice1.GetComponent<Rigidbody> ().position = SpawnPosistion;
			Dice1.GetComponent<Rigidbody> ().rotation = SpawnRotation;
			Dice1.GetComponent<Rigidbody> ().velocity = transform.forward * speed * -1;
			Dice1.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
			
			Vector3 SpawnPosistion2 = new Vector3 (-0.4f, 7f, -5f);
			Quaternion SpawnRotation2 = Quaternion.identity;
			Dice2.GetComponent<Rigidbody> ().position = SpawnPosistion2;
			Dice2.GetComponent<Rigidbody> ().rotation = SpawnRotation2;
			Dice2.GetComponent<Rigidbody> ().velocity = transform.forward * speed * -1;
			Dice2.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
			
			rolling = true;
		}
	}

	void ResolveDiceRoll()
	{
		Vector3 DicePosistion;
		float angle;
		float Minangle;
		rolling = false;

		// Get Dice1 value

		angle = Vector3.Angle (Dice1.GetComponent<Rigidbody> ().transform.up, Vector3.up);
		Minangle = angle;
		Dice1Val = 3;
		angle = Vector3.Angle (Dice1.GetComponent<Rigidbody> ().transform.up * -1, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice1Val = 4;
		}
		angle = Vector3.Angle (Dice1.GetComponent<Rigidbody> ().transform.right * -1, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice1Val = 2;
		}
		angle = Vector3.Angle (Dice1.GetComponent<Rigidbody> ().transform.right, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice1Val = 5;
		}
		angle = Vector3.Angle (Dice1.GetComponent<Rigidbody> ().transform.forward, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice1Val = 6;
		}
		angle = Vector3.Angle (Dice1.GetComponent<Rigidbody> ().transform.forward * -1, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice1Val = 1;
		}

		DisplayDice1.SetActive(true);
		DicePosistion = new Vector3 (1f, 9f, 0f);
		if (Dice1Val == 3)
			DisplayDice1.transform.eulerAngles = new Vector3(0,0,0);
		else if (Dice1Val == 4)
			DisplayDice1.transform.eulerAngles = new Vector3(0,0,180);
		else if (Dice1Val == 2)
			DisplayDice1.transform.eulerAngles = new Vector3(0,0,270);
		else if (Dice1Val == 5)
			DisplayDice1.transform.eulerAngles = new Vector3(0,0,90);
		else if (Dice1Val == 6)
			DisplayDice1.transform.eulerAngles = new Vector3(270,0,0);
		else if (Dice1Val == 1)
			DisplayDice1.transform.eulerAngles = new Vector3(90,0,0);
		DisplayDice1.transform.position = DicePosistion;


		// Get Dice2 value
		
		angle = Vector3.Angle (Dice2.GetComponent<Rigidbody> ().transform.up, Vector3.up);
		Minangle = angle;
		Dice2Val = 3;
		angle = Vector3.Angle (Dice2.GetComponent<Rigidbody> ().transform.up * -1, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice2Val = 4;
		}
		angle = Vector3.Angle (Dice2.GetComponent<Rigidbody> ().transform.right * -1, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice2Val = 2;
		}
		angle = Vector3.Angle (Dice2.GetComponent<Rigidbody> ().transform.right, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice2Val = 5;
		}
		angle = Vector3.Angle (Dice2.GetComponent<Rigidbody> ().transform.forward, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice2Val = 6;
		}
		angle = Vector3.Angle (Dice2.GetComponent<Rigidbody> ().transform.forward * -1, Vector3.up);
		if (angle < Minangle)
		{
			Minangle = angle;
			Dice2Val = 1;
		}

		DisplayDice2.SetActive(true);
		DicePosistion = new Vector3 (-1f, 9f, 0f);
		if (Dice2Val == 3)
			DisplayDice2.transform.eulerAngles = new Vector3(0,0,0);
		else if (Dice2Val == 4)
			DisplayDice2.transform.eulerAngles = new Vector3(0,0,180);
		else if (Dice2Val == 2)
			DisplayDice2.transform.eulerAngles = new Vector3(0,0,270);
		else if (Dice2Val == 5)
			DisplayDice2.transform.eulerAngles = new Vector3(0,0,90);
		else if (Dice2Val == 6)
			DisplayDice2.transform.eulerAngles = new Vector3(270,0,0);
		else if (Dice2Val == 1)
			DisplayDice2.transform.eulerAngles = new Vector3(90,0,0);
		DisplayDice2.transform.position = DicePosistion;

		Debug.Log ("" + Dice1Val + " " + Dice2Val);
	}

	protected IEnumerator SmoothMovement (Vector3 end, GameObject Obj)
	{
		Rigidbody rb;
		float sqrRemainingDistance = (Obj.transform.position - end).sqrMagnitude;
		
		while (sqrRemainingDistance > float.Epsilon)
		{
			rb = Obj.GetComponent<Rigidbody>();
			Vector3 newPosition = Vector3.MoveTowards (Obj.transform.position, end, 5f * Time.deltaTime);
			rb.MovePosition(newPosition);
			sqrRemainingDistance = (Obj.transform.position - end).sqrMagnitude;
			yield return null;			
		}
	}

	protected IEnumerator SmoothRotation (Quaternion rotation)
	{
		while (Token.transform.rotation != rotation) {
			float step = 500f * Time.deltaTime;
			Token.transform.rotation = Quaternion.RotateTowards (Token.transform.rotation, rotation, step);
			yield return null;
		}
		Vector3 FlipPosistion = new Vector3 (Token.transform.position.x, 0.1f, Token.transform.position.z);
		StartCoroutine (SmoothMovement (FlipPosistion, Token));
	}
}
