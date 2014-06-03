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
	public int	MaxCameraChecks = 5;
	public float CameraCheckStep = 0.3f;
	public	float	UnobstructedSmoothTime = 0.3f;
	public	float	ObstructedSmoothTime = 2.0f;

	private Vector2 InitialMouseAxis;
	private	float	ClosestDitanceToCharacter = 0.0f;
	private	float	SmoothTimeSwitch;
	private	float	CameraDistanceBeforeObstruction;
	private	Vector3	CameraPositionBeforeObstruction;

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		this.InitialCameraPosition();
	}

	private void InitialCameraPosition() {
		this.MouseWheel = 10.0f;
		this.SmoothCameraPosition();
		this.SmoothTimeSwitch = this.UnobstructedSmoothTime;
		this.CameraDistanceBeforeObstruction = this.MouseWheel;
	}

	private	void LateUpdate() {
		this.VerifyUserMouseInput();

		int cameraCheckCount = 0;
		while (this.ObstructedCameraCheck(cameraCheckCount)) {
			++cameraCheckCount;
		}
	}

	private	void VerifyUserMouseInput() {
		//if right button pressed, do nothing (DEBUG)
		if (Input.GetButton("Fire2")) {
			this.MouseX += Input.GetAxis("Mouse X") * this.MouseSens;
			this.MouseY += -Input.GetAxis("Mouse Y") * this.MouseSens;
			this.MouseY = Helper.CameraClamp(this.MouseY, -10, 80);
		}
		float	tmpScrollWheel = Input.GetAxis("Mouse ScrollWheel") * this.MouseWheelSpeed;
		this.MouseWheel += tmpScrollWheel;
		//Clamp mouseY
		if (tmpScrollWheel != 0.0f) {
			this.CameraDistanceBeforeObstruction = this.MouseWheel;
			this.SmoothTimeSwitch = this.UnobstructedSmoothTime;
		}
		this.SmoothCameraPosition();
	}

	public void SmoothCameraPosition() {
		this.EvaluateCameraDistanceBeforeObstruction();
		this.MouseWheel = Mathf.Clamp(this.MouseWheel, this.MinDist, this.MaxDist);
		float newMouseWheel = Mathf.SmoothDamp(this.OldMouseWheel, this.MouseWheel, ref this.MouseWheelVelocity, this.SmoothTimeSwitch);
		//Handle Obstruction
		//If distance back to original
//		Debug.Log("Delta:" + (this.CameraDistanceBeforeObstruction - newMouseWheel));
		if (Mathf.Abs(this.CameraDistanceBeforeObstruction - newMouseWheel) < 1.0f) {
			this.SmoothTimeSwitch = this.UnobstructedSmoothTime;
		}
		this.OldMouseWheel = newMouseWheel;
		Vector3 positionVector = this.CreatePositionVector(this.MouseX, this.MouseY, newMouseWheel);
		Vector3 smoothedPosition = this.SmoothCameraAxis(positionVector);
		this.ApplyCameraPosition(smoothedPosition);
	}

	public Vector3	SmoothCameraAxis(Vector3 desiredPosition) {
		Vector3 smoothedPosition;
		smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref this.AxisVelocity, this.SmoothTimeSwitch);
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

	private	void EvaluateCameraDistanceBeforeObstruction() {
		if (this.MouseWheel < this.CameraDistanceBeforeObstruction) {
			this.CameraPositionBeforeObstruction = 
				this.CreatePositionVector(this.MouseX, this.MouseY, this.CameraDistanceBeforeObstruction);
			float newDist = this.CameraCollisionPointsCheck(this.TargetLookAt.transform.position, this.CameraPositionBeforeObstruction);
			if (newDist == -1.0f) {
				this.MouseWheel = this.CameraDistanceBeforeObstruction;
			}
		}
	}

	private void CollisionPointCheck(Vector3 start, Vector3 end, ref float newDistance) {
		RaycastHit hitInfo;
		
		if (Physics.Linecast(start, end, out hitInfo)) {
			if (hitInfo.collider.tag != "Player") {
				if (newDistance == -1.0f || newDistance > hitInfo.distance) {
					newDistance = hitInfo.distance;
				}
			}
		}
	}

	public float CameraCollisionPointsCheck(Vector3 targetLookAtPosition, Vector3 cameraPositionAfterSmoothing) {
		float newDistance = -1.0f;
		Vector3 nearClipPlanePosition = (cameraPositionAfterSmoothing - targetLookAtPosition).normalized * Camera.main.nearClipPlane;
		Vector3 cameraBackBuffer = cameraPositionAfterSmoothing + nearClipPlanePosition;
		Helper.ClipPlaneStruct nearPlane = Helper.FindNearClipPlanePositions ();

		Debug.DrawLine(cameraBackBuffer, targetLookAtPosition, Color.red);
		
		Debug.DrawLine (nearPlane.LowerLeft, nearPlane.LowerRight, Color.white);
		Debug.DrawLine (nearPlane.LowerRight, nearPlane.UpperRight, Color.white);
		Debug.DrawLine (nearPlane.UpperRight, nearPlane.UpperLeft, Color.white);
		Debug.DrawLine (nearPlane.UpperLeft, nearPlane.LowerLeft, Color.white);
		
		Debug.DrawLine (nearPlane.LowerLeft, targetLookAtPosition, Color.white);
		Debug.DrawLine (nearPlane.LowerRight, targetLookAtPosition, Color.white);
		Debug.DrawLine (nearPlane.UpperRight, targetLookAtPosition, Color.white);
		Debug.DrawLine (nearPlane.UpperLeft, targetLookAtPosition, Color.white);

		this.CollisionPointCheck(targetLookAtPosition, cameraBackBuffer, ref newDistance);
		this.CollisionPointCheck(targetLookAtPosition, nearPlane.LowerRight, ref newDistance);
		this.CollisionPointCheck(targetLookAtPosition, nearPlane.UpperRight, ref newDistance);
		this.CollisionPointCheck(targetLookAtPosition, nearPlane.UpperLeft, ref newDistance);
		this.CollisionPointCheck(targetLookAtPosition, nearPlane.LowerLeft, ref newDistance);

		return newDistance;
	}

	public	bool ObstructedCameraCheck(int cameraCheckCount) {
		if (cameraCheckCount < this.MaxCameraChecks) {
			bool cameraObstructionBool = false;
			this.ClosestDitanceToCharacter = this.CameraCollisionPointsCheck(this.TargetLookAt.transform.position, Camera.main.transform.position);
			cameraObstructionBool = (this.ClosestDitanceToCharacter != -1.0f);
			if (cameraObstructionBool) {
				this.SmoothTimeSwitch = this.ObstructedSmoothTime;
				this.MouseWheel -= this.CameraCheckStep;
				SmoothCameraPosition();
			}
			return (cameraObstructionBool);
		} else {
			this.SmoothTimeSwitch = this.ObstructedSmoothTime;
			this.MouseWheel = this.ClosestDitanceToCharacter + Camera.main.nearClipPlane;
			SmoothCameraPosition();
			return false;
		}
	}
}
