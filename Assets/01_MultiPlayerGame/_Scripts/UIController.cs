using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
	public TMP_Text overHeaterMessage;

	public Slider weaponTempSlider;

	public Slider healthSlider;

	public static UIController instance;

	public GameObject deathScreen;

	public TMP_Text deathNote;

	public TMP_Text reSpwanningText;

	private void Awake()
	{
	 instance = this;
		
	}
}
