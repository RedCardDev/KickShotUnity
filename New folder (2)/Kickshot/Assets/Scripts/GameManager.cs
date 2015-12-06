using UnityEngine;
using System.Collections;
using UnityEngine.UI;  // added for Text object 

public class GameManager : MonoBehaviour {
	
	public Texture[] CardTextures;
	public GameObject[] Cards;
	
	public GameObject Dice;
	
	public GameObject Dice1;
	public GameObject Dice2;
	public GameObject Token;
	
	public GameObject DisplayCard;
	public GameObject CardButtons;
	public GameObject RollButton;
	
	public Button PlayCardButton;
	public Button DiscardButton;
	
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
	
	public KeyCode ActivateKey = KeyCode.Mouse0;
	
	
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
	bool playerTurn; // from merge
	bool Discarded;
	
	int Dice1Val;
	int Dice2Val;
	int BallPos; // 0 = center, 11 = top
	int Passing;
	int Intercepting;
	int GoalShotting;
	int GoalBlocking;

	int PlayerHandSize;
	int AIHandSize;

	int FreeKickCardsLeft;

	string GoalShotSide;
	
	int CardChoice;
	
	int DeckPos;
	int AIDeckPos;
	int DeckSize;
	int RoundEnding;
	int Round;

	int Skipped; // Used to check for the end of game

	public Text ScoreText;
	public Text CardText;
	
	// from merge
	public Text message;
	
	bool diceRollComplete = false;
	stages stage;
	enum stages{RollOff, CardSelect,CardPlay,DiceRoll,CardReaction,AITurn,etc};
	// Use this for initialization
	void Start () {
		Hand = new string[10];
		Deck = new string[1000];
		AIHand = new string[10];
		AIDeck = new string[1000];
		Home = true;
		PlayerHasBall = true;
		Discarded = false;
		Passing = 0;
		Intercepting = 0;
		GoalShotting = 0;
		GoalBlocking = 0;
		DeckPos = 0;
		PlayerHandSize = 6;
		AIHandSize = 6;
		FreeKickCardsLeft = 4;
		DeckSize = PassNum + InterceptNum + GoalShotLeftNum + GoalShotRightNum + BlockLeftNum + BlockRightNum - 6;
		Skipped = 0;
		Round = 0;


		Shuffle();
		AIShuffle();
		CardText.text = "";
		StartCoroutine (DisplayMessage ("Tap to Roll Off!", 3000));
		RollButton.SetActive(true);
		stage = stages.RollOff;
	}
	
	IEnumerator DisplayMessage(string newMessage, int time){
		message.text = newMessage;
		//print ("yes");
		System.DateTime now = System.DateTime.Now;
		while(System.DateTime.Now < now.AddMilliseconds(time)){
			//print ("wait");
			yield return null;}
		if (message.text == newMessage) {
			message.text = "";
		}
		yield return null;
	} // end DisplayMessage // from merge

