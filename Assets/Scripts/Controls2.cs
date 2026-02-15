using UnityEngine;
using System.Collections;

public class Controls2 : MonoBehaviour {
	
	[Header("Movement Settings")]
	public float rotationSpeed = 100.0f;
	
	[Header("Animation Timings")]
	public float jumpAnimationDuration = 0.1f;
	public float punchAnimationDuration = 0.5f;
	public float kickAnimationDuration = 0.5f;
	
	private Animator myAnimator;
	
	// Animation parameter name constants
	private const string ANIM_VSPEED = "VSpeed";
	private const string ANIM_HSPEED = "HSpeed";
	private const string ANIM_JUMPING = "Jumping";
	private const string ANIM_PUNCHING = "Punching";
	private const string ANIM_KICKING = "Kicking";
	private const string ANIM_TURNING_LEFT = "TurningLeft";
	private const string ANIM_TURNING_RIGHT = "TurningRight";
	private const string ANIM_CURRENT_ACTION = "CurrentAction";
	
	// Use this for initialization
	void Start () {
		myAnimator = GetComponent<Animator>();
		if (myAnimator == null) {
			Debug.LogError("Animator component not found on " + gameObject.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (myAnimator == null) return;
		
		HandleMovement();
		HandleCombat();
		HandleRotation();
		HandleActions();
	}
	
	void HandleMovement() {
		// Update movement animation parameters
		float verticalInput = Input.GetAxis("Vertical2");
		float horizontalInput = Input.GetAxis("Horizontal2");
		
		myAnimator.SetFloat(ANIM_VSPEED, verticalInput);
		myAnimator.SetFloat(ANIM_HSPEED, horizontalInput);
		
		// Handle jumping
		if (Input.GetButtonDown("Jump2")) {
			myAnimator.SetBool(ANIM_JUMPING, true);
			Invoke("StopJumping", jumpAnimationDuration);
		}
	}
	
	void HandleCombat() {
		// Handle punching
		if (Input.GetButtonDown("Fire1p")) {
			myAnimator.SetBool(ANIM_PUNCHING, true);
			Invoke("StopPunching", punchAnimationDuration);
		}
		
		// Handle kicking
		if (Input.GetButtonDown("Fire2p")) {
			myAnimator.SetBool(ANIM_KICKING, true);
			Invoke("StopKicking", kickAnimationDuration);
		}
	}
	
	void HandleRotation() {
		float verticalInput = Input.GetAxis("Vertical2");
		float horizontalInput = Input.GetAxis("Horizontal2");
		bool isMoving = (verticalInput != 0f) || (horizontalInput != 0f);
		
		// Turn left ([ key)
		if (Input.GetKey(KeyCode.LeftBracket)) {
			transform.Rotate(Vector3.down * Time.deltaTime * rotationSpeed);
			myAnimator.SetBool(ANIM_TURNING_LEFT, !isMoving);
		} else {
			myAnimator.SetBool(ANIM_TURNING_LEFT, false);
		}
		
		// Turn right (] key)
		if (Input.GetKey(KeyCode.RightBracket)) {
			transform.Rotate(Vector3.down * Time.deltaTime * -rotationSpeed);
			myAnimator.SetBool(ANIM_TURNING_RIGHT, !isMoving);
		} else {
			myAnimator.SetBool(ANIM_TURNING_RIGHT, false);
		}
	}
	
	void HandleActions() {
		int currentAction = myAnimator.GetInteger(ANIM_CURRENT_ACTION);
		
		// Action 1 (toggle) - ' key
		if (Input.GetKeyDown(KeyCode.Quote)) {
			ToggleAction(currentAction, 1);
		}
		
		// Action 2 (toggle) - ; key
		if (Input.GetKeyDown(KeyCode.Semicolon)) {
			ToggleAction(currentAction, 2);
		}
		
		// Action 3 (hold) - 3 key
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			myAnimator.SetLayerWeight(1, 1f);
			myAnimator.SetInteger(ANIM_CURRENT_ACTION, 3);
		}
		
		if (Input.GetKeyUp(KeyCode.Alpha3)) {
			myAnimator.SetInteger(ANIM_CURRENT_ACTION, 0);
		}
	}
	
	void ToggleAction(int currentAction, int actionNumber) {
		if (currentAction == actionNumber) {
			myAnimator.SetInteger(ANIM_CURRENT_ACTION, 0);
		} else {
			myAnimator.SetInteger(ANIM_CURRENT_ACTION, actionNumber);
		}
	}
	
	// Animation state reset methods
	void StopJumping() {
		if (myAnimator != null) {
			myAnimator.SetBool(ANIM_JUMPING, false);
		}
	}
	
	void StopPunching() {
		if (myAnimator != null) {
			myAnimator.SetBool(ANIM_PUNCHING, false);
		}
	}
	
	void StopKicking() {
		if (myAnimator != null) {
			myAnimator.SetBool(ANIM_KICKING, false);
		}
	}
}
