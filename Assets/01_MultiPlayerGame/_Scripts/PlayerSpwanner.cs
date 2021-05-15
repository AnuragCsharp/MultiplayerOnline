using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerSpwanner : PunBehaviour
{
	public static PlayerSpwanner intance;

	public GameObject PlayerPrefab;

	private GameObject _player;

	private void Awake()
	{
		intance = this;
	}

	private void Start()
	{
		if (PhotonNetwork.connected)
		{
			SpwanPlayer();
		}

	}


	public void SpwanPlayer()
	{
		Transform SpwanPoint = SpwanManager.instance.GetSpwanPoint();

		_player =  PhotonNetwork.Instantiate(PlayerPrefab.name, SpwanPoint.position , SpwanPoint.rotation,0);
	}
}