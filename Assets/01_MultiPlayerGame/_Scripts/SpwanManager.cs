using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanManager : MonoBehaviour
{
	public Transform[] spwanPoints;

	public static SpwanManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		foreach (var Spwanpoints_item in spwanPoints)
		{
			Spwanpoints_item.gameObject.SetActive(false);
		}
	}

	public Transform GetSpwanPoint()
	{
		return spwanPoints[(Random.Range(0, spwanPoints.Length))];
	}
}
