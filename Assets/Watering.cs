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

	public GameObject particleSystemObj;
	ParticleSystem particles;

	public AudioClip[] waterSounds;


	bool watered = false;
	bool playSounds = false;

	// Use this for initialization
	void Start () {
		cam = transform.Find("Player Camera");
		heldWater = transform.Find("Held Water");
		outlineWater = transform.Find("Outline Water");
		outlinePos = outlineWater.localPosition;
		baseWaterScale = heldWater.transform.localScale;
		if (networkView.isMine) {
			particles = ((GameObject)Network.Instantiate(particleSystemObj, transform.position, Quaternion.identity, 0)).particleSystem;
			particles.enableEmission = false;
			particles.transform.parent = transform;
		}
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

	[RPC]
	void sprinkle(bool on) {
		particles.enableEmission = on;
		playSounds = on;
	}

	void shakeOutline() {
		Hashtable opts = new Hashtable();
		opts.Add("amount", new Vector3(0.01f, 0.01f, 0.01f));
		opts.Add("time", 0.1f);
		opts.Add("islocal", false);
		opts.Add("oncomplete", "RestoreOutline");
		opts.Add("oncompletetarget", gameObject);
		
		iTween.ShakePosition(outlineWater.gameObject, opts);
	}

	void waterSprout(Transform plant) {
		//Debug.Log("Watering");
		if (water > 0) {
			networkView.RPC("WaterPlant", RPCMode.All, plant.networkView.viewID);
			//hit.transform.GetComponent<Grow>().water+=5;
			particles.transform.localPosition = Vector3.zero;
			particles.transform.LookAt(plant.transform.position);
			watered = true;
			water-=10;
			heldWater.transform.localScale = baseWaterScale * water / (float)maxWater;
		} else {
			shakeOutline();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (playSounds && !audio.isPlaying) {
			audio.clip = waterSounds[Mathf.FloorToInt(Random.value * waterSounds.Length)];
			audio.Play();
		}

		if (!transform.networkView.isMine) return;

		watered = false;

		if (Input.GetMouseButton(0)) {
			RaycastHit hit;
			Vector3 fwd = cam.TransformDirection(Vector3.forward);
			if (Physics.Raycast(cam.position, fwd, out hit, 3.0f)) {
				if (hit.transform.name.StartsWith("Sprout")) {
					waterSprout(hit.transform);
				} else if (hit.transform.name.StartsWith("Ground")) {
					Collider[] colliders = Physics.OverlapSphere(hit.point, 1.0f);
					foreach (Collider c in colliders) {
						if (c.name.StartsWith("Sprout")) {
							waterSprout(c.transform);
							break;
						}
					}
				} else if (hit.transform.name.StartsWith("Water")) {
					if (water < maxWater) {
						particles.transform.position = hit.transform.position;
						particles.transform.LookAt(transform.position);
						watered = true;
						water+= 20;
						heldWater.transform.localScale = baseWaterScale * water / (float)maxWater;
					} else {
						shakeOutline();
					}
				}
			}
		}

		if (watered) {
			if (!particles.enableEmission) networkView.RPC("sprinkle", RPCMode.All, true);
			particles.enableEmission = true;
		} else {
			if (particles.enableEmission) networkView.RPC("sprinkle", RPCMode.All, false);
			particles.enableEmission = false;
		}
	}
}
