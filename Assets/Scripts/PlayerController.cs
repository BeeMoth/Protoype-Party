using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public GameObject board;
	public GameObject dice;
	public Transform tile;

	public int diceRoll;
	public int coins;

	private bool moving = false;
	private float timeCheck;
	private int tilesMoved;
	private GameObject diceSpawn;

	private BoardState boardScript;
	private DiceSpin diceScript;
	private TileMovement tileScript;


	void Start () {
		// Uses the start tile to find where the player stars and the first tile they can move to
		GameObject start = GameObject.FindWithTag("Start");
		tileScript = start.GetComponent<TileMovement>();
		tile = tileScript.nextTile;
		transform.position = start.transform.position + new Vector3(0, 0.5f, 0);
	}
	
    void Update () {
		if (!isLocalPlayer)
		{
			return;
		}

		// Ensures the player can't roll another dice while moving
		if (!moving)
		{
			// Spawns a dice on the client and server
			if (timeCheck == 0 && Input.GetKeyDown(KeyCode.Mouse0))
			{
				diceSpawn = Instantiate(dice);
				CmdDiceSpawn();
				timeCheck = Time.time;
			}

			// Stops the dice and outputs a number on player input, the player then moves that many tiles
			if (Time.time - timeCheck >= .06f && Input.GetKeyDown(KeyCode.Mouse0))
			{
				{
					DiceHit();
					Destroy(diceSpawn, 1.5f);
				}
			}
		}

		// While moving is true, move towards the next tile
		if (moving)
		{
			Movement();
		}
    }

	// Spawns a dice on the server
	[Command]
	void CmdDiceSpawn()
	{
		NetworkServer.Spawn(diceSpawn);
	}

	// Stops the dice from spinning and gives a random number between 1 and 6
	void DiceHit()
	{
		diceScript = diceSpawn.GetComponent<DiceSpin>();
		diceScript.Stop();
		diceRoll = diceScript.roll;

		moving = true;
		timeCheck = Time.time;
	}

	// Moves the player an amount of tiles equal to their dice roll
	void Movement()
	{
		float speed = tileScript.playerSpeed;
		// Increases the speed a jump is performed at
		if (tileScript.jump) {
			speed = speed * 1.5f;
		}
		transform.position = Vector3.Lerp(transform.position,
		tile.position + new Vector3(0, 0.5f, 0), speed);
		// Grabs the pointer to the next tile every few seconds, so that movement isn't instant
		if (Time.time - timeCheck >= tileScript.tileDistance * .65f)
		{
			tileScript = tile.GetComponent<TileMovement>();
			tile = tileScript.nextTile;
			timeCheck = Time.time;
			// If the tile moved to is an actual tile, up the number of tiles moved
			if (tileScript.movementTick) {
				tilesMoved++; }
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
