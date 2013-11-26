﻿using UnityEngine;
using System.Collections;

//Controls player based on key/mouse input
public class PlayerBrain : Brain {
	
	public KeyCode forward;
	public KeyCode back;
	public KeyCode left;
	public KeyCode right;
	
	public float mouseSensitivity = 1f;
	
	Camera cam;
	
	override public void Assign(CharacterControl body) {
		base.Assign(body);
		CharacterControl.player = body;
	}

	override public void Kill() {
		Camera.main.transform.parent = null;
		base.Kill();
	}

	//Input to controls
	override public void Update () {
		base.Update();
		
		//Assigns camera here because it wasn't working elsewhere
		if (cam == null)
			cam = Camera.main;
		
		//Movement
		if (Input.GetKey(forward))
			body.Move(body.transform.forward);
		if (Input.GetKey(back))
			body.Move(-body.transform.forward);
		if (Input.GetKey(left))
			body.Move(-body.transform.right);
		if (Input.GetKey(right))
			body.Move(body.transform.right);
		
		if (Input.GetKeyDown(KeyCode.Space)) 
			body.Jump();
		
		if (body.IsRising() && !Input.GetKey(KeyCode.Space)) 
			body.StopRising();
		
		//Rotation - rotates character based on mouse and camera too (because body can't pitch, but camera should)
		Vector3 mouseDiff = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f) * mouseSensitivity;
		Turn(new Vector3(mouseDiff.x, -mouseDiff.y, 0f));
		cam.transform.RotateAround(body.transform.position, body.transform.right, -mouseDiff.y);
		//Locks the cursor into the screen if the screen is active and the cursor is over it
		if (new Rect(0f, 0f, Screen.width, Screen.height).Contains(Input.mousePosition)) {
			Screen.lockCursor = true;
		}
	}

	override public void TouchColorArea(ColorArea.CharColor touch) {
		Debug.Log("Touched object of color: :" + ColorArea.GetPallete(touch).ToString());
		if (color != touch) {
			color = touch;
			body.renderer.material.color = ColorArea.GetPallete(color);
		}
	}
	
	override public void TouchCharacter(CharacterControl other) {
		if (other.brain.color == color)
			other.brain.Kill();
		else
			Kill();
	}
}
