using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomPoleController : MonoBehaviour
{
    public Transform hand1; // First hand (grip point 1)
    public Transform hand2; // Second hand (grip point 2)
    public Transform boomPole; // The boom pole mesh or main body

    private Vector3 initialPoleScale; // Store the original scale of the pole
    private bool isTwoHanded; // Track if two hands are holding the pole

    void Start()
    {
        // Save the initial scale of the pole
        if (boomPole != null)
        {
            initialPoleScale = boomPole.localScale;
        }
    }

    void Update()
    {
        if (isTwoHanded && hand1 != null && hand2 != null && boomPole != null)
        {
            // Calculate the distance between the two hands
            float handDistance = Vector3.Distance(hand1.position, hand2.position);

            // Adjust the boom pole's length based on hand distance
            boomPole.localScale = new Vector3(
                initialPoleScale.x,        // Keep X scale unchanged
                handDistance / 2f,         // Scale Y proportionally to hand distance
                initialPoleScale.z         // Keep Z scale unchanged
            );
        }
    }

    public void OnSecondHandGrab(Transform secondHand)
    {
        hand2 = secondHand;
        isTwoHanded = true;
    }

    public void OnSecondHandRelease()
    {
        hand2 = null;
        isTwoHanded = false;

        // Reset the pole to its initial length
        if (boomPole != null)
        {
            boomPole.localScale = initialPoleScale;
        }
    }
}