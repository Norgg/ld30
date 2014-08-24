using UnityEngine;
using System.Collections;

public class Daycycle : MonoBehaviour {

	public float sunlight = 0.0f;

	void Start () {
	
	}
	
	void Update () {
		transform.Rotate(new Vector3(0.1f, 0, 0));
		sunlight = 0.6f + Mathf.Sin((transform.localEulerAngles.x) * Mathf.Deg2Rad)/2;
		light.intensity = sunlight;
	}
}
