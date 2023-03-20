﻿using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TestPlayerController : NetworkBehaviour {

	private Transform _camera;
    public bool canRotateCamera = true;

	[SerializeField] private float _mouseSensitivity;

	private float _cameraPitch;

    public GameObject AIChatCanvas;

	[SerializeField] public float _walkSpeed;
	[SerializeField] public float _runSpeed;
    [SerializeField] public string WalkAnimationName = "HumanoidWalk";
    [SerializeField] public string WalkBackwardAnimationName = "Walking_Backward";
    [SerializeField] public string WalkLeftAnimationName = "Walk_Left";
    [SerializeField] public string WalkRightAnimationName = "Walk_Right";
    [SerializeField] public string IdleAnimationName = "Fishing_Idle";
    [SerializeField] public string SittingAnimationName = "Sitting_Idle";

    private CharacterController _cController;
    public Vector3 SpawnPos;
    bool DrivingBoat = false;
	private void Start() {
        SpawnPos = transform.position;
		_camera = GetComponentInChildren<Camera>().transform;

		if (!isLocalPlayer) {
			Destroy(_camera.gameObject);
			Destroy(_cController);
			Destroy(this);
		}

		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;

		_cController = GetComponent<CharacterController>();
	}

	private void UpdateMouse() {

        if (AIChatCanvas.activeInHierarchy) { canRotateCamera = false; } else { canRotateCamera = true; }
        if(canRotateCamera == true)
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            mouseDelta /= Screen.dpi;
            mouseDelta *= 100f;

            _cameraPitch -= mouseDelta.y * _mouseSensitivity;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 32f);

            transform.Rotate(_mouseSensitivity * mouseDelta.x * Vector3.up);
            _camera.localEulerAngles = Vector3.right * _cameraPitch;
        }
	}

	private void UpdateMovement() {

        if (AIChatCanvas.activeInHierarchy) { return; }
        if (AIChatCanvas.gameObject.activeInHierarchy)
        {
            InputField inputField = AIChatCanvas.GetComponentInChildren<InputField>();
            if (inputField != null && inputField.gameObject.activeInHierarchy)
            {
                inputField.Select();
                inputField.ActivateInputField();
            }
        }
       
        if (!DrivingBoat)
        {
            Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            inputDir.Normalize();

            Vector3 velocity = transform.forward * inputDir.y + transform.right * inputDir.x;
            velocity *= Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed;
            velocity.y = Physics.gravity.y / 10;
            _cController.Move(velocity * Time.fixedDeltaTime);
        }
	}

    private void Update()
    {
        if (isLocalPlayer)
        {
            bool isWalkingForward = false;
            bool isWalkingBackward = false;
            bool isWalkingLeft = false;
            bool isWalkingRight = false;

            if (DrivingBoat) { return; }
            //Walk Forward
            if (Input.GetButton("Walk") & isWalkingBackward == false & isWalkingLeft == false & isWalkingRight == false)
            {
                isWalkingForward = true;
                this.GetComponent<Animator>().Play(WalkAnimationName);
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                isWalkingForward = false;
                this.GetComponent<Animator>().Play(IdleAnimationName);
            }
            //Walk Backward
            if (Input.GetButton("Walk_Backward") & isWalkingForward == false & isWalkingLeft == false & isWalkingRight == false)
            {
                isWalkingBackward = true;
                this.GetComponent<Animator>().Play(WalkBackwardAnimationName);
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                isWalkingBackward = false;
                this.GetComponent<Animator>().Play(IdleAnimationName);
            }
            //Walk Left
            if (Input.GetButton("Walk_Left") & isWalkingForward == false & isWalkingBackward == false & isWalkingRight == false)
            {
                isWalkingLeft = true;
                this.GetComponent<Animator>().Play(WalkLeftAnimationName);
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                isWalkingLeft = false;
                this.GetComponent<Animator>().Play(IdleAnimationName);
            }
            //Walk Right
            if (Input.GetButton("Walk_Right") & isWalkingForward == false & isWalkingBackward == false & isWalkingLeft == false)
            {
                isWalkingRight = true;
                this.GetComponent<Animator>().Play(WalkRightAnimationName);
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                isWalkingRight = false;
                this.GetComponent<Animator>().Play(IdleAnimationName);
            }
            if (transform.position.y <= -2)
            {
                // Teleport the player to the spawn position
                transform.position = SpawnPos;
                _cController.slopeLimit = 180f;
            }
        }
    }
    public void SetStateSitting(bool TrueFalse)
    {
        if (isLocalPlayer)
        {
            DrivingBoat = TrueFalse;
            this.GetComponent<Animator>().Play(SittingAnimationName);
        }
    }
    private void FixedUpdate() {
		UpdateMouse();
		UpdateMovement();
	}
}
