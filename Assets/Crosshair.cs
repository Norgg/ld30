using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {
	public Texture2D crosshairTexture;
	Rect position;
	
	void Start()
	{
		position = new Rect((Screen.width - crosshairTexture.width) / 2, 
		                	(Screen.height - crosshairTexture.height) / 2, 
		                	crosshairTexture.width, 
		                	crosshairTexture.height);
		Screen.lockCursor = true;
	}

	void Update() {
		if (Input.GetKeyDown("escape")) Screen.lockCursor = false;
		if (Input.GetMouseButtonDown(0)) Screen.lockCursor = true;
	}
	
	void OnGUI()
	{
		GUI.DrawTexture(position, crosshairTexture);
	}
}