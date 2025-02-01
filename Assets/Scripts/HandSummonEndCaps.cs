using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSummonEndCaps : MonoBehaviour
{
    public delegate void EventHandler(GameObject collider);
    public delegate void EndEventHandler(GameObject collider);
    public event EventHandler EndcapCollisionStart;
    public event EndEventHandler EndcapCollisionEnd;

    // Checking a reference to a collider is better than using strings.
    [SerializeField] GameObject collidingWith;

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("HandSummonEndCaps.cs::OnTriggerEnter: Setting collidingWith: " + collider.gameObject.name);
        collidingWith = collider.gameObject;
        EndcapCollisionStart(collidingWith);
    }

    private void OnTriggerExit(Collider collider)
    {
        //Debug.Log("HandSummonEndCaps.cs::OnTriggerExit: Setting collidingWith: " + collider.gameObject.name);
        collidingWith = collider.gameObject;
        EndcapCollisionEnd(collidingWith);
    }


}