	IEnumerator DisplayCardMessage(string newMessage, int time){
		message.text = "";
		if (newMessage == "Pass") 
		{
			CardText.text = "AI plays pass";
			DisplayCard.GetComponent<Renderer> ().material.mainTexture = CardTextures [13];
			DisplayCard.SetActive(true);
		}

		if (newMessage == "Intercept") {
			CardText.text = "AI plays intercept";
			DisplayCard.GetComponent<Renderer> ().material.mainTexture = CardTextures [11];
			DisplayCard.SetActive (true);
		}

		if (newMessage == "FreeKick") {
			CardText.text = "AI plays direct free kick";
			DisplayCard.GetComponent<Renderer> ().material.mainTexture = CardTextures [2];
			DisplayCard.SetActive (true);
		}

		if (newMessage == "GoalShotLeft") {
			CardText.text = "AI plays goal shot left";
			DisplayCard.GetComponent<Renderer> ().material.mainTexture = CardTextures [7];
			DisplayCard.SetActive (true);
		}

		if (newMessage == "GoalShotRight") {
			CardText.text = "AI plays goal shot right";
			DisplayCard.GetComponent<Renderer> ().material.mainTexture = CardTextures [8];
			DisplayCard.SetActive (true);
		}

		if (newMessage == "BlockLeft") {
			CardText.text = "AI plays goal block left";
			DisplayCard.GetComponent<Renderer> ().material.mainTexture = CardTextures [3];
			DisplayCard.SetActive (true);
		}

		if (newMessage == "BlockRight") {
			CardText.text = "AI plays goal block right";
			DisplayCard.GetComponent<Renderer> ().material.mainTexture = CardTextures [4];
			DisplayCard.SetActive (true);
		}


			//print ("yes");
		System.DateTime now = System.DateTime.Now;
		while(System.DateTime.Now < now.AddMilliseconds(time)){
			//print ("wait");
			yield return null;}


		//Debug.Log ("Yes");
		CardText.text = "";
		DisplayCard.SetActive (false);

		if (newMessage == "Intercept") {
			PublicIntercept ();
		}
		if (newMessage == "Pass") {
			PublicPass ();
		}
		if (newMessage == "GoalShotRight") 
		{
			StartCoroutine(GoalShot ("Right"));
		}
		if (newMessage == "GoalShotLeft") 
		{
			StartCoroutine(GoalShot ("Left"));
		}
		if (newMessage == "BlockRight") 
		{
			StartCoroutine(GoalBlock());
		}
		if (newMessage == "BlockLeft") 
		{
			StartCoroutine(GoalBlock());
		}
		if (newMessage == "FreeKick") 
		{
			StartCoroutine(FreeKick());
		}
		yield return null;
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
		
		//my tessa's stuff
		if (diceRollComplete && stage == stages.RollOff) { // if roll off stage
			diceRollComplete = false;
			stage = stages.CardSelect;
			RollButton.SetActive(false);
			if (Dice1Val > Dice2Val) {
				StartCoroutine (DisplayMessage ("You go first", 2000));
				print("You go first");
				playerTurn = true;
				StartCoroutine(CardPrompt());
			} else {
				StartCoroutine (DisplayMessage ("Computer goes first!", 2000));
				print ("Computer goes first!");
				playerTurn = false;
				StartCoroutine(AITurn());
			}
			
		}
		if(playerTurn && stage == stages.CardSelect) { // if player turn and stage card selection
			// need a delay in the message changes
			stage = stages.etc;
			
		}
	}
	
