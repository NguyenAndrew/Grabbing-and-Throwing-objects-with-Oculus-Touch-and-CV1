﻿using UnityEngine;
using System.Collections;

public class Grab : MonoBehaviour
{
    public OVRInput.Controller controller;
    public string buttonName;
    public float grabRadius;
    public LayerMask grabMask;

    private GameObject grabbedObject;
    public GameObject otherHand;

    private bool grabbing;

    private Quaternion lastRotation, currentRotation;

    void GrabObject()
    {
        grabbing = true;

        // Using "Sphere Casting/Ray Casting" to grab object
        RaycastHit[] hits;

        /* 
         * Quick summary of the paramaters of SphereCastAll:
         * 1. Position of Sphere Case
         * 2. Radius of sphere
         * 3. Direction of sphere. We just want to ave sphere appear where fist is, so direction does not matter
         * 4. Length of the ray. Again, we don't care about this
         * 5. Layer Mask. Says what layer we want the grab to work on.
         */
        hits = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, grabMask);

        // Check to see, if any objects was caught in sphere cast
        if (hits.Length > 0)
        {
            // Incase more than one object hit, grabs the closest hit
            int closestHitInd = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < hits[closestHitInd].distance) closestHitInd = i;
            }
            grabbedObject = hits[closestHitInd].transform.gameObject;

            // Block of Code: Removes object from other hand, if this hand is taking it.
            if (otherHand != null)
            {
                // Makes sure the hand contains the grab script
                if (otherHand.GetComponent<Grab>() == null)
                {
                    Debug.LogError("Hands must have the grab script!");
                }
                else if (otherHand.GetComponent<Grab>().grabbedObject == this.grabbedObject)
                {
                    otherHand.GetComponent<Grab>().DropObject();

                    // grabbing needs to be set, because the other hand is still grabbing the trigger.
                    // Prevents the object bouncing back to the hand the object was taken from
                    otherHand.GetComponent<Grab>().grabbing = true;
                }
            }

            // Disables forces on the grabbed object
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            // Moves grabbed object to position of hand 
            grabbedObject.transform.position = transform.position;
            // Locks grabbed object to fist
            grabbedObject.transform.parent = transform;
        }
    }

    void GrabObject(GameObject grabbedObject)
    {
        grabbing = true;

        // Disables forces on the grabbed object
        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;

        // Moves grabbed object to position of hand 
        grabbedObject.transform.position = transform.position;

        // Locks grabbed object to fist
        grabbedObject.transform.parent = transform;
    }

    void DropObject()
    {
        grabbing = false;

        // Check to see if we are actually grabbing an object
        if (grabbedObject != null)
        {
            // Remove object from this hand
            grabbedObject.transform.parent = null;

            // Reapply forces to grabbed object
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;

            // Apply velocities to object that is let go.
            grabbedObject.GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(controller);
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = GetAngularVelocity();

            grabbedObject = null;
        }
    }

    /// <summary>
    /// Used to calculate the angular velocity, when letting go of an object
    /// </summary>
    /// <returns>A Vector3 of the object</returns>
    Vector3 GetAngularVelocity()
    {
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(lastRotation);
        return new Vector3(Mathf.DeltaAngle(0, deltaRotation.eulerAngles.x), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.y), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.z));
    }

    // Update is called once per frame
    void Update()
    {
        // Update grabbed object velocity, when grabbing
        if (grabbedObject != null)
        {
            lastRotation = currentRotation;
            currentRotation = grabbedObject.transform.rotation;
        }

        // Grabs object on push of Hand Trigger
        if (!grabbing && Input.GetAxis(buttonName) == 1)
        {
            // Debug.Log("Trigger is being pushed.");
            GrabObject();
        }


        // Release object on release of Hand Trigger
        if (grabbing && Input.GetAxis(buttonName) < 1)
        {
            // Debug.log("Trigger was released!");
            DropObject();
        }
    }
}