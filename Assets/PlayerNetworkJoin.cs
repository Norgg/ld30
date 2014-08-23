using UnityEngine;
using System.Collections;

public class PlayerNetworkJoin : MonoBehaviour {

	void Start() {
		if (!networkView.isMine) {
			transform.Find("Player Camera").camera.enabled = false;
			transform.Find("Player Camera").GetComponent<AudioListener>().enabled = false;
		}
	}
}
