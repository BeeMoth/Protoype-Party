using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMovement : MonoBehaviour {

	public Transform nextTile;
	public GameObject board;
	public float tileDistance;
	public float playerSpeed;
	public bool movementTick;
	public bool jump;
	public int coinChange;

	private BoardState boardScript;
	private PlayerController playerScript;

	// Converts the distance between this tile and the tile it points to into a speed for the player to use
	private void Start()
	{
		tileDistance = Vector3.Distance(nextTile.position, transform.position);
		playerSpeed = tileDistance * .01f;
	}

	// Activates the different effects of this tile
	public void Effect()
	{
		boardScript = board.GetComponent<BoardState>();
		playerScript = boardScript.activePlayer.GetComponent<PlayerController>();
		if(coinChange != 0) {
			Coins(coinChange); }
	}

	// Alters the amount of coins a player has
	public void Coins(int change)
	{
		playerScript = boardScript.activePlayer.GetComponent<PlayerController>();
		playerScript.coins = playerScript.coins + change;
		if(playerScript.coins < 0) {
			playerScript.coins = 0; }
		Debug.Log(playerScript.coins);
	}
}