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
	public float MouseY;
	public float MouseYVelocity;
	public float MouseSens = 5.0f;
	Vector3 AxisVelocity;
	public float MouseWheel;
	public float OldMouseWheel;

	private Vector2 InitialMouseAxis;

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		this.InitialCameraPosition();
	}

	private void InitialCameraPosition(){
		this.MouseWheel = 10.0f;
		SmoothCameraPosition();
	}

	private	void LateUpdate() {
		this.VerifyUserMouseInput();
	}

	private	void VerifyUserMouseInput() {
		//if right button pressed, do nothing (DEBUG)
		if (Input.GetButton("Fire2")) {
			this.MouseX += Input.GetAxis("Mouse X") * this.MouseSens;
			this.MouseY += -Input.GetAxis("Mouse Y") * this.MouseSens;
			this.MouseY = Helper.CameraClamp(this.MouseY, -10, 80);
		}
		this.MouseWheel += Input.GetAxis("Mouse ScrollWheel") * this.MouseWheelSpeed;
		//Clamp mouseY
		SmoothCameraPosition();
	}

	public void SmoothCameraPosition() {
		this.MouseWheel = Mathf.Clamp(this.MouseWheel, this.MinDist, this.MaxDist);

//		float newMouseWheel= clampedMouseWheel;
		float newMouseWheel = Mathf.SmoothDamp(this.OldMouseWheel, this.MouseWheel, ref this.MouseWheelVelocity, 0.3f);
		this.OldMouseWheel = newMouseWheel;
		Vector3 positionVector = this.CreatePositionVector(this.MouseX, this.MouseY, newMouseWheel);
		Vector3 smoothedPosition = this.SmoothCameraAxis(positionVector);
		this.ApplyCameraPosition(smoothedPosition);
	}

	public Vector3	SmoothCameraAxis(Vector3 desiredPosition) {
		Vector3 smoothedPosition;
		smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref this.AxisVelocity, 0.3f);
		return smoothedPosition;
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
