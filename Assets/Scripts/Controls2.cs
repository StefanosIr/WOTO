using UnityEngine;
using System.Collections;

public class Controls2 : MonoBehaviour {
	
	private Animator myAnimator;
	
	// Use this for initialization
	void Start () {
		myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		myAnimator.SetFloat ("VSpeed", Input.GetAxis ("Vertical2"));
		myAnimator.SetFloat ("HSpeed", Input.GetAxis ("Horizontal2"));
		
		if(Input.GetButtonDown ("Jump2")){
			myAnimator.SetBool ("Jumping", true);
			Invoke ("StopJumping", 0.1f); //calls the invoke after a specified time
		}


		if(Input.GetButtonDown ("Fire1p")){
			myAnimator.SetBool ("Punching", true);
			Invoke ("StopPunching", 0.5f); //calls the invoke after a specified time
		}
		
		if(Input.GetButtonDown ("Fire2p")){
			myAnimator.SetBool ("Kicking", true);
			Invoke ("StopKicking", 0.5f); //calls the invoke after a specified time
		}
		
		if(Input.GetKey("[")){
			transform.Rotate (Vector3.down * Time.deltaTime * 100.0f);
			if((Input.GetAxis ("Vertical2") == 0f) && (Input.GetAxis ("Horizontal2") == 0)){ //check if moving
				myAnimator.SetBool ("TurningLeft", true);
			}
			
		} else {
			myAnimator.SetBool ("TurningLeft", false);
		}
		
		
		if(Input.GetKey("]")){
			transform.Rotate (Vector3.down * Time.deltaTime * -100.0f);
			if((Input.GetAxis ("Vertical2") == 0f) && (Input.GetAxis ("Horizontal2") == 0)){
				myAnimator.SetBool ("TurningRight", true);
			}
		} else {
			myAnimator.SetBool ("TurningRight", false);
		}
		
		if(Input.GetKeyDown ("'")){
			if(myAnimator.GetInteger("CurrentAction") == 0){
				myAnimator.SetInteger("CurrentAction", 1);				
			} else if (myAnimator.GetInteger ("CurrentAction") == 1){
				myAnimator.SetInteger ("CurrentAction", 0);
			}
		}
		
		if(Input.GetKeyDown (";")){
			if(myAnimator.GetInteger ("CurrentAction") == 0){
				myAnimator.SetInteger ("CurrentAction", 2);				
			} else if (myAnimator.GetInteger ("CurrentAction") == 2){
				myAnimator.SetInteger ("CurrentAction", 0);
			}
		}
		
		if(Input.GetKeyDown ("3")){
			myAnimator.SetLayerWeight (1, 1f);
			myAnimator.SetInteger("CurrentAction", 3);
		}
		
		if(Input.GetKeyUp ("3")){
			myAnimator.SetInteger ("CurrentAction", 0);
		}
		
	}
	
	void StopJumping(){
		myAnimator.SetBool ("Jumping", false);
	}
	void StopPunching(){
		myAnimator.SetBool ("Punching", false);
	}
	
	void StopKicking(){
		myAnimator.SetBool ("Kicking", false);
	}
	
}
