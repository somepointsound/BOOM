using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;
using Assets.MultiAudioListener;

[Serializable]
public class SFX
{
    public enum Sounds
    {//%%%
        PickupMic, DropMic, Action, Cut, HoverGeneric, Correct,
        Dialog1, Dialog2, Dialog3, Slate,
        Mumble1, Mumble2, Mumble3, Dialog4, 
        Dialog5, Dialog6, DirectorIntro,
        DirectorScene1,
        DirectorScene2,
        DirectorScene3, DirectorScene3_1,
        Ernie1, Ernie1Mumble, SrgtRose, SrgtRoseMumble,
        Ernie2, Ernie2Mumble, BigPapa1, Hanz1,
        Cooper1, Jason1, SrgtRose2,
        Cooper1Mumble, Jason1Mumble, SrgtRose2Mumble,
        Hanz1Mumble, Hanz2, Hanz2Mumble,
        Ernie3, Ernie3Mumble,
        DirectorFail1, DirectorFail2, DirectorFail3, DirectorFail4, DirectorSuccess1, DirectorSuccess2
    };
}
public class SoundManager : MonoBehaviour
{

    /* there will be lots of sound effects and sound related stuff in this project. 
     * This seems like a good place to put some of that. My unity architecture isn't strong though
     * so if there's a preferred way, tell me! (Noah)
     * Also having a single game object that can manage audio state is useful since audio is tricky
     * */


    // internal variables
    [SerializeField][Tooltip("Used internally.")] 
    private MultiAudioSource audioSource;
    [SerializeField][Tooltip("Used internally.")]
    private AudioClip audioClip;
    [SerializeField][Tooltip("Used internally.")]
    public List<XRBaseInteractable> hoverTargets;

    // set these publicly
    [Tooltip("IMPORTANT! This is where we link up all the ConeZones for our characters. Increase the size to the number of 'talkers' you have, make sure they have SoundConeManager components on them, and then drag them into here to link them.")]
    public List<SoundConeManager> characters;
    [Tooltip("Used internally to determine the last time a sound effect was played. See soundEffectDebounce")]
    private float soundEffectDebounceLastTime;
    [Tooltip("Min time to wait before playing a sound effect again")]
    public float soundEffectDebounce = 0.75f;

    // %%% Audio Clip Definitions. Add to these.
    /* to add a new sound effect, you have to add it in SoundEffects.cs, and also like 4 places in this file. I've marked all the places with // %%% */
    public AudioClip PickupMic;
    public AudioClip DropMic;
    public AudioClip Action;
    public AudioClip Cut;
    public AudioClip HoverGeneric;
    public AudioClip Correct;
    public AudioClip Dialog1;
    public AudioClip Mumble1;
    public AudioClip Dialog2;
    public AudioClip Mumble2;
    public AudioClip Dialog3;
    public AudioClip Mumble3;
    public AudioClip Slate;
    public AudioClip Dialog4;
    public AudioClip Dialog5;
    public AudioClip Dialog6;
    public AudioClip DirectorIntro;
    public AudioClip DirectorScene1;
    public AudioClip DirectorScene2;
    public AudioClip DirectorScene3;
    public AudioClip DirectorScene3_1;
    public AudioClip Ernie1;
    public AudioClip Ernie2;
    public AudioClip Ernie1Mumble;
    public AudioClip Ernie2Mumble;
    public AudioClip BigPapa1;
    public AudioClip Hanz1;
    public AudioClip SrgtRose;
    public AudioClip SrgtRoseMumble;
    public AudioClip Cooper1;
    public AudioClip Jason1;
    public AudioClip SrgtRose2;
    public AudioClip Cooper1Mumble;
    public AudioClip Jason1Mumble;
    public AudioClip SrgtRose2Mumble;
    public AudioClip Hanz1Mumble;
    public AudioClip Hanz2;
    public AudioClip Hanz2Mumble;
    public AudioClip Ernie3;
    public AudioClip Ernie3Mumble;
    public AudioClip DirectorFail1;
    public AudioClip DirectorFail2;
    public AudioClip DirectorFail3;
    public AudioClip DirectorFail4;
    public AudioClip DirectorSuccess1;
    public AudioClip DirectorSuccess2;

