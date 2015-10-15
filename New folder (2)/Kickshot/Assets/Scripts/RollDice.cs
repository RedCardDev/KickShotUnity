using UnityEngine;
using System.Collections;

public class RollDice : MonoBehaviour {

	public GameObject Dice;

	public Object Dice1;
	public Object Dice2;

	public void RollDiceFunc()
	{
		Vector3 SpawnPosistion = new Vector3 (0.2f, 7f, 5f);
		Quaternion SpawnRotation = Quaternion.identity;
		Dice1 = Instantiate (Dice, SpawnPosistion, SpawnRotation);

		Vector3 SpawnPosistion2 = new Vector3 (-0.2f, 7f, 5f);
		Quaternion SpawnRotation2 = Quaternion.identity;
		Dice2 = Instantiate (Dice, SpawnPosistion2, SpawnRotation2);

		//angle = Vector3.Angle(transform.up, Vector3.up);
	}
}
