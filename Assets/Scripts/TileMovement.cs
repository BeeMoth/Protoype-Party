using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMovement : MonoBehaviour {

	public Transform nextTile;
	public GameObject board;
	public int coinChange;

	private BoardState boardScript;
	private PlayerController playerScript;

	// Activates the different effects of this tile
	public void Effect()
	{
		boardScript = board.GetComponent<BoardState>();
		playerScript = boardScript.activePlayer.GetComponent<PlayerController>();
		if(coinChange != 0)
		{
			Coins(coinChange);
		}
	}

	public void Coins(int change)
	{
		playerScript = boardScript.activePlayer.GetComponent<PlayerController>();
		playerScript.coins = playerScript.coins + change;
		if(playerScript.coins < 0)
		{
			playerScript.coins = 0;
		}
		Debug.Log(playerScript.coins);
	}
}