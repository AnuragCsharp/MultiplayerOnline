using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class MatchManager : PunBehaviour , IPunCallbacks 
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

	void Update()
	{

	}

	public void OnEvent(EventData photonEvent)
	{

	}

	private void OnEnable()
	{
		PhotonNetwork.OnEventCall
		
	}

	private void OnDisable()
	{
		
	}
}
