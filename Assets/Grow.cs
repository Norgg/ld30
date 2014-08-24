using UnityEngine;
using System.Collections.Generic;

public class Grow : MonoBehaviour {
	static int startWater = 4000;
	public int water = 0;
	float maxScale = 10;
	Color thirstyColor = new Color(0.5f, 0.5f, 0.5f);
	Color baseColor;
	int maxSprouts = 10;
	int sprouts = 0;
	List<GameObject> petals = new List<GameObject>();

	void Start () {
		baseColor = renderer.material.color;
		water = startWater;
	}
	
	void Update () {
		if (water > 0) water--;

		foreach (GameObject petal in petals) {
			petal.transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y/2, transform.position.z);
			petal.transform.localScale = new Vector3(transform.localScale.x/5, transform.localScale.y/3, transform.localScale.z/5);
		}

		if (water < startWater/2) {
			renderer.material.color = Color.Lerp(thirstyColor, baseColor, water/((float)startWater/2));
		} else {
			renderer.material.color = baseColor;
			if (transform.networkView.isMine) {
				if (sprouts < maxSprouts){// && transform.localScale.y > maxScale / 3) {
					Sprout();
					sprouts++;
				}
				if (transform.localScale.y < maxScale) {
					transform.localScale *= 1.0005f;
				}
			}
		}

		if (water == 0) {
			// Kill it
		}
	}

	void Sprout() {
		Debug.Log("Sprouting new branch");
		GameObject sproutObj= Resources.Load<GameObject>("Prefabs/Petal");
		GameObject petal = (GameObject)Network.Instantiate(sproutObj, 
		                                                   new Vector3(transform.position.x, transform.position.y + transform.localScale.y/2, transform.position.z),
		                                                   Quaternion.Euler(70 + Random.value * 30, Random.value * 180, 0), 
		                                                   0);
		petal.transform.localScale = new Vector3(transform.localScale.x/5, transform.localScale.y/3, transform.localScale.z/5);
		petals.Add(petal);
	}
}