    public void Start()
    {
        // set the ids for characters that were defined
        for (int i = 0; i < characters.Count; i++)
        {
            //Debug.Log("Iterating over defined chars and settings ids: " + i);
            characters[i].charID = i;
        }

        soundEffectDebounceLastTime = 0f;
    }

    /* 
     * Use this function to play sounds. Right now it's not actually queueing them, but it's good to have a separation layer so we can do it later if we need
     */
    public void QueSound(SFX.Sounds clipName)
    {
        AudioClip resolvedAudioClip = ResolveSoundToClip(clipName);
        if (resolvedAudioClip)
        {
            PlaySoundEffect(resolvedAudioClip);
        }
    }

    public AudioClip ResolveSoundToClip(SFX.Sounds clipName)
    {
        AudioClip clip = null;
        // %%%
        switch (clipName)
        {
            case SFX.Sounds.PickupMic:
                clip = this.PickupMic;
                break;
            case SFX.Sounds.DropMic:
                clip = this.DropMic;
                break;
            case SFX.Sounds.Action:
                clip = this.Action;
                break;
            case SFX.Sounds.Cut:
                clip = this.Cut;
                break;
            case SFX.Sounds.HoverGeneric:
                clip = this.HoverGeneric;
                break;
            case SFX.Sounds.Correct:
                clip = this.Correct;
                break;
            case SFX.Sounds.Dialog1:
                clip = this.Dialog1;
                break;
            case SFX.Sounds.Dialog2:
                clip = this.Dialog2;
                break;
            case SFX.Sounds.Dialog3:
                clip = this.Dialog3;
                break;
            case SFX.Sounds.Mumble1:
                clip = this.Mumble1;
                break;
            case SFX.Sounds.Mumble2:
                clip = this.Mumble2;
                break;
            case SFX.Sounds.Mumble3:
                clip = this.Mumble3;
                break;
            case SFX.Sounds.Slate:
                clip = this.Slate;
                break;
            case SFX.Sounds.Dialog4:
                clip = this.Dialog4;
                break;
            case SFX.Sounds.Dialog5:
                clip = this.Dialog5;
                break;
            case SFX.Sounds.Dialog6:
                clip = this.Dialog6;
                break;
            case SFX.Sounds.DirectorIntro:
                clip = this.DirectorIntro;
                break;
            case SFX.Sounds.DirectorScene2:
                clip = this.DirectorScene2;
                break;
            case SFX.Sounds.DirectorScene3:
                clip = this.DirectorScene3;
                break;
            case SFX.Sounds.DirectorScene1:
                clip = this.DirectorScene1;
                break;
            case SFX.Sounds.DirectorScene3_1:
                clip = this.DirectorScene3_1;
                break;
            case SFX.Sounds.Ernie1:
                clip = this.Ernie1;
                break;
            case SFX.Sounds.Ernie2:
                clip = this.Ernie2;
                break;
            case SFX.Sounds.Ernie1Mumble:
                clip = this.Ernie1Mumble;
                break;
            case SFX.Sounds.Ernie2Mumble:
                clip = this.Ernie2Mumble;
                break;
            case SFX.Sounds.SrgtRoseMumble:
                clip = this.SrgtRoseMumble;
                break;
            case SFX.Sounds.SrgtRose:
                clip = this.SrgtRose;
                break;
            case SFX.Sounds.BigPapa1:
                clip = this.BigPapa1;
                break;
            case SFX.Sounds.Hanz1:
                clip = this.Hanz1;
                break;
            case SFX.Sounds.Cooper1:
                clip = this.Cooper1;
                break;
            case SFX.Sounds.Cooper1Mumble:
                clip = this.Cooper1Mumble;
                break;
            case SFX.Sounds.Jason1:
                clip = this.Jason1;
                break;
            case SFX.Sounds.Jason1Mumble:
                clip = this.Jason1Mumble;
                break;
            case SFX.Sounds.SrgtRose2:
                clip = this.SrgtRose2;
                break;
            case SFX.Sounds.SrgtRose2Mumble:
                clip = this.SrgtRose2Mumble;
                break;
            case SFX.Sounds.Hanz1Mumble:
                clip = this.Hanz1Mumble;
                break;
            case SFX.Sounds.Hanz2:
                clip = this.Hanz2;
                break;
            case SFX.Sounds.Hanz2Mumble:
                clip = this.Hanz2Mumble;
                break;
            case SFX.Sounds.Ernie3:
                clip = this.Ernie3;
                break;
            case SFX.Sounds.Ernie3Mumble:
                clip = this.Ernie3Mumble;
                break;
            case SFX.Sounds.DirectorFail1:
                clip = this.DirectorFail1;
                break;
            case SFX.Sounds.DirectorFail2:
                clip = this.DirectorFail2;
                break;
            case SFX.Sounds.DirectorFail3:
                clip = this.DirectorFail3;
                break;
            case SFX.Sounds.DirectorFail4:
                clip = this.DirectorFail4;
                break;
            case SFX.Sounds.DirectorSuccess1:
                clip = this.DirectorSuccess1;
                break;
            case SFX.Sounds.DirectorSuccess2:
                clip = this.DirectorSuccess2;
                break;

        }
        if (clip)
        {
            return clip;
        }
        return null;
    }

