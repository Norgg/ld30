using UnityEngine;
using System.Collections;

public class Planting : MonoBehaviour {
	GameObject heldSeed = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.networkView.isMine && Input.GetMouseButtonDown(0)) {
			Debug.Log("Raycasting");
			RaycastHit hit;
			Vector3 fwd = transform.Find("Player Camera").transform.TransformDirection(Vector3.forward);
			if (Physics.Raycast(transform.position, fwd, out hit)) {
				Debug.Log("Hit: " + hit.transform.name);
				if (hit.transform.name.StartsWith("Seed")) {
					if (heldSeed == null) {
						heldSeed = hit.transform.gameObject;
					}
					Debug.Log("Clicked on a seed");
				} else if (hit.transform.name.StartsWith("Ground")) {
					//plant seed
				}
			}
		}
	}
}
