using UnityEngine;
using System.Collections;

public static class Helper {

	public static float	CameraClamp(float angle, float min, float max) {
		int iangle = (int)angle;
		iangle = iangle % 360;
		return Mathf.Clamp(iangle, min, max);
	}

}