    public void SetCharacterAudio(int charID, SFX.Sounds clipName)
    {
        AudioClip resolvedAudioClip = ResolveSoundToClip(clipName);
        // as long as it returns something
        if (resolvedAudioClip)
        {
            //Debug.Log("SetCharacterAudio| resolvedAudioClip successfully");
            // ensure the char exists
            if (characters[charID])
            {
                //Debug.Log("SetCharacterAudio| characters["+charID+"] exists");
                // the array is called characters, but it's actually an array of SoundConeManager's. So, we have to get the component that actually is making the sound, which is defined as it's talkyTalky gameObject.
                // The talkyTalky is usually an invisible sphere at the charactors mouth. Find that, and then get it's audioSource, which plays the sound.
                MultiAudioSource charAudioSource = characters[charID].talkyTalky?.gameObject.GetComponent<MultiAudioSource>();
                if (!charAudioSource) 
                {
                    // NO audio source yet exists. make one
                    characters[charID].talkyTalky.gameObject.AddComponent<MultiAudioSource>();
                    charAudioSource = characters[charID].talkyTalky.gameObject.GetComponent<MultiAudioSource>();
                    charAudioSource.Loop = true;
                    //charAudioSource.PlayOnAwake = true; *** testing
                    //charAudioSource.SpatialBlend = 1; // needed for normal AudioSource but not necessary for MultiAudioSource, it's the default and only supported mode.
                    charAudioSource.Spread = 212;
                    charAudioSource.VolumeRolloff = AudioRolloffMode.Logarithmic; // cant set this to custom with scripting, so i guess we'll have to make sure we make these manually.
                   // charAudioSource.Play();
                    Debug.LogWarning("WARNING: Creating a new audio source with Logarithmic rollof on gameobject (" + characters[charID].talkyTalky.gameObject.name + ") because there isnt one. However, we cant set the volume rollOf mode in code, so you're better of making an audioSource on the talkyTalky yourself. ");
                }
                // set the audio Clip the defined clip.
                //Debug.Log("SetCharacterAudio| setting charAudioSource to resolvedAudioClip: "+resolvedAudioClip.name + "(clipName: "+ clipName.ToString()+ ")");
                if (charAudioSource.IsPlaying)
                {
                    // TODO: Add this back in when we have a working setup. IMPORTANT, but i'm debugging right now.
                    float trackPosition = charAudioSource.TimePosition;
                    //Debug.Log("SoundManager.cs | charAudioSource is already playing, so saving track position of " + trackPosition.ToString());
                    charAudioSource.AudioClip = resolvedAudioClip;
                    charAudioSource.SetTimePosition(trackPosition);
                } else
                {
                    // not playing, so just load it up and (dont) hit play.
                    charAudioSource.AudioClip = resolvedAudioClip;
                    Debug.Log("SoundManager.cs | charAudioSource is Not Playing. Setting AudioClip and hitting play");
                    charAudioSource.Play();
                }

            } else
            {
                Debug.Log("Error in SoundManager.cs: Char with id " + charID + " not found.");
            }
        }
    }

