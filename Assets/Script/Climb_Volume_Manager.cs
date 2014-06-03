using UnityEngine;
using System.Collections;

public class Climb_Volume_Manager : MonoBehaviour {

	private	void	OnTriggerEnter(Collider other) {
		if (Character_Manager.Instance.gameObject == other.gameObject) {
			Animation_Manager.Instance.SetClimbVolumeTransform(this.transform);
		}
	}

	private	void	OnTriggerExit(Collider other) {
		if (Character_Manager.Instance.gameObject == other.gameObject) {
			Animation_Manager.Instance.ClearClimbVolumeTransform(this.transform);
		}
	}
}
