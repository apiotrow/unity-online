using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	[SyncVar]
	public int moveX = 0;
	public int moveY = 0;
	public float moveSpeed = 0.2f;
	public Vector3 newPos;
	public Vector3 syncPos;

	[SyncVar]
	public Color myColor;

	void Start()
	{
 		DontDestroyOnLoad(gameObject);
		newPos = new Vector3(0f, 0f, 0f);
	}

	public GameObject cratePrefab;


	//[SyncVar]
	public GameObject crate;


	public override void OnStartClient()
	{

		//Debug.Log("Player OnStartClient netId:" + netId + " crate:" + this.crate);
		GetComponent<MeshRenderer>().material.color = myColor;
	}
		
	[ClientRpc]
	void RpcPoke(int value)
	{
		Debug.Log("value:"+value);
	}


	[Command]
	void CmdMakeCrate()
	{

		GameObject crate = (GameObject)Instantiate(cratePrefab, transform.position, Quaternion.identity);
		NetworkServer.Spawn(crate);

		this.crate = crate;
	}

//	[Command]
//	void CmdProvidePosToServer(Vector3 pos){
//		syncPos = pos;
//	}
//
//	[ClientCallback]
//	void transmitPos(){
//		CmdProvidePosToServer(transform.position);
//	}

	void Update () 
	{
		if (!isLocalPlayer) {
			return;
//			transform.position = Vector3.Lerp (transform.position, p, 0.1f);
		}
		
		// input handling for local player only
		int oldMoveX = moveX;
		int oldMoveY = moveY;
		
		moveX = 0;
		moveY = 0;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			CmdMakeCrate();
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			CmdLobby();
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			moveX -= 1;
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			moveX += 1;
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			moveY += 1;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			moveY -= 1;
		}
		if (moveX != oldMoveX || moveY != oldMoveY)
		{
			CmdMove(moveX, moveY);
		}
	}
	
	[Command]
	public void CmdLobby()
	{
		var lobby = NetworkManager.singleton as NetworkLobbyManager;
		NetworkManager.singleton.ServerChangeScene(lobby.lobbyScene);
	}

	[Command]
	public void CmdMove(int x, int y)
	{
		moveX = x;
		moveY = y;

		newPos = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
//		transform.position = Vector3.Lerp (transform.position, newPos, 0.1f);
		print ("CmdMove");
		transform.Translate(moveX * moveSpeed, moveY * moveSpeed, 0);
		base.SetDirtyBit(1);
	}
	
	[ServerCallback]
	public void FixedUpdate()
	{
//		transmitPos();
//		transform.position = Vector3.Lerp (transform.position, newPos, 0.1f);
		transform.Translate(moveX * moveSpeed, moveY * moveSpeed, 0);
	}
}
