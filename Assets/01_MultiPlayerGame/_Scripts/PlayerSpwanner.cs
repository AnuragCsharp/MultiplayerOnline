using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerSpwanner : PunBehaviour
{
	public static PlayerSpwanner instance;

	public GameObject PlayerPrefab;

	private GameObject _player;

	public GameObject deathEffectFx;

	public float respwanTime =3f;

	private void Awake()
	{
		instance = this;
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


	//this is Being called by Player Controller TakeDamage and on the Network for Individual Player
	public void Die(string Damager)
	{
		

		UIController.instance.deathNote.text = "You Were Killed By " + Damager;

		//PhotonNetwork.Destroy(_player);

		//SpwanPlayer();

		if (_player !=null)
		{
			StartCoroutine(DieCoroutine());
		}
	}


	public IEnumerator DieCoroutine()
	{
		PhotonNetwork.Instantiate(deathEffectFx.name, _player.transform.position, Quaternion.identity, 0);

		PhotonNetwork.Destroy(_player);

		UIController.instance.deathScreen.SetActive(true);

		yield return new WaitForSeconds(respwanTime);

		UIController.instance.deathScreen.SetActive(false);

		SpwanPlayer();
	}
}