	IEnumerator AITurn(){

		StartCoroutine(DisplayMessage("AI Turn",1000));
		print ("AI Turn");
		Debug.Log ("AI hand: " + AIHand[0] + " | " + AIHand[1] + " | " + AIHand[2] + " | " + AIHand[3] + " | " + AIHand[4] + " | " + AIHand[5] + " | " + AIHand[6]+ " | " + AIHand[7]+ " | " + AIHand[8]+ " | " + AIHand[9]);
		System.DateTime now = System.DateTime.Now; // change to float 
		while(System.DateTime.Now < now.AddMilliseconds(1000)){yield return null;}
		if(PlayerHasBall )
		{

			if (GoalShotting == 6 && GoalShotSide == "Right")
			{
				if (AIHandContains("BlockRight"))
				{
					StartCoroutine(DisplayCardMessage("BlockRight",3000));
					AIDiscard("BlockRight");
				}
				else if (AIDeckPos < DeckSize)
				{
					StartCoroutine (DisplayMessage ("AI Discards", 1000));
					now = System.DateTime.Now; // change to float 
					while(System.DateTime.Now < now.AddMilliseconds(1000)){yield return null;}

					Debug.Log ("Dicard");

					if (AIDeckPos < DeckSize)
					{
						AIHand[Random.Range (0,5)] = AIDeck[AIDeckPos];
						AIDeckPos++;
					}
					else
					{
						AIHand[Random.Range (0,5)] = "";
					}
					if (AIHandContains("BlockRight"))
					{
						StartCoroutine(DisplayCardMessage("BlockRight",3000));
						AIDiscard("BlockRight");
					}
					else
					{
						StartCoroutine (DisplayMessage ("Player Scores", 3000));

						if (FreeKickCardsLeft > 0)
						{
							Hand[PlayerHandSize] = "FreeKick";
							Cards[PlayerHandSize].GetComponent<Renderer>().material.mainTexture = CardTextures[2];
							PlayerHandSize++;
							Debug.Log("Player earned a card");
							FreeKickCardsLeft --;
						}
						PlayerScore ++;
						UpdateScore ();
						MoveTokenToCenter ();
						GoalShotting = 0;
						EndTurn ();
					}
				}
				else
				{
					Skipped++;
					StartCoroutine (DisplayMessage ("Player Scores", 3000));
					
					if (FreeKickCardsLeft > 0)
					{
						Hand[PlayerHandSize] = "FreeKick";
						Cards[PlayerHandSize].GetComponent<Renderer>().material.mainTexture = CardTextures[2];
						PlayerHandSize++;
						Debug.Log("Player earned a card");
						FreeKickCardsLeft --;
					}
					PlayerScore ++;
					UpdateScore ();
					MoveTokenToCenter ();
					GoalShotting = 0;
					EndTurn ();
				}
			}
			else if (GoalShotting == 6 && GoalShotSide == "Left")
			{
				if (AIHandContains("BlockLeft"))
				{
					StartCoroutine(DisplayCardMessage("BlockLeft",3000));
					AIDiscard("BlockLeft");
				}
				else if (AIDeckPos < DeckSize)
				{
					Debug.Log ("Dicard");
					StartCoroutine (DisplayMessage ("AI Discards", 1000));
					now = System.DateTime.Now; // change to float 
					while(System.DateTime.Now < now.AddMilliseconds(1000)){yield return null;}

					if (AIDeckPos < DeckSize)
					{
						AIHand[Random.Range (0,5)] = AIDeck[AIDeckPos];
						AIDeckPos++;
					}
					else
					{
						AIHand[Random.Range (0,5)] = "";
					}
					if (AIHandContains("BlockLeft"))
					{
						StartCoroutine(DisplayCardMessage("BlockLeft",3000));
						AIDiscard("BlockLeft");
					}
					else
					{
						StartCoroutine (DisplayMessage ("Player Scores", 3000));

						if (FreeKickCardsLeft > 0)
						{
							Hand[PlayerHandSize] = "FreeKick";
							Cards[PlayerHandSize].GetComponent<Renderer>().material.mainTexture = CardTextures[2];
							Debug.Log("Player earned a card");
							PlayerHandSize++;
						}
						PlayerScore ++;
						UpdateScore ();
						MoveTokenToCenter ();
						GoalShotting = 0;
						EndTurn ();
					}
				}
				else
				{
					Skipped++;
					StartCoroutine (DisplayMessage ("Player Scores", 3000));
					
					if (FreeKickCardsLeft > 0)
					{
						Hand[PlayerHandSize] = "FreeKick";
						Cards[PlayerHandSize].GetComponent<Renderer>().material.mainTexture = CardTextures[2];
						Debug.Log("Player earned a card");
						PlayerHandSize++;
					}
					PlayerScore ++;
					UpdateScore ();
					MoveTokenToCenter ();
					GoalShotting = 0;
					EndTurn ();
				}
			}
			else
			{
				if (AIHandContains("FreeKick"))
				{
					StartCoroutine(DisplayCardMessage("FreeKick",3000));
					AIDiscard("FreeKick");
				}
				else if (AIHandContains("Intercept"))
				{
					StartCoroutine(DisplayCardMessage("Intercept",3000));
					AIDiscard("Intercept");
				}
				else if (AIDeckPos < DeckSize)
				{
					Debug.Log ("Dicard");
					StartCoroutine (DisplayMessage ("AI Discards", 1000));
					now = System.DateTime.Now; // change to float 
					while(System.DateTime.Now < now.AddMilliseconds(1000)){yield return null;}

					if (AIDeckPos < DeckSize)
					{
						AIHand[Random.Range (0,5)] = AIDeck[AIDeckPos];
						AIDeckPos++;
					}
					else
					{
						AIHand[Random.Range (0,5)] = "";
					}
					if (AIHandContains("Intercept"))
					{
						StartCoroutine(DisplayCardMessage("Intercept",3000));
						AIDiscard("Intercept");
					}
					else
					{
						EndTurn ();
					}
				}
				else
				{
					Skipped++;
					EndTurn ();
				}
			}

			//PublicIntercept();
		}
		else if (!PlayerHasBall ){//&& AIHandContains("Pass")) {
			if (BallPos > -4)
			{
				if (AIHandContains("Pass"))
				{
					StartCoroutine(DisplayCardMessage("Pass",3000));
					AIDiscard("Pass");
				}
				else if (AIDeckPos < DeckSize)
				{
					Debug.Log ("Discard");
					StartCoroutine (DisplayMessage ("AI Discards", 1000));
					now = System.DateTime.Now; // change to float 
					while(System.DateTime.Now < now.AddMilliseconds(1000)){yield return null;}

					if (AIDeckPos < DeckSize)
					{
						AIHand[Random.Range (0,5)] = AIDeck[AIDeckPos];
						AIDeckPos++;
					}
					else
					{
						AIHand[Random.Range (0,5)] = "";
					}
					if (AIHandContains("Pass"))
					{
						StartCoroutine(DisplayCardMessage("Pass",3000));
						AIDiscard("Pass");
					}
					else
					{
						EndTurn ();
					}
				}
				else
				{
					Skipped++;
					EndTurn ();
				}
				//PublicPass ();
			}
			else
			{
				if (AIHandContains("GoalShotRight") && AIHandContains("GoalShotLeft"))
				{
					if (Random.Range(0,10) > 5)
					{
						StartCoroutine(DisplayCardMessage("GoalShotRight",3000));
						AIDiscard("GoalShotRight");
					}
					else
					{
						StartCoroutine(DisplayCardMessage("GoalShotLeft",3000));
						AIDiscard("GoalShotLeft");
					}
				}
				else if (AIHandContains("GoalShotRight"))
				{
					StartCoroutine(DisplayCardMessage("GoalShotRight",3000));
					AIDiscard("GoalShotRight");

					//(GoalShot ("Right"));
				}
				else if (AIHandContains("GoalShotLeft"))
				{
					StartCoroutine(DisplayCardMessage("GoalShotLeft",3000));
					AIDiscard("GoalShotLeft");
					//StartCoroutine(GoalShot ("Left"));
				}

				else if (AIDeckPos < DeckSize)
				{
					Debug.Log ("Dicard");
					StartCoroutine (DisplayMessage ("AI Discards", 1000));
					now = System.DateTime.Now; // change to float 
					while(System.DateTime.Now < now.AddMilliseconds(1000)){yield return null;}

					if (AIDeckPos < DeckSize)
					{
						AIHand[Random.Range (0,5)] = AIDeck[AIDeckPos];
						AIDeckPos++;
					}
					else
					{
						AIHand[Random.Range (0,5)] = "";
					}

					if (AIHandContains("GoalShotRight"))
					{
						StartCoroutine(DisplayCardMessage("GoalShotRight",3000));
						AIDiscard("GoalShotRight");
					}
					else if (AIHandContains("GoalShotLeft"))
					{
						StartCoroutine(DisplayCardMessage("GoalShotLeft",3000));
						AIDiscard("GoalShotLeft");
					}
					else if (AIHandContains("Pass"))
					{
						StartCoroutine(DisplayCardMessage("Pass",3000));
						AIDiscard("Pass");
					}
					else
					{
						EndTurn ();
					}
				}
				else
				{
					Skipped++;
					EndTurn ();
				}
			}
		}else{EndTurn();};
		
		//StartCoroutine(EndTurn());
	} // added in merge 
	
