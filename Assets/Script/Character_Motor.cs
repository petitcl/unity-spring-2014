using UnityEngine;
using System.Collections;


public class Character_Motor : MonoBehaviour {

	public static Character_Motor Instance;

	public Vector3 MoveVector;

	//Max gravity velocity
	public float TerminalVelocity = 10f;

	public float maxSpeed = 1f;

	public float JumpImpulse = 10f;

	public static Quaternion MODEL_3DSMAX = Quaternion.Euler(-90, 0, 0);

	// Use this for initialization
	private void Awake () {
		Instance = this;
	}

	// Update is called once per frame
	public void ControlledUpdate() {
		if (Character_Manager.Instance.InputUpdated) {
			AlignCharacterToCameraDirection();
		}
		ProcessMotion();
	}

	private void ProcessMotion() {
		this.MoveVector = this.transform.TransformDirection(this.MoveVector);
		if (this.MoveVector.magnitude > 1) {
			this.MoveVector.Normalize();
		}
		this.MoveVector *= this.maxSpeed * Time.deltaTime;
		this.MoveVector.y = Character_Manager.Instance.VerticalVelocity;

		this.ApplyGravity();
		Character_Manager.CharacterControllerComponent.Move(this.MoveVector);
	}

	private void AlignCharacterToCameraDirection() {
		Camera cam = Camera.main;
		GameObject cha = this.gameObject;

		Vector3 lookAt = cam.transform.forward;
		lookAt.y = 0;
		Quaternion lookAtRotation = Quaternion.LookRotation(lookAt);
		cha.transform.rotation = lookAtRotation * MODEL_3DSMAX;
	}

	private void ApplyGravity() {
		if (!Character_Manager.CharacterControllerComponent.isGrounded) {
			if (Mathf.Abs(MoveVector.y) < this.TerminalVelocity) {
				MoveVector.y += Character_Manager.Instance.Gravity * Time.deltaTime;
			}
		} else if (!Character_Manager.Instance.isJumping()) {
			MoveVector.y = 0;
		}
	}

	public void Jump() {
		if (Character_Manager.CharacterControllerComponent.isGrounded) {
			Character_Manager.Instance.VerticalVelocity = this.JumpImpulse;
		}
	}

}
