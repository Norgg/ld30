using UnityEngine;
using System.Collections;

public class Connect : MonoBehaviour {
	bool connected = false;

	public GameObject playerObj;

	// Use this for initialization
	void Awake () {
		MasterServer.RequestHostList("norgg.connections");
	}
	
	void Update() {
		if (!connected) {
			HostData[] data  = MasterServer.PollHostList();
			// Go through all the hosts in the host list
			foreach (HostData element in data) {
				if (element.connectedPlayers < element.playerLimit) {
					Network.Connect(element);
					connected = true;
					print ("Connected to " + element.gameName);
				}
			}

			if (!connected) {
				Network.InitializeServer(32, 23242, !Network.HavePublicAddress());
				MasterServer.RegisterHost("norgg.connections", SystemInfo.deviceUniqueIdentifier, "");
				connected = true;
				print ("Started own server.");
			}
		}
	}

	void OnConnectedToServer() {
		Network.Instantiate(playerObj, new Vector3(0, 1, 0), Quaternion.identity, 0);
	}

	void OnServerInitialized() {
		Network.Instantiate(playerObj, new Vector3(0, 1, 0), Quaternion.identity, 0);
	}
}