	bool AIHandContains(string cardNum){
		foreach (string card in AIHand) {
			if(card == cardNum){
				return true;
			}
		}
		return false;
	}

	void AIDiscard(string cardNum){
		Skipped = 0;
		int i;
		for (i = 0; i < AIHandSize; i++) 
		{
			if (AIHand[i] == cardNum)
			{
				if (i < 6)
				{
					if (AIDeckPos < DeckSize)
					{
						AIHand[i] = AIDeck[AIDeckPos];
						AIDeckPos++;
					}
					else
					{
						AIHand[i] = "";
					}
					break;
				}
				else
				{
					AIHand[i] = "";
					AIHandSize--;
					break;
				}
			}
		}
	}

	bool PlayerHandContains(string cardNum){
		foreach (string card in Hand) {
			if(card == cardNum){
				return true;
			}
		}
		return false;
	}
	
	void EndTurn(){
		Discarded = false;
		DiscardButton.interactable = true;

		if (Skipped < 2) {
			//print ("End Turn");
			playerTurn = !playerTurn;
			//print ("Player turn: " + playerTurn);
			if (playerTurn) {
				StartCoroutine (DisplayMessage ("Your turn!", 4000));
				//print("Your turn!");
				StartCoroutine (CardPrompt ());
			} else if (!playerTurn) {
				;
				StartCoroutine (AITurn ());
			}
		} 
		else 
		{
			StartCoroutine (EndRound());
		}
	}

