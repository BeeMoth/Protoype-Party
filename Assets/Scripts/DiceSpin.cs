﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DiceSpin : NetworkBehaviour {

	[SyncVar]
	public int roll;

	private float tick;
    private int changeRate = 3;
	private bool spinning = true;
	private bool direction = true;

    void Update()
    {
        if(spinning)
        {
            Spin();
			Change();
        }
	}

	// Spins in one direction or the other, depending on the given direction bool
	// Currently needs the transition to be smoothed out
	void Spin()
	{
		if (direction == true)
		{
			transform.Rotate(new Vector3(Random.Range(150, 450), Random.Range(150, 450), Random.Range(150, 450))
			* Time.deltaTime);
		}
		else
		{
			transform.Rotate(new Vector3(Random.Range(-150, -450), Random.Range(-150, -450),
			Random.Range(-150, -450)) * Time.deltaTime);
		}
	}

	// Compares the current time to the previously noted time
	// If the given amount of time between a new roation has passed, randomly change the rotation again
	// When changing rotation, note the time of doing so for further comparisons
	void Change()
	{
		if (Time.time - tick >= changeRate)
		{
			if (direction == true)
			{
				direction = false;
			}
			else
			{
				direction = true;
			}
			tick = Time.time;
		}
	}

	// Stops the dice from spinning, currently returning a random number between 1 and 6
	// todo: Use the min distance between the Sides GameObjects and the Compare one to determine what was rolled
	public void Stop()
	{
		spinning = false;
		roll = Random.Range(1, 7);
	}
}