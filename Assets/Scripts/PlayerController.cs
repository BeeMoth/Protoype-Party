using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public GameObject board;
	public GameObject dice;
	public Transform tile;

	public int diceRoll;
	public int coins;

	private bool moving = false;
	private float timeCheck;
	private float speed = .02f;
	private int tilesMoved;
	private GameObject diceSpawn;

	private BoardState boardScript;
	private DiceSpin diceScript;
	private TileMovement tileScript;

    void Start () {
		// Starts in the starting position of the current board
		boardScript = board.GetComponent<BoardState>();
		transform.position = boardScript.startLocation;

		// Uses the start tile to find the first tile that the player can move to
		GameObject start = GameObject.FindWithTag("Start");
		TileMovement startScript = start.GetComponent<TileMovement>();
		tile = startScript.nextTile;

		// Resets coins, as the Unity player keeps them from game to game. Need to look into
		coins = 0;
	}
	
    void Update () {
		if (!isLocalPlayer)
		{
			return;
		}

		// Ensures the player can't roll another dice while moving
		if (!moving)
		{
			// Spawns a dice
			if (timeCheck == 0 && Input.GetKeyDown(KeyCode.Mouse0))
			{
				diceSpawn = Instantiate(dice);
				NetworkServer.Spawn(diceSpawn);
				timeCheck = Time.time;
			}

			// Stops the dice and outputs a number on player input, the player then moves that many tiles
			if (Time.time - timeCheck >= .06f && Input.GetKeyDown(KeyCode.Mouse0))
			{
				{
					DiceHit();
					Destroy(diceSpawn);
				}
			}
		}

		// While moving is true, move towards the next tile
		if (moving)
		{
			Movement();
		}
    }

	// Stops the dice from spinning and gives a random number between 1 and 6
	void DiceHit()
	{
		diceScript = dice.GetComponent<DiceSpin>();
		diceScript.Stop();
		diceRoll = diceScript.roll;

		moving = true;
		timeCheck = Time.time;
	}

	// Moves the player an amount of tiles equal to their dice roll
	void Movement()
	{
		transform.position = Vector3.Lerp(transform.position, tile.position + new Vector3(0, 0.67f, 0), speed);
		// Grabs the pointer to the next tile every two seconds, so that movement isn't instant
		if (Time.time - timeCheck >= 2)
		{
			tileScript = tile.GetComponent<TileMovement>();
			tile = tileScript.nextTile;
			timeCheck = Time.time;
			tilesMoved++;
			int remainingDice = diceRoll - tilesMoved;
			Debug.Log(remainingDice + "(" + diceRoll + ")");
			// Stops movement once the player has moved the given number of tiles
			if (tilesMoved == diceRoll)
			{
				moving = false;
				tileScript.Effect();
				tilesMoved = 0;
				diceRoll = 0;
				timeCheck = 0;
			}
		}
	}
}
