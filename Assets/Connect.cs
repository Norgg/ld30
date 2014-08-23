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

	[RPC]
	void RemoveWall(NetworkViewID id) {
		Debug.Log("Removing wall: " + id);
		Network.RemoveRPCs(id);
		Destroy(NetworkView.Find(id).gameObject);
	}
	
	void Update() {
		if (connected && client && !worldCreated) {
			// Find an existing world to connect this one to
			GameObject[] worlds = GameObject.FindGameObjectsWithTag("World");
			if (worlds.Length > 0) {
				Debug.Log(worlds.Length);
				int worldIdx = Mathf.FloorToInt(Random.Range(0, worlds.Length));
				GameObject world = worlds[worldIdx];
				int dir = Mathf.FloorToInt(Random.Range(0, 4));
				GameObject newWorld = null;
				int worldSize = 50;
				if (dir == 0) {
					//+x
					Vector3 newPos = new Vector3(world.transform.position.x + worldSize, 0, world.transform.position.z);
					foreach (GameObject w in worlds) if (w.transform.position.Equals(newPos)) return;
					newWorld = (GameObject)Network.Instantiate(worldObj, newPos, Quaternion.identity, 0);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, world.transform.Find("wallx+").networkView.viewID);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, newWorld.transform.Find("wallx-").networkView.viewID);
				} else if (dir == 1) {
					//-x
					Vector3 newPos = new Vector3(world.transform.position.x - worldSize, 0, world.transform.position.z);
					foreach (GameObject w in worlds) if (w.transform.position.Equals(newPos)) return;
					newWorld = (GameObject)Network.Instantiate(worldObj, newPos, Quaternion.identity, 0);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, world.transform.Find("wallx-").networkView.viewID);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, newWorld.transform.Find("wallx+").networkView.viewID);
				} else if (dir == 2) {
					//+z
					Vector3 newPos = new Vector3(world.transform.position.x, 0, world.transform.position.z + worldSize);
					foreach (GameObject w in worlds) if (w.transform.position.Equals(newPos)) return;
					newWorld = (GameObject)Network.Instantiate(worldObj, newPos, Quaternion.identity, 0);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, world.transform.Find("wallz+").networkView.viewID);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, newWorld.transform.Find("wallz-").networkView.viewID);
				} else if (dir == 3) {
					//-z
					Vector3 newPos = new Vector3(world.transform.position.x, 0, world.transform.position.z - worldSize);
					foreach (GameObject w in worlds) if (w.transform.position.Equals(newPos)) return;					
					newWorld = (GameObject)Network.Instantiate(worldObj, newPos, Quaternion.identity, 0);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, world.transform.Find("wallz-").networkView.viewID);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, newWorld.transform.Find("wallz+").networkView.viewID);
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
