using UnityEngine;
using System.Collections;

public class Watering : MonoBehaviour {
	Transform cam;
	Transform heldWater;

	static int maxWater = 3000;
	int water = maxWater;
	Vector3 baseWaterScale;



	// Use this for initialization
	void Start () {
		cam = transform.Find("Player Camera");
		heldWater = transform.Find("Held Water");
		baseWaterScale = heldWater.transform.localScale;
	}

	[RPC]
	void WaterPlant(NetworkViewID plant) {
		NetworkView view = NetworkView.Find(plant);
		if (view.isMine) {
			view.transform.GetComponent<Grow>().water += 5;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!transform.networkView.isMine) return;

		if (Input.GetMouseButton(0)) {
			RaycastHit hit;
			Vector3 fwd = cam.TransformDirection(Vector3.forward);
			if (Physics.Raycast(cam.position, fwd, out hit, 3.0f)) {
				if (hit.transform.name.StartsWith("Sprout")) {
					//Debug.Log("Watering");
					if (water > 0) {
						networkView.RPC("WaterPlant", RPCMode.All, hit.transform.networkView.viewID);
						//hit.transform.GetComponent<Grow>().water+=5;
						water-=5;
						heldWater.transform.localScale = baseWaterScale * water / (float)maxWater;
					}
				} else if (hit.transform.name.StartsWith("Water") && water < maxWater) {
					water+= 10;
					heldWater.transform.localScale = baseWaterScale * water / (float)maxWater;
				}
			}
		}
	}
}
