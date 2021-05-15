using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{

	public TMP_Text buttonText;

	private RoomInfo _info;

	public void SetButtonDetails(RoomInfo inputinfo)
	{
		_info = inputinfo;

		buttonText.text = _info.name;
	}

	public void OpenRoom()
	{
		Launcher.instance.JoinRoom(_info);
	}
}
