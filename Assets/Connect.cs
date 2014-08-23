using UnityEngine;
using System.Collections;

public class Connect : MonoBehaviour {
	bool connected = false;
	int searchAttempts = 50;

	public GameObject playerObj;
	GameObject player;

	// Use this for initialization
	void Awake () {
		MasterServer.RequestHostList("norgg.connections");
	}
	
	void Update() {
		if (!connected) {
			if (searchAttempts > 0) {
				HostData[] data  = MasterServer.PollHostList();
				Debug.Log("Listing results");
				// Go through all the hosts in the host list, find a server with room
				foreach (HostData element in data) {
					Debug.Log(element.gameName);
					if (element.connectedPlayers < element.playerLimit) {
						Network.Connect(element);
						connected = true;
						Debug.Log("Connected to " + element.gameName);
						searchAttempts = 5;
					}
				}
				searchAttempts--;
			} else {
				Network.InitializeServer(32, 23242, !Network.HavePublicAddress());
				MasterServer.RegisterHost("norgg.connections", SystemInfo.deviceUniqueIdentifier, "");
				connected = true;
				Debug.Log("Started own server.");
				searchAttempts = 5;
			}
		}
	}

	void OnConnectedToServer() {
		Network.Instantiate(playerObj, new Vector3(0, 1, 0), Quaternion.identity, 0);
	}

	void OnServerInitialized() {
		Network.Instantiate(playerObj, new Vector3(0, 1, 0), Quaternion.identity, 0);
	}

	void OnDisconnectedFromServer() {
		connected = false;
	}
}
