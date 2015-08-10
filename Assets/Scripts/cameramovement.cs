using UnityEngine;
using System.Collections;

public class cameramovement : MonoBehaviour {

	public GameObject player;
	//public GameObject player2; // to follow the second player as well
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position;
			
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = player.transform.position + offset;

	}
}
