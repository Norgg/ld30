using UnityEngine;
using System.Collections;

public class Watering : MonoBehaviour {
	Transform cam;
	Transform heldWater;

	static int maxWater = 100;
	int water = maxWater;
	Vector3 baseWaterScale;

	// Use this for initialization
	void Start () {
		cam = transform.Find("Player Camera");
		heldWater = transform.Find("Held Water");
		baseWaterScale = heldWater.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (!transform.networkView.isMine) return;

		if (Input.GetMouseButton(0)) {
			RaycastHit hit;
			Vector3 fwd = cam.TransformDirection(Vector3.forward);
			if (Physics.Raycast(cam.position, fwd, out hit, 3.0f)) {
				if (hit.transform.name.StartsWith("Sprout")) {
					Debug.Log("Watering");
					if (water > 0) {
						hit.transform.GetComponent<Grow>().water++;
						water--;
						heldWater.transform.localScale = baseWaterScale * water / (float)maxWater;
					}
				}
			}
		}
	}
}
