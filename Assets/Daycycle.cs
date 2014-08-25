using UnityEngine;
using System.Collections;

public class Daycycle : MonoBehaviour {

	public float sunlight = 0.0f;
	Color lightCol;
	Color setCol;

	void Start () {
		lightCol = light.color;
		setCol = new Color(32f, lightCol.g, lightCol.b);
	}
	
	void Update () {
		transform.Rotate(new Vector3(0.1f, 0, 0));
		sunlight = 0.6f + Mathf.Sin((transform.localEulerAngles.x) * Mathf.Deg2Rad)/2;
		if (sunlight < 0.8) {
			light.color = Color.Lerp(setCol, lightCol, Mathf.Abs(sunlight-0.6f)*5);
		} else {
			light.color = lightCol;
		}
		light.intensity = sunlight;
	}
}
