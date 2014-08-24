using UnityEngine;
using System.Collections;

public class Planting : MonoBehaviour {
	GameObject heldSeed = null;
	Transform cam;
	Transform arm;

	// Use this for initialization
	void Start () {
		cam = transform.Find("Player Camera").transform;
	}

	void RemoveSeed(NetworkViewID id) {
		Network.RemoveRPCs(id);
		Destroy(NetworkView.Find(id).gameObject);
	}

	// Update is called once per frame
	void Update () {
		if (!transform.networkView.isMine) return;

		if (heldSeed == null) {
			if (Input.GetMouseButtonDown(0)) {
				Debug.Log("Raycasting");
				RaycastHit hit;
				Vector3 fwd = cam.TransformDirection(Vector3.forward);
				if (Physics.Raycast(cam.position, fwd, out hit)) {
					if (hit.transform.name.StartsWith("Seed")) {
						heldSeed = hit.transform.gameObject;
						Debug.Log("Picked up a seed");
					}
				}
			}
		} else {
			heldSeed.transform.position = cam.position + cam.TransformDirection(Vector3.forward) + cam.TransformDirection(Vector3.left)/2;
			if (Input.GetMouseButtonDown(0)) {
				RaycastHit hit;
				Vector3 fwd = cam.TransformDirection(Vector3.forward);
				if (Physics.Raycast(cam.position, fwd, out hit)) {
					if (hit.transform.name.StartsWith("Ground")) {
						//plant seed
						Debug.Log ("Planting seed");
						RemoveSeed(heldSeed.networkView.viewID);
						heldSeed = null;
					}
				}
			}
		}
	}
}
