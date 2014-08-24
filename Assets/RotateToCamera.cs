using UnityEngine;
using System.Collections;

public class RotateToCamera : MonoBehaviour {
	Camera cam;
	//Transform body;

	// Use this for initialization
	void Start () {
		cam = transform.FindChild("Player Camera").camera;
		//body = transform.FindChild("Body");
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.networkView.isMine) {
			transform.Rotate(new Vector3(0, cam.transform.localEulerAngles.y, 0));
			cam.transform.localEulerAngles = new Vector3(cam.transform.rotation.eulerAngles.x, 0, cam.transform.rotation.eulerAngles.z);
		}
	}
}