    private void PlaySoundEffect(AudioClip audioClip)
    {
        if (audioSource = gameObject.GetComponent<MultiAudioSource>())
        {
            audioSource.AudioClip = audioClip;
            // TODO: Re-implement PlayOneShot on the MultiAudioSource.
            audioSource.Play();
            audioSource.Loop = false;
        } else { 
            audioSource = gameObject.AddComponent<MultiAudioSource>();
            audioSource.Loop = false;
            audioSource.PlayOnAwake = false;

            audioSource.AudioClip = audioClip;
            audioSource.Loop = false;
            audioSource.Play();
        }
        
    }

 
    public void ResolveInteractionSounds(XRBaseInteractor controller)
    {

        // this stuff whole function doesn't get called until after the event handlers have already been done, 
        // so for hover, we have to check if it's happening right now. At that point we might as well bind the events

        // okay it seems like this only happens when we are in a hover state, and therefor target is null.

        controller.GetHoverTargets(hoverTargets);
        foreach (XRBaseInteractable target in hoverTargets)
        {
            //Debug.Log("hoverTarget: " + target.name);
            SoundEffects objectSFX = target?.GetComponent<SoundEffects>() ?? null;

            // %%%
            SFX.Sounds pickupSound = objectSFX.pickupSound;
            SFX.Sounds dropSound = objectSFX.dropSound;
            SFX.Sounds activateSound = objectSFX.activateSound;
            SFX.Sounds hoverSound = objectSFX.hoverSound;
            SFX.Sounds correctSound = objectSFX.correctSound;
            SFX.Sounds dialog1Sound = objectSFX.dialog1Sound;
            SFX.Sounds dialog2Sound = objectSFX.dialog2Sound;
            SFX.Sounds dialog3Sound = objectSFX.dialog3Sound;
            SFX.Sounds mumble1Sound = objectSFX.mumble1Sound;
            SFX.Sounds mumble2Sound = objectSFX.mumble2Sound;
            SFX.Sounds mumble3Sound = objectSFX.mumble3Sound;
            SFX.Sounds slateSound = objectSFX.slateSound;
            SFX.Sounds Dialog4 = objectSFX.Dialog4;
            SFX.Sounds Dialog5 = objectSFX.Dialog5;
            SFX.Sounds Dialog6 = objectSFX.Dialog6;
            SFX.Sounds directorIntro = objectSFX.directorIntro;
            SFX.Sounds directorScene1 = objectSFX.directorScene1;
            SFX.Sounds directorScene2 = objectSFX.directorScene2;
            SFX.Sounds directorScene3 = objectSFX.directorScene3;
            SFX.Sounds directorScene3_1 = objectSFX.directorScene3_1;
            SFX.Sounds ernie1 = objectSFX.ernie1;
            SFX.Sounds ernie2 = objectSFX.ernie2;
            SFX.Sounds bigPapa1 = objectSFX.bigPapa1;
            SFX.Sounds hanz1 = objectSFX.hanz1;
            SFX.Sounds srgtRose = objectSFX.srgtRose;
            SFX.Sounds srgtRoseMumble = objectSFX.srgtMumble;
            SFX.Sounds cooper1 = objectSFX.cooper1;
            SFX.Sounds cooper1Mumble = objectSFX.cooper1Mumble;
            SFX.Sounds jason1 = objectSFX.jason1;
            SFX.Sounds jason1Mumble = objectSFX.jason1Mumble;
            SFX.Sounds srgtRose2 = objectSFX.srgtRose2;
            SFX.Sounds srgtRose2Mumble = objectSFX.srgtRose2Mumble;
            SFX.Sounds hanz1Mumble = objectSFX.hanz1Mumble;
            SFX.Sounds hanz2 = objectSFX.hanz2;
            SFX.Sounds hanz2Mumble = objectSFX.hanz2Mumble;
            SFX.Sounds ernie3 = objectSFX.ernie3;
            SFX.Sounds ernie3Mumble = objectSFX.ernie3Mumble;
            SFX.Sounds directorFail1 = objectSFX.directorFail1;
            SFX.Sounds directorFail2 = objectSFX.directorFail2;
            SFX.Sounds directorFail3 = objectSFX.directorFail3;
            SFX.Sounds directorFail4 = objectSFX.directorFail4;
            SFX.Sounds directorSuccess1 = objectSFX.directorSuccess1;
            SFX.Sounds directorSuccess2 = objectSFX.directorSuccess2;



            // we are hovering, so play the hover sound.
            if (objectSFX != null)
            {
                QueSound(objectSFX.hoverSound);
            }
         
            // now bind the rest of the sounds to this object that's currently hovered.
            controller.onHoverEntered.RemoveAllListeners();
            controller.onHoverExited.RemoveAllListeners();
            controller.onSelectEntered.RemoveAllListeners();
            controller.onSelectExited.RemoveAllListeners();

            // then, wire this up so when we drop it it knows what sound to play. Yay for closures.
            controller.onHoverExited.AddListener((XRBaseInteractable item) =>
            {
                if (item)
                {
                    //Debug.Log("onHoverExit: " + item.gameObject.name);
                    QueSound(hoverSound);
                    controller.onHoverEntered.RemoveAllListeners();
                    controller.onHoverExited.RemoveAllListeners();
                }
                else
                {
                    Debug.Log("onHoverExit of unknown item ");
                }
            });
            controller.onHoverEntered.AddListener((XRBaseInteractable item) =>
            {
                if (item)
                {
                    Debug.Log("onHoverEnter: " + item.gameObject.name);
                    QueSound(hoverSound);
                }
                else
                {
                    Debug.Log("onHoverEnter of unknown item ");
                }
            });
            controller.onSelectEntered.AddListener((XRBaseInteractable item) =>
            {
                if (item)
                {
                    //Debug.Log("onSelectEnter item " + item.gameObject.name);
                    QueSound(pickupSound);
                } else
                {
                    Debug.Log("onSelectEnter unknown item ");
                }
            });
            controller.onSelectExited.AddListener((XRBaseInteractable item) =>
            {
                if (item)
                {
                    //Debug.Log("onSelectExit: " + item.gameObject.name);
                    QueSound(dropSound);
                    controller.onHoverEntered.RemoveAllListeners();
                    controller.onHoverExited.RemoveAllListeners();
                    controller.onSelectEntered.RemoveAllListeners();
                    controller.onSelectExited.RemoveAllListeners();
                }
                else
                {
                    Debug.Log("onSelectExit of unknown item ");
                }
            });
        }

    }


    // kind of a utility class. Should probably put this in the SoundManager when I do that migration.
    public bool CheckSoundEffectDebounce()
    {
        // if the current time is greater than the last time we played a sound + the defined minimum sound interval, then...
        if (Time.time > soundEffectDebounceLastTime + soundEffectDebounce)
        {
            soundEffectDebounceLastTime = Time.time;
            return true;
        }
        return false;
    }
}
