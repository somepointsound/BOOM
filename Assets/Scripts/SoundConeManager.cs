using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundConeManager : MonoBehaviour
{

    // fields used internally only
    [SerializeField][Tooltip("Set automagically.")]
    private GameObject collidingWith;
    [SerializeField][Tooltip("Set automagically.")]
    private bool inTheConeZone = false;
    [Tooltip("Is set to true while the mic is in the cone zone, and the raycast is reporting that the mic is pointed at the talkytalky. Check this every frame, and you can get a picture of how well they've done.")]
    public bool goal = false;
    [SerializeField][Tooltip("Set automagically.")]
    private Transform microphonePickup;
    [SerializeField][Tooltip("Set automagically.")]
    private Ray rayFromMic;
    private SoundManager soundManager;
    [Tooltip("Set automagically. Charactor id. Should be a unique integer. Automatically assigned by the SoundManager, as long as each SoundConeManager is in the character list on the SoundManager game object. If it's showing 999, you didn't link it correctly. This is how we keep track of the different 'goals', the audioTracks, etc. If you're having weird audio behavior,  make sure these ids are all different, and i think they need to be sequential.")]
    public int charID = 999;


    // can be set manually, but have automatic values
    [Tooltip("Drag the transform from Talky Talky here. Otherwise it'll be automatically set to the parent gameobject's transform.")]
    public Transform talkyTalky;
    [Tooltip("Perfect Mic Distance. Aka the 'perfect distance'.")]
    public float perfectDistance = 0.2f;
    [Tooltip("Critical hit is perfectDistance +/- perfectDistanceAllowedVariancePercent")]
    public float perfectDistanceAllowedVariancePercent = 0.1f;
    //[Tooltip("DEPRECATED. I should move this. (Tell us where the director is, so that we can make sounds come from him)")]
    //public GameObject director;
    [Tooltip("The main audio track you'd like to play from this object")]
    public SFX.Sounds primaryAudioTrack;
    [Tooltip("The mumble track, to play when the object isnt properly mic'd")]
    public SFX.Sounds mumbleAudioTrack;


    void Awake()
    {
        if (talkyTalky == null)
        {
            talkyTalky = transform.parent.GetComponent<Transform>();
        }
        if (!talkyTalky)
        {
            Debug.LogError("Error in SoundConeManager.cs | Unable to find talkyTalky object. It should be the parent of the ConeZone object, and it must have a transform.");
        }
        if (soundManager == null)
        {
            soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>() ?? null;
        }
    }
    public void OnTriggerEnter(Collider collider)
    {
        collidingWith = collider.gameObject;
        Debug.Log("colliding with " + collider.name);

        // let's determine how well they did at nailing the position.
        // the ideal position should be in the center of the waveform cone (ConeZone), pointing at the sound generator (TalkyTalky)
        // we just need to draw a triangle between these three points, and then look at the angles.
        // Scratch that, that's too much work, let's just compare the transform distance between the talkyTalky and the mic.
        inTheConeZone = true;
        PlayPrimaryAudio(); // switches the audio track to the correct one.

    }

    public void OnTriggerExit(Collider collider)
    {
        inTheConeZone = false;
        Mumble(); // outside the cone it sounds like mumbling
    }

    void Update()
    {
        if (inTheConeZone)
        {
            CheckIfRayCastHit();
        } else
        {
            goal = false;
        }
    }

    public void PlayPrimaryAudio()
    {
        // play the primary audio track
        //Debug.Log("SoundConeManager::PlayPrimaryAudio => soundManager.SetCharacterAudio(" + this.charID + ", primaryAudioTrack: "+ primaryAudioTrack.ToString()+")");
        soundManager.SetCharacterAudio(this.charID, primaryAudioTrack);
    }

    public void Mumble()
    {
        // play the mumble audio track
        //Debug.Log("SoundConeManager::Mumble => soundManager.SetCharacterAudio(" + this.charID + ", mumbleAudioTrack: "+ mumbleAudioTrack.ToString()+")");
        soundManager.SetCharacterAudio(this.charID, mumbleAudioTrack);
    }

    public void CheckIfRayCastHit()
    {
        //Debug.Log("CheckIfRayCastHit running...");
        microphonePickup = collidingWith.transform;
        // NOTE: We could add rotation here as well, but at the moment I think we only care about rotation along two of the three axis, and also it might be too hard.
        rayFromMic = new Ray(microphonePickup.position, microphonePickup.forward);
        RaycastHit hit;
            if (Physics.Raycast(rayFromMic, out hit))
            {
#if UNITY_EDITOR
            Debug.DrawRay(microphonePickup.position, microphonePickup.forward, Color.green, 5);
#endif
            if (hit.collider.gameObject.name == talkyTalky.gameObject.name)
            {
                //Debug.Log("Hitting: " + hit.collider.gameObject.name + ". Looking for: " + talkyTalky.gameObject.name + "; Distance is: " + hit.distance);
                //Debug.DrawRay(microphonePickup.position, microphonePickup.forward, Color.red, 5);
                // let's see how far away it is, and compute the floor and ceiling for our sweet-spot. Give them a reward if they hit it.
                var perfectDistanceMin = perfectDistance + (-1 * perfectDistanceAllowedVariancePercent);
                var perfectDistanceMax = perfectDistance + (1 * perfectDistanceAllowedVariancePercent);
                if (hit.distance > perfectDistanceMin && hit.distance < perfectDistanceMax)
                {
                    //Debug.Log("Critical Hit!" + "Distance is: " + hit.distance);
                    PerfectPositionHit(microphonePickup.gameObject, hit.collider.gameObject);
                    goal = true;
                } else
                {
                    //Debug.Log("Not in the sweet spot.");
                    goal = false;
                }
            }
            }


    }


    public void PerfectPositionHit(GameObject whichMic, GameObject whichTalker)
    {
        // if they've got the perfect position, and they're actively holding the mic, let them know.
        if (whichMic.GetComponentInParent<Mic>().isBeingHeld)
        {
            // make sure we don't play it too often.
            if (soundManager.CheckSoundEffectDebounce())
            {
                Debug.Log("Perfect position hit! Mic was: " + whichMic.name + ". Talker was :" + whichTalker.transform.parent.gameObject.name);
                soundManager.QueSound(whichMic.GetComponentInParent<SoundEffects>().correctSound);
            }
        }
    }



}