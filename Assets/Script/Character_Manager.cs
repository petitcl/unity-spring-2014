using UnityEngine;
using System.Collections;

//@script RequireComponent(CharacterMotor)
public class Character_Manager : MonoBehaviour {

	public static CharacterController CharacterControllerComponent;

	public static Character_Manager Instance;

	public float deadZone = 0.01f;

	public float Gravity = -9.81f;

	public float VerticalVelocity;

	private bool inputUpdated;

	public bool InputUpdated {
		get {
			return inputUpdated;
		}
	}

	// Use this for initialization
	private void Start() {
	
	}

	private void Awake() {
		Instance = this;

		Character_Manager.CharacterControllerComponent = GetComponent<CharacterController>();
		if (CharacterControllerComponent == null) {
			//Handle error when no CharacterController Component
		}
		Camera_Manager.InitialCameraCheck();
	}
	
	// Update is called once per frame
	private void Update() {
//		if (GetComponent<Camera>() != null) {
		this.ControllerInput();
			this.ActionInput();
			Character_Motor.Instance.ControlledUpdate();
//		}
	}

	private void ControllerInput() {
		float v = Input.GetAxis("Vertical");
		float h = Input.GetAxis("Horizontal");

		inputUpdated = false;

		this.VerticalVelocity = Character_Motor.Instance.MoveVector.y;
		Character_Motor.Instance.MoveVector = Vector3.zero;
		//change how the input is fetched to deal with "normal" 
		if (v > deadZone || v < -deadZone) {
			Character_Motor.Instance.MoveVector.y = -v;
			inputUpdated = true;
		}
		if (h > deadZone || h < -deadZone) {
			Character_Motor.Instance.MoveVector.x = h;
			inputUpdated = true;
		}
	}

	private void ActionInput() {
		if (Input.GetButton("Jump")) {
			this.DelegateJump();
		}
	}

	public bool isJumping() {
		return Input.GetButton("Jump");
	}

	private void DelegateJump() {
		Character_Motor.Instance.Jump();
	}


}
