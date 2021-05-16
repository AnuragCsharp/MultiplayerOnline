using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PlayerController : PunBehaviour
{
	public Transform _viewPoint;
	private float _verticalRotation;
	private Vector2 mouseInput;

	public float mouseSenitivity = 1f;

	public float moveSpeed = 5f;
	public float runSpeed = 8f;

	private float activeMoveSpeed;

	public bool invertLook;

	private Vector3 _moveDirection;
	private Vector3 _movement;

	public CharacterController characCol;

	private Camera _Cam;

	public float jumpForce = 12f, gravitymod = 2.5f;

	public Transform groundCheckPoint;
	private bool isGrounded;
	public LayerMask groudnLayers;

	public GameObject bulltImpact;

	//public float timeBetweenShots = 0.1f;

	public float shotCounter;
	public float muzzleDispayTime;
	private float _muzzleCounter;


	public float maxHeat = 10f, /*heatPerShot = 1f, */ coolRate = 4f, overHeatCoolRate = 5f;

	private float _heatCounter;

	private bool _overHeated;

	public Gun[] allGuns;

	private int _selectedGun;


	public GameObject playerHitImpact;

	



	#region For Windows Build
#if UNITY_STANDALONE_WIN

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;

		_Cam = Camera.main;

		UIController.instance.weaponTempSlider.maxValue = maxHeat;

		SwitchGuns();
		
		//Without Network Spwan use below else comment it
		
		/*
		Transform NewPosition = SpwanManager.instance.GetSpwanPoint();
		this.transform.position = NewPosition.position;
		this.transform.rotation = NewPosition.rotation; */
	}



	private void Update()
	{
		if (photonView.isMine)
		{
			// Get the Mouse axis and assign to Vector2 MouseINput
			mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

			//Apply the rotation to player Based on Mouse Inputs X axis
			this.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

			//Look up and down based on Player VIewPoint + Clamp the look to 60 Degrees..

			_verticalRotation += mouseInput.y; //to Avoid Wierd Clamp Behaviour we are setting a vertical Roation

			_verticalRotation = Mathf.Clamp(_verticalRotation, -60f, 60f);

			_viewPoint.rotation = Quaternion.Euler(-_verticalRotation, _viewPoint.rotation.eulerAngles.y, _viewPoint.rotation.eulerAngles.z);


			//TO move the PLayer
			_moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

			if (Input.GetKey(KeyCode.LeftShift)) // to check if player is running or moving 
			{
				activeMoveSpeed = runSpeed;
			}
			else
			{
				activeMoveSpeed = moveSpeed;
			}

			float yVel = _movement.y;


			_movement = ((transform.forward * _moveDirection.z) + (transform.right * _moveDirection.x)).normalized * activeMoveSpeed;


			//If only player not in groud make the Y value increase to act like gravity
			if (!characCol.isGrounded)
			{
				_movement.y = yVel;
			}

			//to check if player is on ground layer or not
			isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groudnLayers);

			if (Input.GetButtonDown("Jump") && isGrounded)
			{
				_movement.y = jumpForce;
			}

			_movement.y += Physics.gravity.y * Time.deltaTime * gravitymod;

			characCol.Move(_movement * Time.deltaTime);


			if (allGuns[_selectedGun].Muzzle.activeInHierarchy)
			{
				_muzzleCounter -= Time.deltaTime;

				if (_muzzleCounter <= 0)
				{
					//Deactivate the Muzzle
					allGuns[_selectedGun].Muzzle.SetActive(false);
				}

			}


			//TO Shoot the Different Guns with Different Burst Mode

			if (!_overHeated)
			{



				if (Input.GetMouseButtonDown(0))
				{
					Shoot();
					
				}

				if (Input.GetMouseButton(0) && allGuns[_selectedGun].isAutomatic)
				{
					shotCounter -= Time.deltaTime;

					

					if (shotCounter <= 0)
					{


						GameObject Go = GameObject.Find("weapon .001");
						Go.GetComponent<Animator>().SetBool("Fire", true);
										

						Shoot();

					

					}

				}


				_heatCounter -= coolRate * Time.deltaTime;

				
			}

			else
			{
				_heatCounter -= overHeatCoolRate * Time.deltaTime;
				

				if (_heatCounter <= 0)
				{
					
					UIController.instance.overHeaterMessage.gameObject.SetActive(false);

					_overHeated = false;
				}
			}

			if (_heatCounter <= 0)
			{
				_heatCounter = 0;
			}

			UIController.instance.weaponTempSlider.value = _heatCounter;


			//Gun Selecter
			if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
			{

				_selectedGun++;

				if (_selectedGun >= allGuns.Length)
				{
					_selectedGun = 0;
				}
				SwitchGuns();
			}


			else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
			{
				_selectedGun--;

				if (_selectedGun < 0)
				{
					_selectedGun = allGuns.Length - 1;
				}
				SwitchGuns();
			}

			//Swich Guns Using Number Keys as well
			for (int i = 0; i < allGuns.Length; i++)
			{
				if (Input.GetKeyDown((i + 1).ToString()))
				{
					_selectedGun = i;
					SwitchGuns();
				}
			}


			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Cursor.lockState = CursorLockMode.None;
			}
			else if (Cursor.lockState == CursorLockMode.None)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Cursor.lockState = CursorLockMode.Locked;
				}
			}

			
		}
	}

	[PunRPC] //Remote Procedure Call -- This function will call on everyMachine
	public void DealDamage()
	{
		Debug.Log("This is RPC Call");	
	}



	private void LateUpdate() // will get call after Update
	{
		//to make sure the Camera move smoothly ---
		//Photonvirw.ismine is for individual player to have their own COntroll
		if (photonView.isMine)
		{
			_Cam.transform.position = _viewPoint.position;
			_Cam.transform.rotation = _viewPoint.rotation;
		}

		

	}


	void SwitchGuns()
	{
		foreach (var Guns in allGuns)
		{
			Guns.gameObject.SetActive(false);
		}

		allGuns[_selectedGun].gameObject.SetActive(true);

		allGuns[_selectedGun].Muzzle.SetActive(false);
	}


	private void Shoot()
	{
		//shoot a ray from Camera view port
		Ray ray = _Cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));

		ray.origin = _Cam.transform.position;

		if (Physics.Raycast( ray ,out RaycastHit hit))
		{
			if (hit.collider.gameObject.CompareTag("Player"))
			{
				Debug.Log("HIt " + hit.collider.gameObject.GetPhotonView().name);
				PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity,0);

				hit.collider.gameObject.GetPhotonView().RPC("DealDamage", PhotonTargets.All );
			}
			else
			{
				GameObject _BulletImpactObject = Instantiate(bulltImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));

				Destroy(_BulletImpactObject, 5f);
			}

		 
		}

		shotCounter = allGuns[_selectedGun].timeBetweenShot;

		_heatCounter += allGuns[_selectedGun].hearPerShot;

		if (_heatCounter >= maxHeat)
		{
			_heatCounter = maxHeat;


			UIController.instance.overHeaterMessage.gameObject.SetActive(true);

			GameObject.Find("weapon .001").GetComponent<Animator>().SetBool("Fire", false);

			_overHeated = true;

			GameObject Go = GameObject.Find("weapon .001");
			Go.GetComponent<Animator>().SetBool("Fire", false);
			
		
		}

		allGuns[_selectedGun].Muzzle.SetActive(true);
		_muzzleCounter = muzzleDispayTime;

		
	}

#endif
	#endregion


}
