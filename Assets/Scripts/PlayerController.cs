using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public GameObject board;
	public GameObject dice;

	public int coins;

	private float timeCheck;
	private GameObject diceSpawn;
	private int diceRoll;
	private Transform nextTile;
	private Vector3 nextTilePosition;
	private bool isMoving = false;
	private bool isBranching = false;
	private int tilesMoved;

	private BoardState boardScript;
	private DiceSpin diceScript;
	private TileMovement tileScript;

	void Start () {
		// Uses the start tile to find where the player stars and the first tile they can move to
		GameObject start = GameObject.FindWithTag("Start");
		transform.position = start.transform.position + new Vector3(0, 0.5f, 0);
		tileScript = start.GetComponent<TileMovement>();
		nextTile = tileScript.nextTile;
	}
	
    void Update () {
		if (!isLocalPlayer) {
			return; }

		// Ensures the player can't roll another dice while moving
		if (!isMoving)
		{
			// Spawns a dice on the client and server
			if (timeCheck == 0 && Input.GetKeyDown(KeyCode.Mouse0))
			{
				diceSpawn = Instantiate(dice);
				CmdDiceSpawn();
				ResetTimeCheck();
			}

			// Stops the dice and outputs a number on player input, then destroying the dice
			if (Time.time - timeCheck >= .06f && Input.GetKeyDown(KeyCode.Mouse0))
			{
				{
					DiceHit();
					Destroy(diceSpawn, 1.5f);
				}
			}
		}

		// If the player is currently on a branching tile, wait for input to decide which tile they will move to
		if (isBranching)
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				nextTilePosition = tileScript.nextTile.position;
				nextTile = tileScript.nextTile;
				BranchingComplete();
			}
			if (Input.GetKeyDown(KeyCode.D))
			{
				nextTilePosition = tileScript.nextTileBranch.position;
				nextTile = tileScript.nextTileBranch;
				BranchingComplete();
			}
		}

		// If moving and not deciding on a branching tile, move towards the next tile
		if (isMoving && !isBranching) {
			Movement(); }
    }

	// Spawns a dice on the server
	[Command]
	void CmdDiceSpawn() {
		NetworkServer.Spawn(diceSpawn); }

	// Stops the dice from spinning and gives a random number between 1 and 6, letting the player move
	void DiceHit()
	{
		diceScript = diceSpawn.GetComponent<DiceSpin>();
		diceScript.Stop();
		diceRoll = diceScript.roll;
		nextTilePosition = transform.position;
		isMoving = true;
		ResetTimeCheck();
	}

	// Moves the player an amount of tiles equal to their dice roll
	void Movement()
	{
		// Gets the position of the next tile if the variable which holds tile positions is at a default state
		if (nextTilePosition == transform.position) {
			nextTilePosition = tileScript.nextTile.position; }
		// Converts the distance between the tile and the tile it points to into a speed for the player to use
		float tileDistance = Vector3.Distance(nextTilePosition, tileScript.transform.position);
		float speed = tileDistance * .01f;
		transform.position = Vector3.Lerp(transform.position, nextTile.position + new Vector3(0, 0.5f, 0), speed);
		// Grabs the pointer to the next tile every few seconds, so that movement isn't instant
		if (Time.time - timeCheck >= tileDistance * .55f)
		{
			tileScript = nextTile.GetComponent<TileMovement>();
			// Checks to see if the next tile is branching or not
			if (!tileScript.nextTileBranch) {
				nextTile = tileScript.nextTile; }
			else {
				isBranching = true; }
			nextTilePosition = transform.position;
			ResetTimeCheck();
			// If the tile moved to is an actual tile, up the number of tiles moved
			if (tileScript.movementTick)
			{
				tilesMoved++;
				int remainingDice = diceRoll - tilesMoved;
				Debug.Log(remainingDice + "(" + diceRoll + ")");
			}
			// Stops movement once the player has moved the given number of tiles
			if (tilesMoved == diceRoll)
			{
				isMoving = false;
				tileScript.Effect();
				tilesMoved = 0;
				diceRoll = 0;
				timeCheck = 0;
			}
		}
	}

	void ResetTimeCheck() {
		timeCheck = Time.time; }

	void BranchingComplete()
	{
		isBranching = false;
		ResetTimeCheck();
	}
}
