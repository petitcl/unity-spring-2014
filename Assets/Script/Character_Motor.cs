using UnityEngine;
using System.Collections;


public class Character_Motor : MonoBehaviour {

	public static Character_Motor Instance;

	public Vector3 MoveVector;

	//Max gravity velocity
	public float TerminalVelocity = 10f;

	public float maxSpeed = 1f;

	public float JumpImpulse = 10f;

	// Use this for initialization
	private void Awake () {
		Instance = this;
	}

	// Update is called once per frame
	public void ControlledUpdate() {
		AlignCharacterToCameraDirection();
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
//		Vector3 view = -(Camera_Manager.Instance.TargetLookAt.transform.position - Camera.main.transform.position);
//		this.transform.LookAt(this.transform.position + view, Vector3.forward);

		//Camera TargetLookAt vector
//		Vector3 eulerAngles = new Vector3(
//			this.transform.rotation.eulerAngles.x,
//			this.transform.rotation.eulerAngles.y,
//			Camera.main.transform.rotation.eulerAngles.y);
//		Quaternion rotation3Dsmax = Quaternion.Euler(eulerAngles);
//
//		this.transform.rotation = rotation3Dsmax;
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
