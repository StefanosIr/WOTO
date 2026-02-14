using UnityEngine;
using System.Collections;

public class cameramovement : MonoBehaviour {

	[Header("Target Settings")]
	public GameObject player;
	//public GameObject player2; // to follow the second player as well
	
	[Header("Camera Settings")]
	public Vector3 offset = new Vector3(0, 5, -10);
	public float smoothSpeed = 0.125f;
	public bool useSmoothFollow = true;

	private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		if (player != null) {
			// Calculate initial offset if not manually set
			if (offset == Vector3.zero) {
				offset = transform.position - player.transform.position;
			}
		}
	}
	
	// LateUpdate is called after all Update functions
	// This ensures the camera follows after the player has moved
	void LateUpdate () {
		if (player == null) return;

		Vector3 desiredPosition = player.transform.position + offset;

		if (useSmoothFollow) {
			// Smooth camera following using SmoothDamp for natural movement
			transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
		} else {
			// Instant following (original behavior)
			transform.position = desiredPosition;
		}
	}

	/// <summary>
	/// Sets a new target for the camera to follow
	/// </summary>
	public void SetTarget(GameObject newTarget) {
		player = newTarget;
		if (player != null) {
			offset = transform.position - player.transform.position;
		}
	}
}
