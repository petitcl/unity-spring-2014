using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animation_Manager : MonoBehaviour {

	public static Animation_Manager Instance;
	
	public enum MotionStateList {
		Stationary,
		Forward,
		Backward,
		Left,
		Right,
		LeftForward,
		RightForward,
		LeftBackward,
		RightBackward
	}
	
	
	public enum AnimationStateList {
		Idle,
		Run,
		RunBackwards,
		StrafeRunLeft,
		StrafeRunRight,
		StrafeBackLeft,
		StrafeBackRight
	}

	public	delegate void	AfterAnimationStateAction();

	public bool Left = false;
	public bool Right = false;
	public bool Forward = false;
	public bool Backward = false;

	public MotionStateList CharacterMotionState = MotionStateList.Stationary;
	public AnimationStateList CharacterAnimationState = AnimationStateList.Idle;


	private	Dictionary<AnimationStateList, AfterAnimationStateAction>	AfterAnimationActions;

	private void Awake() {
		Instance = this;

		//setup here after animations actions
		this.AfterAnimationActions = new Dictionary<AnimationStateList, AfterAnimationStateAction>();
		this.AfterAnimationActions[AnimationStateList.Idle] = this.AnimationAfterIdleState;
		this.AfterAnimationActions[AnimationStateList.Run] = this.AnimationAfterRunState;
		this.AfterAnimationActions[AnimationStateList.RunBackwards] = this.AnimationAfterRunBackwardsState;
		this.AfterAnimationActions[AnimationStateList.StrafeRunLeft] = this.AnimationAfterStrafeRunLeftState;
		this.AfterAnimationActions[AnimationStateList.StrafeRunRight] = this.AnimationAfterStrafeRunRightState;
		this.AfterAnimationActions[AnimationStateList.StrafeBackLeft] = this.AnimationAfterStrafeBackLeftState;
		this.AfterAnimationActions[AnimationStateList.StrafeBackRight] = this.AnimationAfterStrafeBackRightState;

	}
		
	public void CurrentMotionState() {

		this.Left = false;
		this.Right = false;
		this.Forward = false;
		this.Backward = false;

		Vector3 moveVector = Character_Motor.Instance.MoveVector;
		if (Character_Manager.Instance.handle3DSmax) {
			if (moveVector.y < 0) {
				this.Forward = true;
			} else if (moveVector.y > 0) {
				this.Backward = true;
			}
		} else {
			if (moveVector.z > 0) {
				this.Forward = true;
			} else if (moveVector.z < 0) {
				this.Backward = true;
			}
		}
	
		if (moveVector.x > 0) {
			this.Right = true;
		} else if (moveVector.x < 0) {
			this.Left = true;
		}

		if (this.Left) {
			if (this.Forward) {
				this.CharacterMotionState = MotionStateList.LeftForward;
			} else if(this.Backward) {
				this.CharacterMotionState = MotionStateList.LeftBackward;
			} else {
				this.CharacterMotionState = MotionStateList.Left;
			}
		} else if (this.Right) {
			if (this.Forward) {
				this.CharacterMotionState = MotionStateList.RightForward;
			} else if(this.Backward) {
				this.CharacterMotionState = MotionStateList.RightBackward;
			} else {
				this.CharacterMotionState = MotionStateList.Right;
			}
		} else if (this.Forward) {
			this.CharacterMotionState = MotionStateList.Forward;
		} else if (this.Backward) {
			this.CharacterMotionState = MotionStateList.Backward;
		}

		if (!this.Left && !this.Right && !this.Forward && !this.Backward) {
			this.CharacterMotionState = MotionStateList.Stationary;
		}
		Debug.Log(this.CharacterMotionState);
	}

	public void CurrentAnimationState() {
		//check if player is dead
		//check if jumping/falling/landing
		//check if action (ie: using/climbing)

		switch (this.CharacterMotionState) {
		case MotionStateList.Stationary:
			this.CharacterAnimationState = AnimationStateList.Idle;
			break;
		case MotionStateList.Forward:
			this.CharacterAnimationState = AnimationStateList.Run;
			break;
		case MotionStateList.Backward:
			this.CharacterAnimationState = AnimationStateList.RunBackwards;
			break;
		case MotionStateList.Left:
			this.CharacterAnimationState = AnimationStateList.StrafeRunLeft;
			break;
		case MotionStateList.Right:
			this.CharacterAnimationState = AnimationStateList.StrafeRunRight;
			break;
		case MotionStateList.LeftForward:
			this.CharacterAnimationState = AnimationStateList.StrafeRunLeft;
			break;
		case MotionStateList.RightForward:
			this.CharacterAnimationState = AnimationStateList.StrafeRunRight;
			break;
		case MotionStateList.LeftBackward:
			this.CharacterAnimationState = AnimationStateList.StrafeBackLeft;
			break;
		case MotionStateList.RightBackward:
			this.CharacterAnimationState = AnimationStateList.StrafeBackRight;
			break;
		default:
			this.CharacterAnimationState = AnimationStateList.Idle;
			break;
		}
		Debug.Log(this.CharacterAnimationState);
	}

	public void		ProcessAnimationState() {
		if (!this.AfterAnimationActions.ContainsKey(this.CharacterAnimationState)) {
			return;
		}
		AfterAnimationStateAction action = this.AfterAnimationActions[this.CharacterAnimationState];
		action();
	}

	public	void	AnimationAfterIdleState() {
		animation.CrossFade("Idle");
	}
	public	void	AnimationAfterRunState() {
		animation.CrossFade("Run");
	}
	public	void	AnimationAfterRunBackwardsState() {
		animation.CrossFade("RunBackwards");
	}
	public	void	AnimationAfterStrafeRunLeftState() {
		animation.CrossFade("StrafeRunLeft");
	}
	public	void	AnimationAfterStrafeRunRightState() {
		animation.CrossFade("StrafeRunRight");
	}
	public	void	AnimationAfterStrafeBackLeftState() {
		animation.CrossFade("StrafeBackLeft");
	}
	public	void	AnimationAfterStrafeBackRightState() {
		animation.CrossFade("StrafeBackRight");
	}

}
