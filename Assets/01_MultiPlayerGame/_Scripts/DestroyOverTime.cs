using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
	public float lifeTime = 1.5f;

		private void Start()
	{
		Destroy(gameObject, lifeTime);
	}
}
