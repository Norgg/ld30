using UnityEngine;
using System.Collections;

public class Connect : MonoBehaviour {
	bool connected = false;
	bool client = false;
	bool worldCreated = false;
	int searchAttempts = 50;

	public GameObject playerObj;
	public GameObject worldObj;
	GameObject player;

	// Use this for initialization
	void Awake () {
		MasterServer.RequestHostList("norgg.connections");
	}
	
	void Update() {
		if (connected && client && !worldCreated) {
			// Find an existing world to connect this one to
			GameObject[] worlds = GameObject.FindGameObjectsWithTag("World");
			if (worlds.Length > 0) {
				Debug.Log(worlds.Length);
				GameObject world = worlds[Mathf.FloorToInt(Random.Range(0, worlds.Length))];
				int dir = Mathf.FloorToInt(Random.Range(0, 4));
				GameObject newWorld = null;
				int worldSize = 50;
				if (dir == 0) {
					//+x
					newWorld = (GameObject)Network.Instantiate(worldObj, new Vector3(world.transform.position.x + worldSize, 0, 0), Quaternion.identity, 0);
					GameObject.Destroy(world.transform.Find("wallx+").gameObject);
					GameObject.Destroy(newWorld.transform.Find("wallx-").gameObject);
				} else if (dir == 1) {
					//-x
					newWorld = (GameObject)Network.Instantiate(worldObj, new Vector3(world.transform.position.x - worldSize, 0, 0), Quaternion.identity, 0);
					GameObject.Destroy(world.transform.Find("wallx-").gameObject);
					GameObject.Destroy(newWorld.transform.Find("wallx+").gameObject);
				} else if (dir == 2) {
					//+z
					newWorld = (GameObject)Network.Instantiate(worldObj, new Vector3(0, 0, world.transform.position.z + worldSize), Quaternion.identity, 0);
					GameObject.Destroy(world.transform.Find("wallz+").gameObject);
					GameObject.Destroy(newWorld.transform.Find("wallz-").gameObject);
				} else if (dir == 3) {
					//-z
					newWorld = (GameObject)Network.Instantiate(worldObj, new Vector3(0, 0, world.transform.position.z - worldSize), Quaternion.identity, 0);
					GameObject.Destroy(world.transform.Find("wallz+").gameObject);
					GameObject.Destroy(newWorld.transform.Find("wallz-").gameObject);
				}
				Network.Instantiate(playerObj, new Vector3(newWorld.transform.position.x, 1, newWorld.transform.position.z), Quaternion.identity, 0);
				worldCreated = true;
			}
		}
		if (!connected && !client) {
			if (searchAttempts > 0) {
				HostData[] data  = MasterServer.PollHostList();
				Debug.Log("Listing results");
				// Go through all the hosts in the host list, find a server with room
				foreach (HostData element in data) {
					Debug.Log(element.gameName);
					if (element.connectedPlayers < element.playerLimit) {
						Network.Connect(element);
						client = true;
						Debug.Log("Connected to " + element.gameName);
						searchAttempts = 5;
					}
				}
				searchAttempts--;
			} else {
				Network.InitializeServer(32, 23242, !Network.HavePublicAddress());
				MasterServer.RegisterHost("norgg.connections", SystemInfo.deviceUniqueIdentifier, "");
				connected = true;
				client = false;
				Debug.Log("Started own server.");
				searchAttempts = 5;
			}
		}
	}

	void OnConnectedToServer() {
		connected = true;
	}

	void OnServerInitialized() {
		Network.Instantiate(worldObj, new Vector3(0, 0, 0), Quaternion.identity, 0);
		Network.Instantiate(playerObj, new Vector3(0, 1, 0), Quaternion.identity, 0);
	}

	void OnDisconnectedFromServer() {
		client = false;
		worldCreated = false;
		connected = false;
	}
}
