using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BoardState : NetworkBehaviour {

	public GameObject activePlayer;
	public Vector3 startLocation = new Vector3(-8.19f, 0.67f, -8.46f);
}
