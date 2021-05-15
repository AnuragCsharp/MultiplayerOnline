using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class Launcher : PunBehaviour
{
	public static Launcher instance;

	private void Awake()
	{
		instance = this;
	}

	public GameObject loadingScreen;

	public GameObject menuButton;

	public TMP_Text LoadingText;

	public string gameVersion;

	public GameObject CreateRoomScreen;

	public TMP_InputField RoomNameInputFieldtext;

	public GameObject roomScreen;

	public TMP_Text roomNameText,playerNameText;

	private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

	public GameObject ErrorScreen;

	public TMP_Text ErrorText;

	public GameObject roomBrowserScreen;

	public RoomButton theRoomButton;

	private List<RoomButton> allRoomButtons = new List<RoomButton>();

	public GameObject NameInputScreen;

	public TMP_InputField NameInput;

	private bool _hasSetNickName;

	public string LevelName;

	public GameObject StartButton;

	public GameObject RoomTestButton;






	private void Start()
	{
		CloseMenu();

		loadingScreen.SetActive(true);
		LoadingText.text = "Connectin to Network...";

		PhotonNetwork.ConnectUsingSettings(gameVersion);


#if UNITY_EDITOR


		RoomTestButton.SetActive(true);


#endif
	}





	private void CloseMenu()
	{
		loadingScreen.SetActive(false);
		menuButton.SetActive(false);
		CreateRoomScreen.SetActive(false);
		roomScreen.SetActive(false);
		ErrorScreen.SetActive(false);
		roomBrowserScreen.SetActive(false);
		NameInputScreen.SetActive(false);
	}

	//accessing the Photon's VIrtual Method to Connec to Master Server
	public override void OnConnectedToMaster()
	{		

		PhotonNetwork.JoinLobby();

		PhotonNetwork.automaticallySyncScene = true;

		LoadingText.text = "Joining Lobby..";
	}

	public override void OnJoinedLobby()
	{
		CloseMenu();
		menuButton.SetActive(true);

		PhotonNetwork.player.NickName = Random.Range(0, 1000).ToString();

		if (!_hasSetNickName)
		{
			CloseMenu();
			NameInputScreen.SetActive(true);

			if (PlayerPrefs.HasKey("PlayerName"))
			{
				NameInput.text = PlayerPrefs.GetString("PlayerName");
			}
		}

		else
		{
			PhotonNetwork.player.NickName = PlayerPrefs.GetString("PlayerName");
		}
	}

	public void OpenRoomCreateScreen()
	{
		CloseMenu();

		CreateRoomScreen.SetActive(true);
	}

	public void CreateRoom()
	{
		if (!string.IsNullOrEmpty(RoomNameInputFieldtext.text))
		{
			RoomOptions Roptions = new RoomOptions();

			Roptions.MaxPlayers = 8;

			PhotonNetwork.CreateRoom(RoomNameInputFieldtext.text);

			CloseMenu();

			LoadingText.text = "Creating Room....";

			loadingScreen.SetActive(true);
		}
	}

	public override void OnJoinedRoom()
	{
		CloseMenu();

		roomScreen.SetActive(true);

		roomNameText.text = PhotonNetwork.room.Name;

		ListAllPlayers();

		if (PhotonNetwork.isMasterClient)
		{
			StartButton.SetActive(true);
		}
		else
		{
			StartButton.SetActive(false);
		}
	}


	private void ListAllPlayers()
	{
		
		foreach (TMP_Text Player in allPlayerNames)
		{
			Destroy(Player.gameObject);
		}
		allPlayerNames.Clear();


		PhotonPlayer[] _Players = PhotonNetwork.playerList;
		
		for (int i = 0; i < _Players.Length; i++)
		{
			TMP_Text newPlayerLabel = Instantiate(playerNameText, playerNameText.transform.parent);

			newPlayerLabel.text = _Players[i].NickName;

			newPlayerLabel.gameObject.SetActive(true);

			allPlayerNames.Add(newPlayerLabel);
		}

		
	}


	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		TMP_Text newPlayerLabel = Instantiate(playerNameText, playerNameText.transform.parent);

		newPlayerLabel.text = newPlayer.NickName;

		newPlayerLabel.gameObject.SetActive(true);

		allPlayerNames.Add(newPlayerLabel);
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		ListAllPlayers();
	}

	public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
	{
		ErrorText.text = "Failed to Create Room:" + "Try Different Name";
		CloseMenu();
		ErrorScreen.SetActive(true);
	}

	public void CloseErrorScreen()
	{
		CloseMenu();
		menuButton.SetActive(true);
	}


	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
		CloseMenu();
		LoadingText.text = "Leaving the Room";
		loadingScreen.SetActive(true);

	}

	public override void OnLeftRoom()
	{
		CloseMenu();
		menuButton.SetActive(true);
	}

	public void OpenRoomBrowser()
	{

		CloseMenu();
		roomBrowserScreen.SetActive(true);
	}

	public void CloseRoomBrowser()
	{
		CloseMenu();
		roomBrowserScreen.SetActive(false);
	}

	public override void OnReceivedRoomListUpdate()
	{

		Debug.Log("The Recieve Room list is being Called ");
		foreach (RoomButton rb in allRoomButtons)
		{
			Destroy(rb.gameObject);
		}

		allRoomButtons.Clear();

		theRoomButton.gameObject.SetActive(false);
		var roomList = PhotonNetwork.GetRoomList();

		for (int i = 0; i < roomList.Length; i++)
		{

			RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);

			newButton.SetButtonDetails(roomList[i]);

			newButton.gameObject.SetActive(true);

			allRoomButtons.Add(newButton);

		}



	}


	public void JoinRoom(RoomInfo inputinfo)
	{
		PhotonNetwork.JoinRoom(inputinfo.Name);
		CloseMenu();
		LoadingText.text = "Joining Room";
		loadingScreen.SetActive(true);
		
	}

	public void SetNickName()
	{
		if (!string.IsNullOrEmpty(NameInput.text))
		{
			PhotonNetwork.player.NickName = NameInput.text;

			PlayerPrefs.SetString("PlayerName", NameInput.text);

			CloseMenu();

			menuButton.SetActive(true);

			_hasSetNickName = true;
		}
	}

	public void StartGame()
	{
		PhotonNetwork.LoadLevel(LevelName);
	}


	public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		if (PhotonNetwork.isMasterClient)
		{
			StartButton.SetActive(true);
		}
		else
		{
			StartButton.SetActive(false);
		}
	}


	public void QuickJoin()
	{

		RoomOptions Roptions = new RoomOptions();

		Roptions.MaxPlayers = 8;

		PhotonNetwork.CreateRoom("Test");
		CloseMenu();
		LoadingText.text = "Craeting Room";
		loadingScreen.SetActive(true);

	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