	IEnumerator EndRound()
	{
		System.DateTime d = System.DateTime.Now;
		if (Round < 1) {
			RoundEnding = 0;
			StartCoroutine (DisplayMessage ("Round over", 4000));
			MoveTokenToCenter ();
			RoundEnding = 1;
			d = System.DateTime.Now;
			while (System.DateTime.Now < d.AddMilliseconds(4000)) {
				yield return null;
			}

			while (RoundEnding == 1) {
				yield return null;
			}

			if (Home == false) {
				FlipToken ();
			}

			PlayerHasBall = true;
			Discarded = false;
			Passing = 0;
			Intercepting = 0;
			GoalShotting = 0;
			GoalBlocking = 0;
			DeckPos = 0;
			PlayerHandSize = 6;
			AIHandSize = 6;

			FreeKickCardsLeft = 4;
			Skipped = 0;
			Round++;

			Shuffle ();
			AIShuffle ();
			StartCoroutine (DisplayMessage ("Tap to Roll Off!", 3000));
			RollButton.SetActive (true);
			diceRollComplete = false;

			stage = stages.RollOff;
		} 
		else 
		{
			StartCoroutine (DisplayMessage ("Game over", 2000));
			d = System.DateTime.Now;
			while (System.DateTime.Now < d.AddMilliseconds(2000)) {
				yield return null;
			}

			if (PlayerScore > AIScore)
			{
				StartCoroutine (DisplayMessage ("You win!", 2000));
				d = System.DateTime.Now;
				while (System.DateTime.Now < d.AddMilliseconds(2000)) {
					yield return null;
				}
			}
			else if (PlayerScore < AIScore)
			{
				StartCoroutine (DisplayMessage ("AI wins", 2000));
				d = System.DateTime.Now;
				while (System.DateTime.Now < d.AddMilliseconds(2000)) {
					yield return null;
				}
			}
			else
			{
				StartCoroutine (DisplayMessage ("It's a draw", 2000));
				d = System.DateTime.Now;
				while (System.DateTime.Now < d.AddMilliseconds(2000)) {
					yield return null;
				}
			}

			StartCoroutine (DisplayMessage ("Restarting game ...", 2000));
			d = System.DateTime.Now;
			while (System.DateTime.Now < d.AddMilliseconds(2000)) {
				yield return null;
			}

			Application.LoadLevel(Application.loadedLevel);
		}
	}
	
