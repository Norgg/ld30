using UnityEngine;
using System.Collections;

public class ArmFollow : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localEulerAngles = transform.parent.parent.Find("Player Camera").localEulerAngles;
	}
}
