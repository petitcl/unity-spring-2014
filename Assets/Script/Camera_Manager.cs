using UnityEngine;
using System.Collections;

public class Camera_Manager : MonoBehaviour {

	public static Camera_Manager Instance;

	public GameObject TargetLookAt;

	public float RotationSpeed = 0.20f;

	public float MaxDist = 30.0f;
	public float MinDist = 2.0f;
	public float MouseWheelVelocity;
	public float MouseWheelSpeed = 10.0f;


	public float MouseX;
	public float MouseXVelocity;
	public float MouseY;
	public float MouseYVelocity;
	public float MouseSens = 2.0f;
	public float MouseWheel;
	public float OldMouseWheel;

	private Vector2 InitialMouseAxis;

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		Vector3 v = Camera.main.transform.position - this.TargetLookAt.transform.position;

		//special case: targetLookAt and camera are in the same position
		if (v == Vector3.zero) {
			v = Vector3.up * this.MinDist;
		}
		float dist = Mathf.Clamp(v.magnitude, this.MinDist, this.MaxDist); //resize vector between targetLooktAndCamer
		v = v.normalized * dist;
		this.InitialCameraPosition(this.TargetLookAt.transform.position + v);
	}

	private void InitialCameraPosition(Vector3 newPos){
		Camera.main.transform.position = newPos;
		this.MouseWheel = 10.0f;
	}

	private	void LateUpdate() {
		this.VerifyUserMouseInput();
	}

	private	void VerifyUserMouseInput() {
		//if right button pressed, do nothing (DEBUG)
		if (Input.GetButton("Fire2")) {
			return;
		}
		this.MouseX += Input.GetAxis("Mouse X") * this.MouseSens;
		this.MouseY += Input.GetAxis("Mouse Y") * this.MouseSens;
		this.MouseWheel += Input.GetAxis("Mouse ScrollWheel") * this.MouseWheelSpeed;
		//Clamp mouseY
		this.MouseY = Helper.CameraClamp(this.MouseY, -50, 80);
		SmoothCameraPosition();

	}

	public void SmoothCameraPosition() {
//		float newMouseY =  Mathf.SmoothDamp (this.MouseY, mouseY, ref this.MouseYVelocity,0.3f);
//		this.MouseY = newMouseY;
//
//		float newMouseX =  Mathf.SmoothDamp (this.MouseX, mouseX, ref this.MouseXVelocity,0.3f);
//		this.MouseX = newMouseX;

		float clampedMouseWheel = Mathf.Clamp(this.MouseWheel, this.MinDist, this.MaxDist);

		float newMouseWheel = Mathf.SmoothDamp(this.OldMouseWheel, clampedMouseWheel, ref this.MouseWheelVelocity, 0.3f);
		this.OldMouseWheel = newMouseWheel;
		Vector3 positionVector = this.CreatePositionVector(this.MouseX, this.MouseY, newMouseWheel);
		Vector3 smoothedPosition = this.SmoothCameraAxis(positionVector);
		this.ApplyCameraPosition(smoothedPosition);
	}

	public Vector3	SmoothCameraAxis(Vector3 desiredPosition) {
		Vector3 smoothedPosition = Vector3.up;
		return desiredPosition;
	}

	public Vector3 CreatePositionVector(float mouseX, float mouseY, float dist) {
		Quaternion	rotation = Quaternion.Euler(mouseY, mouseX, 0.0f);
		return this.TargetLookAt.transform.position + (rotation * Vector3.forward * -dist);
	}

	public void	ApplyCameraPosition(Vector3 newPos) {
		Camera.main.transform.position = newPos;
		Camera.main.transform.LookAt(this.TargetLookAt.transform);
	}

	//
	//Static functions
	//
	public static void InitialCameraCheck() {

		//initialize targetLookAt


		//initialize camera
		if (Camera.main == null) {
			GameObject tmpCamera = new GameObject("MainCamera");
			tmpCamera.AddComponent<Camera>();
			tmpCamera.AddComponent<Camera_Manager>();
			tmpCamera.tag = "MainCamera";
			Debug.LogWarning("Couldn't find MainCamera, creating it");
		} else {
			Camera.main.gameObject.AddComponent<Camera_Manager>();
		}

		GameObject tmpTarget = GameObject.Find("targetLookAt");
		if (tmpTarget == null) {
			tmpTarget = new GameObject("targetLookAt");
			tmpTarget.transform.position = Vector3.zero;
			Debug.LogWarning("Couldn't find targetLookAt, creating it");
		}
		Camera_Manager.Instance.TargetLookAt = tmpTarget;
	}
	

}
