using UnityEngine;
using System.Collections;


public class Character_Motor : MonoBehaviour {

	public static Character_Motor Instance;

	public Vector3 MoveVector;
	public Vector3 SlideVector;

	//Max gravity velocity
	public float TerminalVelocity = 10f;

	public float	ForwardSpeedLimit = 10f;
	public float	BackwardSpeedLimit = 4f;
	public float	StrafingSpeedLimit = 12f;
	public float	SlidingSpeedLimit = 10f;

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
//		this.gameObject.transform.rotation = lookAtRotation;
	}

	private void ProcessMotion() {
		this.MoveVector = this.transform.TransformDirection(this.MoveVector);
		if (this.MoveVector.magnitude > 1) {
			this.MoveVector.Normalize();
		}
		this.MoveVector *= this.SpeedLimit() * Time.deltaTime;
		this.MoveVector.y = Character_Manager.Instance.VerticalVelocity;

		this.Slide();
		this.ApplyGravity();
		Character_Manager.CharacterControllerComponent.Move(this.MoveVector);
	}

	private void Slide() {
		if (!Character_Manager.CharacterControllerComponent.isGrounded) {
			return;
		}
		this.SlideVector = Vector3.zero;
		RaycastHit hit;
		if (!Physics.Raycast(this.transform.position + Vector3.up,
		                    Vector3.down,
		                    out hit)) {
			return;
		}
		this.SlideVector = hit.normal.normalized;
//		Vector3 hitNormal = hit.normal;
//
//		moveDirection = Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
//		Vector3.OrthoNormalize (hitNormal, moveDirection);

		float mag = this.SlideVector.y;
//		this.SlideVector.y = 0;
		Vector3 realSlideVector = new Vector3(this.SlideVector.x, 0.0f, this.SlideVector.z);
		if (hit.normal.y < 0.9f) {
			if (mag < 0.7f) {
				this.MoveVector = realSlideVector * this.SpeedLimit() * Time.deltaTime;
			} else {
				this.MoveVector += realSlideVector * this.SpeedLimit() * Time.deltaTime;
			}
		}
	}

	private float SpeedLimit() {
		switch (Animation_Manager.Instance.CharacterMotionState) {
		case Animation_Manager.MotionStateList.Backward:
			return BackwardSpeedLimit;
		case Animation_Manager.MotionStateList.LeftBackward:
			return BackwardSpeedLimit;
		case Animation_Manager.MotionStateList.RightBackward:
			return BackwardSpeedLimit;

		case Animation_Manager.MotionStateList.Forward:
			return ForwardSpeedLimit;
		case Animation_Manager.MotionStateList.LeftForward:
			return ForwardSpeedLimit;
		case Animation_Manager.MotionStateList.RightForward:
			return ForwardSpeedLimit;

		case Animation_Manager.MotionStateList.Left:
			return StrafingSpeedLimit;
		case Animation_Manager.MotionStateList.Right:
			return StrafingSpeedLimit;

		default:
			if (this.IsSliding()) {
				return this.SlidingSpeedLimit;
			} else {
				return 0.0f;
			}
		}
	}

	private bool IsSliding() {
		return (this.SlideVector.y > 0.0f);
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
