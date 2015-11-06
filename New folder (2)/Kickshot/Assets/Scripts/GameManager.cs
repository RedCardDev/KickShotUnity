using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	public Texture[] CardTextures;
	public GameObject[] Cards;
	
	public GameObject Dice;
	
	public GameObject Dice1;
	public GameObject Dice2;
	public GameObject Token;
	
	string[] Hand;
	string[] Deck;

	string[] AIHand;
	string[] AIDeck;
	
	public GameObject DisplayDice1;
	public GameObject DisplayDice2;
	
	public float speed;
	public float tumble;
	public float DisplayTime;
	
	public bool Home;

	public int PlayerScore;
	public int AIScore;

	public int PassNum;
	public int InterceptNum;
	public int GoalShotLeftNum;
	public int GoalShotRightNum;
	public int BlockLeftNum;
	public int BlockRightNum;


	int CurrentPassNum;
	int CurrentInterceptNum;
	int CurrentGoalShotLeftNum;
	int CurrentGoalShotRightNum;
	int CurrentBlockLeftNum;
	int CurrentBlockRightNum;

	float HideDiceTime;
	
	bool rolling;
	bool singlerolling;
	bool DiceShown;
	bool PlayerHasBall;
	
	int Dice1Val;
	int Dice2Val;
	int BallPos; // 0 = center, 11 = top
	int Passing;
	int Intercepting;
	
	// Use this for initialization
	void Start () {
		Hand = new string[6];
		Deck = new string[100];
		Home = true;
		PlayerHasBall = true;
		Passing = 0;
		Intercepting = 0;
		Shuffle();
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

	public void Shuffle()
	{
		int i;
		float Selected;

		CurrentPassNum = PassNum;
		CurrentInterceptNum = InterceptNum;
		CurrentGoalShotLeftNum = GoalShotLeftNum;
		CurrentGoalShotRightNum = GoalShotRightNum;
		CurrentBlockLeftNum = BlockLeftNum;
		CurrentBlockRightNum = BlockRightNum;


		for (i = 0; i < 4; i++) 
		{
			Selected = Random.Range(0, CurrentPassNum + CurrentGoalShotLeftNum + CurrentGoalShotRightNum);
			if (Selected < CurrentPassNum)
			{
				Hand[i] = "Pass";
				CurrentPassNum--;
				Cards[i].GetComponent<Renderer>().material.mainTexture  = CardTextures[14];
			}
			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum)
			{
				Hand[i] = "GoalShotLeft";
				CurrentGoalShotLeftNum--;
				Cards[i].GetComponent<Renderer>().material.mainTexture  = CardTextures[9];
			}
			else
			{
				Hand[i] = "GoalShotRight";
				CurrentGoalShotRightNum--;
				Cards[i].GetComponent<Renderer>().material.mainTexture  = CardTextures[10];
			}
		}

		for (i = 0; i < 2; i++) 
		{
			Selected = Random.Range(0, CurrentInterceptNum + CurrentBlockLeftNum + CurrentBlockRightNum);

			if (Selected < CurrentInterceptNum)
			{
				Hand[i+4] = "Intercept";
				CurrentInterceptNum--;
				Cards[i+4].GetComponent<Renderer>().material.mainTexture  = CardTextures[12];
			}
			else if (Selected < CurrentInterceptNum + CurrentBlockLeftNum)
			{
				Hand[i+4] = "BlockLeft";
				CurrentBlockLeftNum--;
				Cards[i+4].GetComponent<Renderer>().material.mainTexture  = CardTextures[5];
			}
			else
			{
				Hand[i+4] = "BlockRight";
				CurrentBlockRightNum--;
				Cards[i+4].GetComponent<Renderer>().material.mainTexture  = CardTextures[6];
			}
		}

		i = 0;

		while (CurrentPassNum + CurrentInterceptNum + CurrentGoalShotRightNum + CurrentGoalShotLeftNum + CurrentBlockRightNum + CurrentBlockLeftNum > 0) 
		{
			Selected = Random.Range(0, CurrentPassNum + CurrentInterceptNum + CurrentGoalShotRightNum + CurrentGoalShotLeftNum + CurrentBlockRightNum + CurrentBlockLeftNum);
			if (Selected < CurrentPassNum)
			{
				Deck[i] = "Pass";
				CurrentPassNum--;
				i++;
			}
			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum)
			{
				Deck[i] = "GoalShotLeft";
				CurrentGoalShotLeftNum--;
				i++;
			}
			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum + CurrentGoalShotRightNum)
			{
				Deck[i] = "GoalShotRight";
				CurrentGoalShotRightNum--;
				i++;
			}

			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum + CurrentGoalShotRightNum + CurrentInterceptNum)
			{
				Deck[i] = "Intercept";
				CurrentInterceptNum--;
				i++;
			}

			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum + CurrentGoalShotRightNum + CurrentInterceptNum + CurrentBlockRightNum)
			{
				Deck[i] = "BlockRight";
				CurrentBlockRightNum--;
				i++;
			}
			else
			{
				Deck[i] = "BlockLeft";
				CurrentBlockLeftNum--;
				i++;
			}
		}
	}

	public void AIShuffle()
	{
		int i;
		float Selected;
		
		CurrentPassNum = PassNum;
		CurrentInterceptNum = InterceptNum;
		CurrentGoalShotLeftNum = GoalShotLeftNum;
		CurrentGoalShotRightNum = GoalShotRightNum;
		CurrentBlockLeftNum = BlockLeftNum;
		CurrentBlockRightNum = BlockRightNum;
		
		
		for (i = 0; i < 4; i++) 
		{
			Selected = Random.Range(0, CurrentPassNum + CurrentGoalShotLeftNum + CurrentGoalShotRightNum);
			if (Selected < CurrentPassNum)
			{
				AIHand[i] = "Pass";
				CurrentPassNum--;
			}
			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum)
			{
				AIHand[i] = "GoalShotLeft";
				CurrentGoalShotLeftNum--;
			}
			else
			{
				AIHand[i] = "GoalShotRight";
				CurrentGoalShotRightNum--;
			}
		}
		
		for (i = 0; i < 2; i++) 
		{
			Selected = Random.Range(0, CurrentInterceptNum + CurrentBlockLeftNum + CurrentBlockRightNum);
			
			if (Selected < CurrentInterceptNum)
			{
				AIHand[i+4] = "Intercept";
				CurrentInterceptNum--;
			}
			else if (Selected < CurrentInterceptNum + CurrentBlockLeftNum)
			{
				AIHand[i+4] = "BlockLeft";
				CurrentBlockLeftNum--;
			}
			else
			{
				AIHand[i+4] = "BlockRight";
				CurrentBlockRightNum--;
			}
		}
		
		i = 0;
		
		while (CurrentPassNum + CurrentInterceptNum + CurrentGoalShotRightNum + CurrentGoalShotLeftNum + CurrentBlockRightNum + CurrentBlockLeftNum > 0) 
		{
			Selected = Random.Range(0, CurrentPassNum + CurrentInterceptNum + CurrentGoalShotRightNum + CurrentGoalShotLeftNum + CurrentBlockRightNum + CurrentBlockLeftNum);
			if (Selected < CurrentPassNum)
			{
				AIDeck[i] = "Pass";
				CurrentPassNum--;
				i++;
			}
			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum)
			{
				AIDeck[i] = "GoalShotLeft";
				CurrentGoalShotLeftNum--;
				i++;
			}
			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum + CurrentGoalShotRightNum)
			{
				AIDeck[i] = "GoalShotRight";
				CurrentGoalShotRightNum--;
				i++;
			}
			
			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum + CurrentGoalShotRightNum + CurrentInterceptNum)
			{
				AIDeck[i] = "Intercept";
				CurrentInterceptNum--;
				i++;
			}
			
			else if (Selected < CurrentPassNum + CurrentGoalShotLeftNum + CurrentGoalShotRightNum + CurrentInterceptNum + CurrentBlockRightNum)
			{
				AIDeck[i] = "BlockRight";
				CurrentBlockRightNum--;
				i++;
			}
			else
			{
				AIDeck[i] = "BlockLeft";
				CurrentBlockLeftNum--;
				i++;
			}
		}
	}
	
	public void PublicPass (){
		StartCoroutine (Pass());
	}

	protected IEnumerator Pass()
	{
		int distance = 1;
		int NewPos = 0;

		if (PlayerHasBall == true)
		{
			RollDiceFunc ();
		}
		else
		{
			AIRollDiceFunc ();
		}
		Passing = 1;

		while (Passing == 1) 
		{
			yield return null;
		}
		
		if (Dice1Val > Dice2Val)
			distance = Dice1Val;
		else if (Dice1Val < Dice2Val)
			distance = Dice2Val;
		else if (Dice1Val == Dice2Val)
			distance = Dice1Val + 1;
		
		if (PlayerHasBall == true) 
		{
			NewPos = BallPos + distance;
		}
		else
		{
			NewPos = BallPos - distance;
		}
		
		if (NewPos >= 11) 
		{
			NewPos = 11;
		}
		
		if (NewPos <= -11) 
		{
			NewPos = -11;
		}
		BallPos = NewPos;
		MoveTokenToPos (NewPos);

		Passing = 3;

		while (Passing == 3) 
		{
			yield return null;
		}

		if (Dice1Val == 1 || Dice2Val == 1) 
		{
			FlipToken ();
		}

		if (NewPos <= -11 || NewPos >= 11) {
			if (Dice1Val != 1 && Dice2Val != 1) 
			{
				OpposedRollDiceFunc();
				Passing = 5;

				while (Passing == 5) 
				{
					yield return null;
				}

				while (Dice1Val == Dice2Val)
				{
					OpposedRollDiceFunc();
					Passing = 5;
					
					while (Passing == 5) 
					{
						yield return null;
					}
				}

				if (Dice1Val > Dice2Val)
				{
					if (PlayerHasBall == true)
					{
						PlayerScore ++;
						MoveTokenToCenter();
					}
					else
					{
						FlipToken();
					}
				}
				else if (Dice1Val < Dice2Val)
				{
					if (PlayerHasBall == false)
					{
						AIScore ++;
						MoveTokenToCenter();
					}
					else
					{
						FlipToken();
					}
				}

			}
		}
		Passing = 0;
		
	}

	public void PublicIntercept()
	{
		StartCoroutine (Intercept());
	}

	protected IEnumerator Intercept()
	{
		
		if (PlayerHasBall == true)
		{
			RollDiceFunc ();
		}
		else
		{
			AIRollDiceFunc ();
		}
		Intercepting = 1;
		
		while (Intercepting == 1) 
		{
			yield return null;
		}

		if (Dice1Val == Dice2Val && Dice1Val != 1) 
		{
			FlipToken();
		}
		Intercepting = 0;
	}

	void MoveTokenToPos(int Pos)
	{
		Vector3 NewPosistion;
		NewPosistion = new Vector3 (0.0f, 0.1f, 0.0f);
		
		if (Pos > 0) 
		{
			//Pos = Pos * -1;
			NewPosistion = new Vector3 (0.0f, 0.1f, Pos * -0.6f + 0.2f);
		}
		if (Pos < 0) 
		{
			//Pos = Pos * -1;
			NewPosistion = new Vector3 (0.0f, 0.1f, Pos * -0.6f - 0.2f);
		}
		if (Pos == 0) 
		{
			NewPosistion = new Vector3 (0.0f, 0.1f, 0.0f);
		}
		StartCoroutine (SmoothMovement (NewPosistion, Token));
	}
	
	public void ShowCards()
	{
		Vector3 CardPosistion;
		
		CardPosistion = new Vector3 (3.5f, 0.06f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[0]));
		
		CardPosistion = new Vector3 (2.1f, 0.05f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[1]));
		
		CardPosistion = new Vector3 (0.7f, 0.04f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[2]));
		
		CardPosistion = new Vector3 (-0.7f, 0.03f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[3]));
		
		CardPosistion = new Vector3 (-2.1f, 0.02f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[4]));
		
		CardPosistion = new Vector3 (-3.5f, 0.01f, 7f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[5]));
	}
	
	public void HideCards()
	{
		Vector3 CardPosistion;
		
		CardPosistion = new Vector3 (3.5f, 0.06f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[0]));
		
		CardPosistion = new Vector3 (2.1f, 0.05f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[1]));
		
		CardPosistion = new Vector3 (0.7f, 0.04f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[2]));
		
		CardPosistion = new Vector3 (-0.7f, 0.03f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[3]));
		
		CardPosistion = new Vector3 (-2.1f, 0.02f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[4]));
		
		CardPosistion = new Vector3 (-3.5f, 0.01f, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards[5]));
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
		
		if (PlayerHasBall == true) 
		{
			PlayerHasBall = false;
		} 
		else 
		{
			PlayerHasBall = true;
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

		if (Passing == 1 || Passing == 5) 
		{
			Passing++;
		}

		if (Intercepting == 1) 
		{
			Intercepting++;
		}
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

	public void OpposedRollDiceFunc()
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
			
			Vector3 SpawnPosistion2 = new Vector3 (-0.4f, 7f, -5f);
			Quaternion SpawnRotation2 = Quaternion.identity;
			Dice2.GetComponent<Rigidbody> ().position = SpawnPosistion2;
			Dice2.GetComponent<Rigidbody> ().rotation = SpawnRotation2;
			Dice2.GetComponent<Rigidbody> ().velocity = transform.forward * speed * -1;
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
		if (Passing == 3) 
		{
			Passing++;
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
