using UnityEngine;
using System.Collections;

public static class Helper {
	public struct ClipPlaneStruct
	{
		public Vector3 UpperLeft;
		public Vector3 UpperRight;
		public Vector3 LowerLeft;
		public Vector3 LowerRight;
	}

	public static float	CameraClamp(float angle, float min, float max) {
		int iangle = (int)angle;
		iangle = iangle % 360;
		return Mathf.Clamp(iangle, min, max);
	}

	public static ClipPlaneStruct FindNearClipPlanePositions() {
		ClipPlaneStruct returnValue;

		returnValue.LowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
		returnValue.LowerRight = Camera.main.ScreenToWorldPoint (new Vector3 (Camera.main.pixelWidth, 0, Camera.main.nearClipPlane));
		returnValue.UpperLeft = Camera.main.ScreenToWorldPoint (new Vector3 (0, Camera.main.pixelHeight, Camera.main.nearClipPlane));
		returnValue.UpperRight = Camera.main.ScreenToWorldPoint (new Vector3 (Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));
		return returnValue;
	}
}
