using UnityEngine;
using System.Collections.Generic;

public class Grow : MonoBehaviour {
	static int startWater = 2000;
	public int water = 0;
	float maxScale = 3;
	Color thirstyColor = new Color(0.5f, 0.5f, 0.5f);
	Color deadColor = new Color(0, 0, 0);
	Color baseColor;
	Color petalColor;
	List<GameObject> petals = new List<GameObject>();
	Daycycle daycycle;

	bool dead = false;

	public GameObject seedObj;
	int seeds = 2;
	public AudioClip plantClip;

	void Start () {
		baseColor = renderer.material.color;
		water = startWater;
		daycycle = GameObject.Find("Sun").GetComponent<Daycycle>();

		petalColor = new Color(Random.value, Random.value, Random.value);
		audio.PlayOneShot(plantClip);
		if (networkView.isMine) makePetals();
	}
	
	void Update () {
		if (!transform.networkView.isMine) return;
		if (dead) return;
		if (water > -startWater	&& daycycle.sunlight > 0.5f) water--;
		if (water > startWater) water = startWater;

		foreach (GameObject petal in petals) {
			petal.transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y/2, transform.position.z);
			petal.transform.localScale = new Vector3(transform.localScale.y/30, transform.localScale.y/3, transform.localScale.y/30);
		}

		if (water == -startWater) {
			dead = true;
			renderer.material.color = deadColor;
			foreach (GameObject petal in petals) {
				petal.renderer.material.color = deadColor;
			}
		} else if (water < 0) {
			foreach (GameObject petal in petals) {
				petal.renderer.material.color = Color.Lerp(petalColor, thirstyColor, (startWater+water)/startWater);
			}
		} else if (water < startWater/3) {
			renderer.material.color = Color.Lerp(thirstyColor, baseColor, water/((float)startWater/3));
			foreach (GameObject petal in petals) {
				petal.renderer.material.color = petalColor;
			}
		} else {
			renderer.material.color = baseColor;
			if (transform.networkView.isMine && daycycle.sunlight > 0.5) {
				if (transform.localScale.y < maxScale) {
					transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 1.0005f, transform.localScale.z);
				}
				if (transform.localScale.y > maxScale/2 && seeds > 0) {
					Network.Instantiate(seedObj, new Vector3(transform.position.x + Random.value/2, transform.position.y + 1, transform.position.z + Random.value/2), Quaternion.identity, 0);
					seeds--;
				}
			}
		}
	}

	void makePetals() {
		for (int i = 0; i < 4; i++) {
			makePetal(new Vector3(transform.position.x, transform.position.y + transform.localScale.y/2, transform.position.z),
			          Quaternion.Euler(70 + Random.value * 30, i*45 - 5 + 10 * Random.value, 0));
		}
	}


	void makePetal(Vector3 pos, Quaternion rot) {
		GameObject petalObj= Resources.Load<GameObject>("Prefabs/Petal");
		GameObject petal = (GameObject)Network.Instantiate(petalObj, pos, rot, 0);
		petal.transform.localScale = new Vector3(transform.localScale.x/5, transform.localScale.y/3, transform.localScale.z/5);
		petal.renderer.material.color = petalColor;
		petals.Add(petal);
	}

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		// Always send transform (depending on reliability of the network view)
		stream.Serialize(ref water);
		if (stream.isWriting) {
			Vector3 pos = transform.localPosition;
			Quaternion rot = transform.localRotation;
			Vector3 scale = transform.localScale;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref scale);
		} else {
			// Receive latest state information
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			Vector3 scale = Vector3.zero;
			stream.Serialize (ref pos);
			stream.Serialize (ref rot);
			stream.Serialize (ref scale);
			transform.localPosition = pos;
			transform.localRotation = rot;
			transform.localScale = scale;
		}
	}
}
