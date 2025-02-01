using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Mic : MonoBehaviour
{
    [Tooltip("Used internally to keep track of if this object is being held right now.")]
    public bool isBeingHeld = false;
    [SerializeField]
    [Tooltip("Used internally to keep track of the interacatable component")]
    public XRGrabInteractable interactable;
    [SerializeField][Tooltip("Set automagically. Holds the soundManager reference.")]
    private SoundManager soundManager;

    void Start()
    {
        if (!soundManager)
        {
            soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        }
        if (!soundManager)
        {
            Debug.LogError("Error in Mic Prefab/Mic.cs | Unable to find SoundManager game object. This should get set automatically if there's a gameobject called SoundManager in the scene, with a SoundManager component on it.");
        }
        if (!interactable)
        {
            interactable = GetComponent<XRGrabInteractable>();
        }
        if (interactable)
        {

            interactable.onSelectEntered.RemoveListener(DidGetSelected);
            interactable.onSelectExited.RemoveListener(DidLoseSelected);
            interactable.onSelectEntered.AddListener(DidGetSelected);
            interactable.onSelectExited.AddListener(DidLoseSelected);

            // we can't set references to an external component's function on a prefab, so we have to do this here. We just need hover, because on hover it binds the rest of the events automatically.
            interactable.onHoverEntered.AddListener(soundManager.ResolveInteractionSounds);
        } else
        {
            Debug.Log("Prepared non-interactable mic. Probably this is what you intended, but you won't be able to move this mic or interact with it.");
        }

    }

    public void DidGetSelected(XRBaseInteractor interactor)
    {
        isBeingHeld = true;
        // this might be redundant with the onHover listener. Should test that.
        //soundManager.ResolveInteractionSounds(interactor);
    }

    public void DidLoseSelected(XRBaseInteractor interactor)
    {
        isBeingHeld = false;
        //soundManager.ResolveInteractionSounds(interactor);
    }
}
