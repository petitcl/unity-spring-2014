using UnityEngine;
using System.Collections;


public class Character_Motor : MonoBehaviour {

	public static Character_Motor Instance;

	public Vector3 MoveVector;
	public Vector3 SlideVector;

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

	private void AlignCharacterToCameraDirection() {
		Vector3 lookAt = Camera.main.transform.forward;
		lookAt.y = 0;
		Quaternion lookAtRotation = Quaternion.LookRotation(lookAt);
		this.gameObject.transform.rotation = lookAtRotation * MODEL_3DSMAX;
	}

	private void ProcessMotion() {
		this.MoveVector = this.transform.TransformDirection(this.MoveVector);
		if (this.MoveVector.magnitude > 1) {
			this.MoveVector.Normalize();
		}
		this.MoveVector *= this.maxSpeed * Time.deltaTime;
		this.MoveVector.y = Character_Manager.Instance.VerticalVelocity;

		this.Slide();
		this.ApplyGravity();
		Character_Manager.CharacterControllerComponent.Move(this.MoveVector);
	}

	private void Slide() {
		if (!Character_Manager.CharacterControllerComponent.isGrounded) {
			Debug.Log("goodbye!");
			return;
		}
		this.SlideVector = Vector3.zero;
		RaycastHit hit;
		if (!Physics.Raycast(this.transform.position + Vector3.up,
		                    Vector3.down,
		                    out hit)) {
			Debug.Log("goodbye! nothing to do");
			return;
		}
		//Debug.Log(hit.normal);
		this.SlideVector = hit.normal.normalized;
//		Vector3 hitNormal = hit.normal;
//
//		moveDirection = Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
//		Vector3.OrthoNormalize (hitNormal, moveDirection);

		float mag = this.SlideVector.y;
		this.SlideVector.y = 0;
		Debug.Log(mag);
		if (hit.normal.y < 0.9f) {
			if (mag < 0.7f) {
				this.MoveVector = this.SlideVector * 10.0f * Time.deltaTime;
			} else {
				this.MoveVector += this.SlideVector * 10.0f * Time.deltaTime;
			}
		}
	}

	private void ApplyGravity() {
		if (!Character_Manager.CharacterControllerComponent.isGrounded) {
			if (Mathf.Abs(MoveVector.y) < this.TerminalVelocity) {
				MoveVector.y += Character_Manager.Instance.Gravity * Time.deltaTime;
			}
		} else if (!Character_Manager.Instance.isJumping()) {
			MoveVector.y = -1;
		}
	}

	public void Jump() {
		if (Character_Manager.CharacterControllerComponent.isGrounded) {
			Character_Manager.Instance.VerticalVelocity = this.JumpImpulse;
		}
	}

}
