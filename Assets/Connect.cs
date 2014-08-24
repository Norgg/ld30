using UnityEngine;
using System.Collections.Generic;

public class Connect : MonoBehaviour {
	bool connected = false;
	bool client = false;
	bool worldCreated = false;
	static int maxSearchAttempts = 100;
	int searchAttempts = maxSearchAttempts;

	string connectingTo = null;
	List<string> failed = new List<string>();

	public GameObject playerObj;
	public GameObject worldObj;
	public GameObject seedObj;
	GameObject player;

	// Use this for initialization
	void Awake () {
		Screen.showCursor = false;
		MasterServer.RequestHostList("norgg.connections");
	}

	[RPC]
	void RemoveWall(NetworkViewID id) {
		Network.RemoveRPCs(id);
		Destroy(NetworkView.Find(id).gameObject);
	}

	void OnGUI() {
		if (!connected) {
			GUI.Label(new Rect(0,0,Screen.width,Screen.height),"Joining...");
		}
	}
	
	void Update() {
		if (Input.GetKeyDown(KeyCode.R)) {
			Application.LoadLevel(0);
		}

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
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, world.transform.Find("Wallx+").networkView.viewID);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, newWorld.transform.Find("Wallx-").networkView.viewID);
				} else if (dir == 1) {
					//-x
					Vector3 newPos = new Vector3(world.transform.position.x - worldSize, 0, world.transform.position.z);
					foreach (GameObject w in worlds) if (w.transform.position.Equals(newPos)) return;
					newWorld = (GameObject)Network.Instantiate(worldObj, newPos, Quaternion.identity, 0);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, world.transform.Find("Wallx-").networkView.viewID);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, newWorld.transform.Find("Wallx+").networkView.viewID);
				} else if (dir == 2) {
					//+z
					Vector3 newPos = new Vector3(world.transform.position.x, 0, world.transform.position.z + worldSize);
					foreach (GameObject w in worlds) if (w.transform.position.Equals(newPos)) return;
					newWorld = (GameObject)Network.Instantiate(worldObj, newPos, Quaternion.identity, 0);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, world.transform.Find("Wallz+").networkView.viewID);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, newWorld.transform.Find("Wallz-").networkView.viewID);
				} else if (dir == 3) {
					//-z
					Vector3 newPos = new Vector3(world.transform.position.x, 0, world.transform.position.z - worldSize);
					foreach (GameObject w in worlds) if (w.transform.position.Equals(newPos)) return;					
					newWorld = (GameObject)Network.Instantiate(worldObj, newPos, Quaternion.identity, 0);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, world.transform.Find("Wallz-").networkView.viewID);
					networkView.RPC("RemoveWall", RPCMode.AllBuffered, newWorld.transform.Find("Wallz+").networkView.viewID);
				}
				Network.Instantiate(playerObj, new Vector3(newWorld.transform.position.x, 1, newWorld.transform.position.z-4), Quaternion.identity, 0);
				Network.Instantiate(seedObj, new Vector3(newWorld.transform.position.x+1, 2, newWorld.transform.position.z+1), Quaternion.identity, 0);
				Network.Instantiate(seedObj, new Vector3(newWorld.transform.position.x+1, 2, newWorld.transform.position.z-1), Quaternion.identity, 0);
				Network.Instantiate(seedObj, new Vector3(newWorld.transform.position.x-1, 2, newWorld.transform.position.z+1), Quaternion.identity, 0);
				Network.Instantiate(seedObj, new Vector3(newWorld.transform.position.x-1, 2, newWorld.transform.position.z-1), Quaternion.identity, 0);
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
					if (element.connectedPlayers < element.playerLimit && !failed.Contains(element.gameName)) {
						Network.Connect(element);
						client = true;
						Debug.Log("Connecting to " + element.gameName);
						connectingTo = element.gameName;
						searchAttempts = maxSearchAttempts;
					}
				}
				searchAttempts--;
			} else {
				Network.InitializeServer(8, 23242, !Network.HavePublicAddress());
				MasterServer.RegisterHost("norgg.connections", SystemInfo.deviceUniqueIdentifier, "");
				connected = true;
				client = false;
				Debug.Log("Started own server.");
				searchAttempts = maxSearchAttempts;
			}
		}
	}

	void OnConnectedToServer() {
		connected = true;
	}

	void OnServerInitialized() {
		Network.Instantiate(worldObj, new Vector3(0, 0, 0), Quaternion.identity, 0);
		Network.Instantiate(playerObj, new Vector3(0, 1, -4), Quaternion.identity, 0);
		Network.Instantiate(seedObj, new Vector3(1, 2, 1), Quaternion.identity, 0);
		Network.Instantiate(seedObj, new Vector3(1, 2, -1), Quaternion.identity, 0);
		Network.Instantiate(seedObj, new Vector3(-1, 2, 1), Quaternion.identity, 0);
		Network.Instantiate(seedObj, new Vector3(-1, 2, -1), Quaternion.identity, 0);
	}

	void OnDisconnectedFromServer() {
		client = false;
		worldCreated = false;
		connected = false;
		Application.LoadLevel(0);
	}

	void OnFailedToConnect() {
		Debug.Log("Couldn't connect to " + connectingTo);
		client = false;
		worldCreated = false;
		connected = false;
		MasterServer.RequestHostList("norgg.connections");
		searchAttempts = maxSearchAttempts;
		failed.Add(connectingTo);
	}

	[RPC]
	void RemovePlayer(NetworkViewID id) {
		GameObject p = NetworkView.Find(id).gameObject;
		Transform body = p.transform.Find("Body");
		body.renderer.material.color = new Color(0,0,0);
		if (body.Find("REye") == null) Debug.Log("REye missing?!?");
		body.Find("REye").renderer.material.color = new Color(0.3f, 0.3f, 0.3f);
		body.Find("LEye").renderer.material.color = new Color(0.3f, 0.3f, 0.3f);
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		// Take ownership of everything owned by that player
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject p in players) {
			if (p.networkView.owner == player) {
				networkView.RPC("RemovePlayer", RPCMode.AllBuffered, p.networkView.viewID);
			}
		}
	}
}