	IEnumerator CardPrompt(){
		ShowCards ();
		yield return null;
		System.DateTime d = System.DateTime.Now;
		while(System.DateTime.Now < d.AddMilliseconds(2000)){
			yield return null;
		}
		StartCoroutine (DisplayMessage ("Pick a Card!", 3000));
		StartCoroutine (PickCard());
		yield return null;
	}
	
	
	IEnumerator PickCard()
	{
		int waiting = 0;
		int card = 0;
		while (waiting != 1) 
		{
			if (Input.GetKey(ActivateKey))
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) 
				{
					//Debug.Log (hit.collider.gameObject.name);
					if(hit.collider.gameObject.name == "Card1")
					{
						card = 1;
						waiting = 1;
					}
					else if(hit.collider.gameObject.name == "Card2")
					{
						card = 2;
						waiting = 1;
					}
					else if(hit.collider.gameObject.name == "Card3")
					{
						card = 3;
						waiting = 1;
					}
					else if(hit.collider.gameObject.name == "Card4")
					{
						card = 4;
						waiting = 1;
					}
					else if(hit.collider.gameObject.name == "Card5")
					{
						card = 5;
						waiting = 1;
					}
					else if(hit.collider.gameObject.name == "Card6")
					{
						card = 6;
						waiting = 1;
					}
					else if(hit.collider.gameObject.name == "Card7")
					{
						card = 7;
						waiting = 1;
					}
					else if(hit.collider.gameObject.name == "Card8")
					{
						card = 8;
						waiting = 1;
					}
					else if(hit.collider.gameObject.name == "Card9")
					{
						card = 9;
						waiting = 1;
					}
					else if(hit.collider.gameObject.name == "Card10")
					{
						card = 10;
						waiting = 1;
					}
				}
			}
			yield return null;
		}
		
		CardChoice = card - 1;

		message.text = "";
		DisplayCard.GetComponent<Renderer> ().material.mainTexture = Cards [card - 1].GetComponent<Renderer> ().material.mainTexture;
		DisplayCard.SetActive(true);
		CardButtons.SetActive(true);
		
		CheckValidCard ();
	}
	
	void CheckValidCard()
	{
		if (Discarded == true || DeckPos >= DeckSize || CardChoice >= 6) 
		{
			DiscardButton.interactable = false;
		}
		if (Hand [CardChoice] == "Pass" || Hand [CardChoice] == "GoalShotLeft" || Hand [CardChoice] == "GoalShotRight") 
		{
			if (Home == true)
			{
				PlayCardButton.interactable = true;
			}
			else
			{
				PlayCardButton.interactable = false;
			}
		}
		
		if (Hand [CardChoice] == "Intercept" )
		{
			if (Home == false && GoalShotting != 6)
			{
				PlayCardButton.interactable = true;
			}
			else
			{
				PlayCardButton.interactable = false;
			}
		}

		if (Hand [CardChoice] == "FreeKick" )
		{
			if (GoalShotting != 6)
			{
				PlayCardButton.interactable = true;
			}
			else
			{
				PlayCardButton.interactable = false;
			}
		}

		if (Hand [CardChoice] == "BlockLeft" )
		{
			if (Home == false && GoalShotting == 6 && GoalShotSide == "Left")
			{
				PlayCardButton.interactable = true;
			}
			else
			{
				PlayCardButton.interactable = false;
			}
		}

		if (Hand [CardChoice] == "BlockRight") 
		{
			if (Home == false && GoalShotting == 6 && GoalShotSide == "Right")
			{
				PlayCardButton.interactable = true;
			}
			else
			{
				PlayCardButton.interactable = false;
			}
		}
	}

	public void SkipTurn()
	{
		if (GoalShotting == 6) 
		{
			StartCoroutine (DisplayMessage ("AI Scores", 3000));
			if (FreeKickCardsLeft > 0)
			{
				AIHand[AIHandSize] = "FreeKick";
				AIHandSize++;
				Debug.Log("AI earned a card");
				FreeKickCardsLeft --;
			}
			AIScore ++;
			UpdateScore ();
			MoveTokenToCenter ();
			GoalShotting = 0;
		}
		if (Discarded == false)
		{
			Skipped++;
		}
		DisplayCard.SetActive(false);
		CardButtons.SetActive(false);
		HideCards ();
		EndTurn ();
	}


	
	public void PlayCard()
	{
		Skipped = 0;
		DisplayCard.SetActive(false);
		CardButtons.SetActive(false);
		HideCards();
		
		if (Hand [CardChoice] == "Pass") 
		{
			PublicPass();
		}
		
		if (Hand [CardChoice] == "Intercept") 
		{
			PublicIntercept();
		}
		
		if (Hand [CardChoice] == "GoalShotRight") {
			StartCoroutine(GoalShot ("Right"));
		}
		
		if (Hand [CardChoice] == "GoalShotLeft") {
			StartCoroutine(GoalShot ("Left"));
		}

		if (Hand [CardChoice] == "BlockRight") {
			StartCoroutine(GoalBlock());
		}

		if (Hand [CardChoice] == "BlockLeft") {
			StartCoroutine(GoalBlock());
		}

		if (Hand [CardChoice] == "FreeKick") {
			StartCoroutine(FreeKick());
		}
		//StartCoroutine(EndTurn ());
		DiscardCard ();
	}
	
	public void CancelPlayingCard ()
	{
		DisplayCard.SetActive(false);
		CardButtons.SetActive(false);
		StartCoroutine (PickCard());
	}
	
	public void DiscardCard()
	{
		int i;
		Discarded = true;
		Skipped = 0;

		if (CardChoice < 6) {
			if (DeckPos < DeckSize)
			{
				Debug.Log ("No" + DeckPos + DeckSize);
				Hand [CardChoice] = Deck [DeckPos];
				DeckPos++;
			
				if (Hand [CardChoice] == "Pass") {
					Cards [CardChoice].GetComponent<Renderer> ().material.mainTexture = CardTextures [14];
				} else if (Hand [CardChoice] == "GoalShotLeft") {
					Cards [CardChoice].GetComponent<Renderer> ().material.mainTexture = CardTextures [9];
				} else if (Hand [CardChoice] == "GoalShotRight") {	
					Cards [CardChoice].GetComponent<Renderer> ().material.mainTexture = CardTextures [10];
				} else if (Hand [CardChoice] == "Intercept") {	
					Cards [CardChoice].GetComponent<Renderer> ().material.mainTexture = CardTextures [12];
				} else if (Hand [CardChoice] == "BlockLeft") {	
					Cards [CardChoice].GetComponent<Renderer> ().material.mainTexture = CardTextures [5];
				} else {
					Cards [CardChoice].GetComponent<Renderer> ().material.mainTexture = CardTextures [6];
				}
			}
			else
			{
				Debug.Log ("Yes");
				for (i = CardChoice; i < PlayerHandSize; i++)
				{
					Hand [i] = Hand [i + 1];

					if (Hand [i] == "Pass") {
						Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [14];
					} else if (Hand [i] == "GoalShotLeft") {
						Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [9];
					} else if (Hand [i] == "GoalShotRight") {	
						Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [10];
					} else if (Hand [i] == "Intercept") {	
						Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [12];
					} else if (Hand [i] == "FreeKick") {	
						Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [2];
					} else if (Hand [i] == "BlockLeft") {	
						Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [5];
					} else {
						Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [6];
					}
				}
				Hand[PlayerHandSize] = "";
				PlayerHandSize--;
				//ShowCards ();
			}

		} 
		else 
		{
			for (i = CardChoice + 1; i < PlayerHandSize; i++)
			{
				Hand [i] = Hand  [i + 1];

				if (Hand [i] == "Pass") {
					Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [14];
				} else if (Hand [i] == "GoalShotLeft") {
					Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [9];
				} else if (Hand [i] == "GoalShotRight") {	
					Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [10];
				} else if (Hand [i] == "Intercept") {	
					Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [12];
				} else if (Hand [i] == "FreeKick") {	
					Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [2];
				} else if (Hand [i] == "BlockLeft") {	
					Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [5];
				} else {
					Cards [i].GetComponent<Renderer> ().material.mainTexture = CardTextures [6];
				}
			}
			Hand[PlayerHandSize] = "";
			PlayerHandSize--;
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

		for (i = 6; i < 10; i++) 
		{
			Hand[i] = "";
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

		for (i = 6; i < 10; i++) 
		{
			AIHand[i] = "";
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
						UpdateScore();
						StartCoroutine (DisplayMessage("You Score!", 3000));
						if (FreeKickCardsLeft > 0)
						{
							Hand[PlayerHandSize] = "FreeKick";
							Cards[PlayerHandSize].GetComponent<Renderer>().material.mainTexture  = CardTextures[2];
							PlayerHandSize++;
							Debug.Log("Player earned a card");
							FreeKickCardsLeft++;
						}
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
						UpdateScore();
						StartCoroutine (DisplayMessage("AI Scores", 3000));
						if (FreeKickCardsLeft > 0)
						{
							AIHand[AIHandSize] = "FreeKick";
							Debug.Log("AI earned a card");
							AIHandSize++;
							FreeKickCardsLeft--;
						}
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
		EndTurn();
	}



	protected IEnumerator FreeKick()
	{
		int distance = 1;
		int NewPos = 0;
		
		if (playerTurn == true)
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
		
		if (playerTurn == true) 
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

		if (PlayerHasBall == playerTurn)
		{
			if (Dice1Val == 1 || Dice2Val == 1) 
			{
				FlipToken ();
			}
		}
		else
		{
			if (Dice1Val != 1 && Dice2Val != 1) 
			{
				FlipToken ();
			}
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
					if (playerTurn == true)
					{
						PlayerScore ++;
						UpdateScore();
						StartCoroutine (DisplayMessage("You Score!", 3000));
						if (FreeKickCardsLeft > 0)
						{
							Hand[PlayerHandSize] = "FreeKick";
							Cards[PlayerHandSize].GetComponent<Renderer>().material.mainTexture  = CardTextures[2];
							PlayerHandSize++;
							Debug.Log("Player earned a card");
						}
						MoveTokenToCenter();
					}
					else
					{
						FlipToken();
					}
				}
				else if (Dice1Val < Dice2Val)
				{
					if (playerTurn == false)
					{
						AIScore ++;
						UpdateScore();
						StartCoroutine (DisplayMessage("AI Scores", 3000));
						if (FreeKickCardsLeft > 0)
						{
							AIHand[AIHandSize] = "FreeKick";
							AIHandSize++;
							Debug.Log("AI earned a card");
						}
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
		EndTurn();
	}
	
	public void PublicIntercept()
	{
		StartCoroutine (Intercept());
	}
	
	protected IEnumerator Intercept()
	{
		
		if (PlayerHasBall == true)
		{
			AIRollDiceFunc ();
		}
		else
		{
			RollDiceFunc ();
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
		EndTurn();
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
		int i;

		for (i = 0; i < PlayerHandSize; i++) 
		{
			CardPosistion = new Vector3 (3.5f - (7f  / (PlayerHandSize - 1f) * i), 0.1f - 0.01f * i, 7f);
			StartCoroutine (SmoothMovement (CardPosistion, Cards[i]));
		}


//		CardPosistion = new Vector3 (3.5f, 0.06f, 7f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[0]));
		
//		CardPosistion = new Vector3 (2.1f, 0.05f, 7f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[1]));
		
//		CardPosistion = new Vector3 (0.7f, 0.04f, 7f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[2]));
		
//		CardPosistion = new Vector3 (-0.7f, 0.03f, 7f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[3]));
		
//		CardPosistion = new Vector3 (-2.1f, 0.02f, 7f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[4]));
		
//		CardPosistion = new Vector3 (-3.5f, 0.01f, 7f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[5]));
	}
	
	public void HideCards()
	{
		Vector3 CardPosistion;
		int i;

		for (i = 0; i < PlayerHandSize; i++) 
		{
			CardPosistion = new Vector3 (3.5f - (7f  / (PlayerHandSize - 1f) * i), 0.1f - 0.01f * i, 10f);
			StartCoroutine (SmoothMovement (CardPosistion, Cards[i]));
		}

		for (i = PlayerHandSize; i < 10; i++) 
		{
			CardPosistion = new Vector3 (3.5f - (7f  / (7 - 1f) * i), 0.1f - 0.01f * i, 10f);
			StartCoroutine (SmoothMovement (CardPosistion, Cards[i]));
		}


//		CardPosistion = new Vector3 (3.5f, 0.06f, 10f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[0]));
//		
//		CardPosistion = new Vector3 (2.1f, 0.05f, 10f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[1]));
//		
//		CardPosistion = new Vector3 (0.7f, 0.04f, 10f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[2]));
//		
//		CardPosistion = new Vector3 (-0.7f, 0.03f, 10f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[3]));
//		
//		CardPosistion = new Vector3 (-2.1f, 0.02f, 10f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[4]));
//		
//		CardPosistion = new Vector3 (-3.5f, 0.01f, 10f);
//		StartCoroutine (SmoothMovement (CardPosistion, Cards[5]));
	}

	public void HideCard(int Card)
	{
		Vector3 CardPosistion;

		CardPosistion = new Vector3 (3.5f - (7f / (PlayerHandSize - 1f) * Card), 0.1f - 0.01f * Card, 10f);
		StartCoroutine (SmoothMovement (CardPosistion, Cards [Card]));
	}
	
	public void MoveTokenToGoal()
	{
		Vector3 GoalPosistion = new Vector3 (0.0f, 0.1f, 6.5f);
		StartCoroutine (SmoothMovement (GoalPosistion, Token));
	}
	
	public void MoveTokenToCenter()
	{
		BallPos = 0;
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
		
		if (GoalShotting == 1 || GoalShotting == 5) 
		{
			GoalShotting++;
		}

		if (GoalBlocking == 1) 
		{
			GoalBlocking++;
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
			RollButton.SetActive(false);
			
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
		
		//Debug.Log ("" + Dice1Val + " " + Dice2Val);
		
		diceRollComplete = true;
	}
	
	protected IEnumerator GoalShot(string side)
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
		GoalShotting = 1;
		
		while (GoalShotting == 1) 
		{
			yield return null;
		}
		
		//if (Dice1Val != Dice2Val)
		//	distance = Dice1Val + ;
		//else if (Dice1Val < Dice2Val)
		//	distance = Dice2Val;
		//else if (Dice1Val == Dice2Val)
		//	distance = Dice1Val + 1; // not sure about this if 

		if (Dice1Val != Dice2Val)
			distance = Dice1Val + Dice2Val;
		else if (Dice1Val == Dice2Val)
			distance = Dice1Val + Dice2Val + 1;
		
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
		
		GoalShotting = 3;
		
		while (GoalShotting == 3) 
		{
			yield return null;
		}
		
		if (NewPos > -11 && NewPos < 11) 
		{
			FlipToken ();
		}
		else
		{
			GoalShotting = 6;
			GoalShotSide = side;

		}
		//GoalShotting = 0;
		EndTurn();
	}

	protected IEnumerator GoalBlock()
	{
		GoalShotting = 0;
		int distance = 1;
		int NewPos = 0;

		if (PlayerHasBall == true)
		{
			AIRollDiceFunc ();
		}
		else
		{
			RollDiceFunc ();
		}
		GoalBlocking = 1;
		
		while (GoalBlocking == 1) 
		{
			yield return null;
		}
		
		if (Dice1Val != Dice2Val)
			distance = Dice1Val + Dice2Val;
		else if (Dice1Val == Dice2Val)
			distance = Dice1Val + Dice2Val + 1;
		
		if (PlayerHasBall == true) 
		{
			NewPos = BallPos - distance;
		}
		else
		{
			NewPos = BallPos + distance;
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
		
		GoalBlocking = 3;
		
		while (GoalBlocking == 3) 
		{
			yield return null;
		}
		
		if (Dice1Val != 1 && Dice2Val != 1) 
		{
			FlipToken ();
		}

		EndTurn();
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
		if (GoalShotting == 3) 
		{
			GoalShotting++;
		}
		if (GoalBlocking == 3) 
		{
			GoalBlocking++;
		}
		if (RoundEnding == 1) 
		{
			RoundEnding++;
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
	
	void UpdateScore()
	{
		ScoreText.text = "Player: " + PlayerScore + "\nAI: " + AIScore;
	}
}
