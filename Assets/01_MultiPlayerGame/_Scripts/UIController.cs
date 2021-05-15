using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
	public TMP_Text overHeaterMessage;

	public Slider weaponTempSlider;


	public static UIController instance;

	private void Awake()
	{
	 instance = this;
		
	}
}
