using UnityEngine;
using System.Collections;

public class Watering : MonoBehaviour {
	Transform cam;
	Transform heldWater;
	Transform outlineWater;

	Vector3 outlinePos;

	static int maxWater = 3000;
	int water = maxWater;
	Vector3 baseWaterScale;
	


	// Use this for initialization
	void Start () {
		cam = transform.Find("Player Camera");
		heldWater = transform.Find("Held Water");
		outlineWater = transform.Find("Outline Water");
		outlinePos = outlineWater.localPosition;
		baseWaterScale = heldWater.transform.localScale;
	}

	[RPC]
	void WaterPlant(NetworkViewID plant) {
		NetworkView view = NetworkView.Find(plant);
		if (view.isMine) {
			view.transform.GetComponent<Grow>().water += 10;
		}
	}

	void RestoreOutline() {
		outlineWater.localPosition = outlinePos;
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
						water-=10;
						heldWater.transform.localScale = baseWaterScale * water / (float)maxWater;
					} else {
						Hashtable opts = new Hashtable();
						opts.Add("amount", new Vector3(0.02f, 0.02f, 0.02f));
						opts.Add("time", 0.3f);
						opts.Add("islocal", false);
						opts.Add("oncomplete", "RestoreOutline");
						opts.Add("oncompletetarget", gameObject);

						iTween.ShakePosition(outlineWater.gameObject, opts);
					}
				} else if (hit.transform.name.StartsWith("Water") && water < maxWater) {
					water+= 20;
					heldWater.transform.localScale = baseWaterScale * water / (float)maxWater;
				}
			}
		}
	}
}
