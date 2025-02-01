using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class GreenMan : MonoBehaviour
{
    private class BoneTransform
    {
        public Vector3 Position {get; set;}

        public Quaternion Rotation { get; set;}
    }
    private enum ChattingState
    {
        Chatting,
        Ragdoll,
        StandingUp
    }

    [SerializeField]
    private string _standUpStateName;



    private Rigidbody[] _ragdollRigidbodies;
    private ChattingState _currentState = ChattingState.Chatting;
    private Animator _animator;
    private CharacterController _characterController;
    private float _timeToWakeUp;
    private Transform _hipBone;

    void Awake()
    {
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _hipBone = _animator.GetBoneTransform(HumanBodyBones.Hips);

        DisableRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case ChattingState.Chatting:
                ChattingBehavior();
                break;
            case ChattingState.Ragdoll:
                RagdollBehavior();
                break;
            case ChattingState.StandingUp:
                StandingUpBehavior();
                break;
        }
    }

    private void StandingUpBehavior()
    {
       if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_standUpStateName) == false)
       {
            _currentState = ChattingState.Chatting;
       }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BoomPole"))
        {
            EnableRagdoll();
            
            
            _timeToWakeUp = Random.Range(5, 10);
            _currentState = ChattingState.Ragdoll;  // Change state to Ragdoll
        }
    }

    private Rigidbody FindHitRigidbody(Vector3 hitPoint)
    {
        Rigidbody closestRigidbody = null;
        float closestDistance = 0;

        foreach (var rigidbody in _ragdollRigidbodies)
        {
            float distance = Vector3.Distance(rigidbody.position, hitPoint);

            if (closestRigidbody == null || distance < closestDistance)
            {
                closestDistance = distance;
                closestRigidbody = rigidbody;
            }
        }

        return closestRigidbody;
    }
    private void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }

        _animator.enabled = true;
        _characterController.enabled = true;
    }

    private void EnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        _animator.enabled = false;
        _characterController.enabled = false;
    }

    private void ChattingBehavior()
    {
        // Define behavior for the Chatting state here if needed
    }

    private void RagdollBehavior()
    {
        _timeToWakeUp -= Time.deltaTime;

        if (_timeToWakeUp <= 0)
        {
            AlignPositionToHips();
            _currentState = ChattingState.StandingUp;
            DisableRagdoll();

            _animator.Play(_standUpStateName);
        }
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipBone.position; 
        transform.position = _hipBone.position;

       if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
       {
              transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
       }
        _hipBone.position = originalHipsPosition;
    }
}
