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



	public Vector3 SmoothedCameraPosition;
	public Vector3 CameraVelocity;

	public float MouseX;
	public float MouseY;

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

		float clampedMouseWheel = Mathf.Clamp(this.MouseWheel, this.MinDist, this.MaxDist);
		float newMouseWheel = Mathf.SmoothDamp(this.OldMouseWheel, clampedMouseWheel, ref this.CameraVelocity.z, 0.3f);

		this.OldMouseWheel = newMouseWheel;
		this.SmoothCameraAxis();
		Vector3 positionVector = this.CreatePositionVector(this.SmoothedCameraPosition.x, this.SmoothedCameraPosition.y, newMouseWheel);
		this.ApplyCameraPosition(positionVector);
	}

	public void	SmoothCameraAxis() {
		this.SmoothedCameraPosition.x = Mathf.SmoothDamp(this.SmoothedCameraPosition.x, this.MouseX, ref this.CameraVelocity.x, 0.3f);
		this.SmoothedCameraPosition.y = Mathf.SmoothDamp(this.SmoothedCameraPosition.y, this.MouseY, ref this.CameraVelocity.y, 0.3f);
	}

	public Vector3 CreatePositionVector(float mouseX, float mouseY, float dist) {
		Quaternion cameraRotation = Quaternion.Euler(mouseY, mouseX, 0.0f);
		Vector3 cameraPosition = this.TargetLookAt.transform.position + (cameraRotation * Vector3.forward * -dist);
		return cameraPosition;
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
