using UnityEngine;
using System.Collections;

public class RotateToCamera : MonoBehaviour {
	Camera cam;
	Transform body;

	// Use this for initialization
	void Start () {
		cam = transform.FindChild("Player Camera").camera;
		body = transform.FindChild("Body");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0, cam.transform.rotation.eulerAngles.y, 0));
		cam.transform.eulerAngles = new Vector3(cam.transform.rotation.eulerAngles.x, 0, cam.transform.rotation.eulerAngles.z);
	}
}
