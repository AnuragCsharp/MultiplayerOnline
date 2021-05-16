using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.SceneManagement;

public class MatchManager : PunBehaviour
{
    public static MatchManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (!PhotonNetwork.connected)
		{
			SceneManager.LoadScene(0); //Main Menu to be Loaded if it is not connected to Network;
		}
	}
}
