using UnityEngine;
using System.Collections;

public class TouchController : MonoBehaviour {

	// Lets developer set the controller (Most likely LTouch or RTouch) in Unity
	public OVRInput.Controller Controller;

	void Update () {
		// Sets Controller Position
		transform.localPosition = OVRInput.GetLocalControllerPosition(Controller);

		// Sets Controller Rotation
		transform.localRotation = OVRInput.GetLocalControllerRotation(Controller);
	}
}
