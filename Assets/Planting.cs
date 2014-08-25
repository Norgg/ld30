using UnityEngine;
using System.Collections;

public class Planting : MonoBehaviour {
	GameObject heldSeed = null;
	public GameObject sproutObj;
	Transform cam;
	Transform arm;
	public AudioClip pickClip;
	public AudioClip[] chatSounds;

	// Use this for initialization
	void Start () {
		cam = transform.Find("Player Camera").transform;
	}

	[RPC]
	void RemoveSeed(NetworkViewID id) {
		Network.RemoveRPCs(id);
		Destroy(NetworkView.Find(id).gameObject);
	}

	[RPC]
	void Chat() {
		audio.PlayOneShot(chatSounds[Mathf.FloorToInt(Random.value * chatSounds.Length)]);
	}

	// Update is called once per frame
	void Update () {
		if (!transform.networkView.isMine) return;

		if (heldSeed == null) {
			if (Input.GetMouseButtonDown(0)) {
				RaycastHit hit;
				Vector3 fwd = cam.TransformDirection(Vector3.forward);
				if (Physics.Raycast(cam.position, fwd, out hit, 3.0f)) {
					if (hit.transform.name.StartsWith("Seed")) {
						heldSeed = hit.transform.gameObject;
						Debug.Log("Picked up a seed");
						audio.PlayOneShot(pickClip);
					} else if (hit.transform.name.StartsWith("Player")) {
						networkView.RPC("Chat", RPCMode.All);
					}
				}
			}
		} else {
			heldSeed.transform.position = cam.position + cam.TransformDirection(Vector3.forward) + cam.TransformDirection(Vector3.left)/2;
			if (Input.GetMouseButtonDown(0)) {
				RaycastHit hit;
				Vector3 fwd = cam.TransformDirection(Vector3.forward);
				if (Physics.Raycast(cam.position, fwd, out hit, 3.0f)) {
					if (hit.transform.name.StartsWith("Ground")) {
						//plant seed
						Debug.Log ("Planting seed");
						networkView.RPC("RemoveSeed", RPCMode.AllBuffered, heldSeed.networkView.viewID);
						heldSeed = null;
						Network.Instantiate(sproutObj, hit.point, Quaternion.identity, 0);
					} else if (hit.transform.name.StartsWith("Player")) {
						networkView.RPC("Chat", RPCMode.All);
					} else {
						heldSeed.rigidbody.velocity = new Vector3(0, 0, 0);
						heldSeed = null;
					}
				} else {
					heldSeed.rigidbody.velocity = new Vector3(0, 0, 0);
					heldSeed = null;
				}
			}
		}
	}
}
