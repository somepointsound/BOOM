using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudioSetup : MonoBehaviour
{

    // can be set manually, but auto-filled magically.
    [Tooltip("Set magically. Or, Drag all the cone zones in here.")]
    public SoundConeManager[] characterAudioTracks;
    public SoundManager soundManager;

    void Start()
    {
        // make sure we have a soundmanager reference
        if (soundManager == null)
        {
            Debug.Log("No sound manager specified, trying to find one by looking for gameobjects named 'SoundManager'");
            soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        }
        // make sure we have audio tracks specified.
        if (characterAudioTracks.Length < 1)
        {
            Debug.Log("No Character Audio Tracks Defined. Gonna try to figure this out automatically.");
            characterAudioTracks = GameObject.FindObjectsOfType<SoundConeManager>();
        }
        if (soundManager == null)
        {
            Debug.LogError("Error in CharacterAudioSetup | No sound manager specified. Character audio won't work.");
        }
        if (characterAudioTracks.Length < 1)
        {
            Debug.LogError("Error in CharacterAudioSetup | No characterAudioTracks specified. Character audio won't work.");
        }
        //StartAllCharacterAudioTracks();
    }
 
    public void StartAllCharacterAudioTracks()
    {
        // we are starting up, so call SetCharacterAudio for each of the audio tracks.
        foreach (SoundConeManager characterAudioTrack in characterAudioTracks)
        {
            // get which audio tracks from the soundconemounager so we can attach it
            var charID = characterAudioTrack.charID;
            var charSoundClip = characterAudioTrack.mumbleAudioTrack;
            Debug.Log("Setting character audio for " + characterAudioTrack.gameObject.transform.parent.transform.parent.gameObject.name + " to " + characterAudioTrack.mumbleAudioTrack);
            soundManager.SetCharacterAudio(charID, charSoundClip);
        }
    }
}
