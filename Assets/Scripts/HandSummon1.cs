using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandSummon : MonoBehaviour
{
    // must be set by hand
    [Tooltip("The material to use to indicate the user screwed up")]
    public Material errorMaterial;
    [Tooltip("The normal material, so we can switch back to it")]
    public Material okMaterial;


    // can be set, but populated automatically too.
    [Tooltip("Set automagically. Left hand controller game object. If empty it'll just look for a gameobject called 'LeftHand Controller'")]
    public XRBaseControllerInteractor l_hand;
    [Tooltip("Set automagically. Right hand controller game object. If empty it'll just look for a gameobject called 'RightHand Controller'")]
    public XRBaseControllerInteractor r_hand;
    [Tooltip("Set automagically.")]
    private XRRig xrRig;
#pragma warning disable 649
    [Tooltip("The colliders that you must slide yours hands to, in order to summon")]
    private List<Collider> endCapColliders = new List<Collider>();
#pragma warning restore 649
    [Tooltip("Set automagically. The game object to show when showing the tutorial. Has all the children.")]
    [SerializeField]
    private GameObject tutorial;
    [Tooltip("Set automagically. The game object in the tutorial, which we change the material in.")]
    [SerializeField]
    private GameObject tutorialBar;
    [Tooltip("The intensity of the vibrations triggered with hand summoning")]
    private float hapticIntensity = 0.2f;
    [Tooltip("Duration for fading when stuff fades")]
    float lerpDuration = 0.5f;

    // used internally
    [SerializeField][Tooltip("Used internally. dont touch")]
    private List<XRController> xrControllers = null;    // basically the same as the other list, but these ones get set internally to the XRController component, which we often need for haptic pulses and stuff like that. Feels redundant.
    [SerializeField]
    private bool leftHandBegin = false;
    [SerializeField]
    private bool rightHandBegin = false;
    [SerializeField]
    private bool leftHandDone = false;
    [SerializeField]
    private bool rightHandDone = false;
    [SerializeField]
    private float debounceLastTime = 0;
    [SerializeField]
    private bool step1 = false;
    [SerializeField]
    private bool step2 = false;
    [SerializeField]
    private float lerpStart = 0;
    [SerializeField]
    private bool errorIndicationHappening = false;
    [SerializeField]
    private bool instantiateIndicationHappening = false; 



    public void Start()
    {
        if (!xrRig)
        {
            xrRig = GameObject.Find("XR Rig").GetComponent<XRRig>() ?? null;
        }
        if (!xrRig)
        {
            Debug.LogError("Critical Error in HandSummon.cs. Cannot find XR Rig named 'XR Rig', did you rename it? If so, drag it into the XRRig field in HandSummon via the inspector.");
        }
        // set up the hands
        if (!l_hand)
        {
            l_hand = GameObject.Find("LeftHand Controller").GetComponent<XRBaseControllerInteractor>() ?? null;
            xrControllers.Add(l_hand.GetComponent<XRController>());
        }
        if (!r_hand)
        {
            r_hand = GameObject.Find("RightHand Controller").GetComponent<XRBaseControllerInteractor>() ?? null;
            xrControllers.Add(r_hand.GetComponent<XRController>());
        }
        if (!l_hand || !r_hand)
        {
            Debug.LogError("Error in HandSummon.cs: Unable to find hands. Either define them manually by dragging your controllers to the l_hand and r_hand fields in the inspector, or name them LeftHand Controller and RightHand Controller so I can find them automagically.");
        }
        if (!tutorial)
        {
            tutorial = GameObject.Find("TutorialHands") ?? null;
        }
        if (!tutorial) 
        {
            Debug.LogError("Unable to find tutorial hands game object. Tutorial hands will not show.");
        }
        if (!tutorialBar)
        {
            tutorialBar = GameObject.Find("TutorialBar") ?? null;
        }
        if (!tutorialBar)
        {
            Debug.LogError("Unable to find tutorial bar game object. Tutorial tutorial bar will not show.");
        }
        tutorial.SetActive(false);
        tutorialBar.SetActive(false);

        // ensure materials are defined
        if (!errorMaterial)
        {
            Debug.LogError("HandSummon.cs | Missing material for errorMaterial! This is bad. You should set this in the SummonManager to a material");
        }
        if (!okMaterial)
        {
            Debug.LogError("HandSummon.cs | Missing material for okMaterial! This is bad. You should set this in the SummonManager to a material");
        }

        // make sure there are endCapColliders
        if (endCapColliders.Count < 2)
        {
            //Debug.Log("No endcap colliders defined, so looking for and adding the expected ones by name 'EndCapLeft' and 'EndCapRight'");
            endCapColliders.Add(GameObject.Find("EndCapLeft").GetComponent<BoxCollider>());
            endCapColliders.Add(GameObject.Find("EndCapRight").GetComponent<BoxCollider>());
        }
        if (endCapColliders.Count != 2)
        {
            Debug.LogError("Error in HandSummon. No endcaps defined. Summoning won't work. Either you have to specify these manually in the inspector, or name them EndCapLeft and EndCapRight");
        }
        foreach (BoxCollider collider in endCapColliders)
        {
            //Debug.Log("Binding colliders for endcaps: " + collider.gameObject.name);
            HandSummonEndCaps endcap = collider.GetComponent<HandSummonEndCaps>();
            // a nice little Lambda so we can bind events in this script instead of having to split this script up even more.
            endcap.EndcapCollisionStart += (GameObject other) => {
                //Debug.Log("EndcapCollisionStart triggered by: " + other.name);
                switch (other?.transform?.parent?.gameObject.name)
                {
                    case "LeftHand Controller":
                        this.leftHandDone = true;
                        other.transform.parent.GetComponent<XRBaseControllerInteractor>().SendHapticImpulse(hapticIntensity, 0.05f);
                        break;
                    case "RightHand Controller":
                        this.rightHandDone = true;
                        other.transform.parent.GetComponent<XRBaseControllerInteractor>().SendHapticImpulse(hapticIntensity, 0.05f);
                        break;
                    default:
                        Debug.LogWarning("Warning in HandSummon.cs:EndcapCollisionStart triggered by gameobject trigger thats not LeftHand Controller or RightHand Controller. If summoning doesn't work, this could be a clue. If everything works fine, don't worry about it.");
                        break;
                }
                
            };
            endcap.EndcapCollisionEnd += (GameObject other) => {
                //Debug.Log("EndcapCollisionEnd triggered by: " + other.name);
                switch (other?.transform?.parent?.gameObject.name)
                {
                    case "LeftHand Controller":
                        leftHandDone = false;
                        break;
                    case "RightHand Controller":
                        rightHandDone = false;
                        break;
                    default:
                        Debug.LogWarning("Warning in HandSummon.cs:EndcapCollisionEnd triggered by gameobject trigger thats not LeftHand Controller or RightHand Controller. If summoning doesn't work, this could be a clue. If everything works fine, don't worry about it.");
                        break;
                }
            };
        }
    }

    public void VibrateBothControllers(float intensity, float duration)
    {
        foreach (XRController device in xrControllers)
        {
            device.SendHapticImpulse(intensity, duration);
        }
    }

    public void Update()
    {
         

        if (rightHandBegin && leftHandBegin)
        {
            if (!step1)
            {
                Debug.Log("Step1 begun! ...");
                SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
                soundManager.QueSound(SFX.Sounds.Correct);
                // show them the thingy so they have some kind of visible cue
                ShowTutorial();
                step1 = true;   // first step is to get both hands in
            }
            if (HandsInsideBox()) {

                if (Debounce(0.25f))
                {
                    VibrateBothControllers(0.5f, 0.25f);
                }
            } else
            {
                ResetSummon();
                FlashRedThenHide();
            }

            // if their hands make it to the endCaps, then they've done it, and they should get an object.
            if (rightHandDone && leftHandDone)
            {
                // and we haven't celebrated yet
                if (!step2) { 
                    //Debug.Log("Summoning Item!");
                    SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
                    soundManager.QueSound(SFX.Sounds.Correct);
                    step1 = false;
                    step2 = true;
                    WarpInNewObject("mic1");
                }
            }

        }

        // step2 is when we actually summon the object and maybe let them size it and stuff. 
        // It's basically it's own thing, and we no longer care where their hands are.
        if (step2)
        {
            // TODO: allow sizing or selecting or something
        }

        HandleTweeningIfNeeded();
    }

    public void HandleTweeningIfNeeded()
    {
        // this gets started by the FlashRedThenHide() function. One day there might be more than one tween here, so we'll want to break those out.
        if (errorIndicationHappening)
        {
            var progress = Time.time - lerpStart;
            var renderer = tutorialBar.GetComponent<MeshRenderer>();
            float newAlpha = Mathf.Lerp(lerpDuration, 0.0f, progress / lerpDuration);
            errorMaterial.color = new Color(errorMaterial.color.r, errorMaterial.color.g, errorMaterial.color.b, newAlpha);
            renderer.material = errorMaterial; 
            //Debug.Log("FlashRedThenHide running... " + newAlpha + ".");

            if (lerpDuration < progress)
            {
                //Debug.Log("FlashRedThenHide Done! ");
                errorIndicationHappening = false;
                HideTutorial();
            }
        }

        // for warping in new objects
        if (instantiateIndicationHappening)
        {
            // do the stuff like above, if we watn to.
        }
    }

    public void WarpInNewObject(string objectName)
    {
        Debug.Log("Warping in new one!");
        Object prefab = Resources.Load("Mic1", typeof(GameObject));
        var rotationForHands = Quaternion.Euler(90f, l_hand.transform.rotation.eulerAngles.y, 70f);
        var positionForHands = r_hand.transform.position;
        GameObject newMic = Instantiate(prefab, positionForHands, rotationForHands) as GameObject;
        instantiateIndicationHappening = true;
        //newMic.SetActive(true);
        ResetSummon(true);
        HideTutorial();
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.QueSound(SFX.Sounds.Correct);
    }

    public void ResetSummon(bool silent = false)
    {
        // they moved their hands too far outside of the allowed bounds while doing the gesture, so reset everything. TODO: make this a function.
        Debug.Log("ResetSummon");
        if (!silent)
        {
            SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
            soundManager.QueSound(SFX.Sounds.DropMic);
        }
        rightHandBegin = false;
        leftHandBegin = false;
        rightHandDone = false;
        leftHandDone = false;
        step1 = false;
        step2 = false;
    }

    public void FlashRedThenHide()
    {
        lerpStart = Time.time;
        errorIndicationHappening = true;
    }

    public void FancyWarpIn()
    {
        lerpStart = Time.time;
        instantiateIndicationHappening = true;
    }

    public void ShowTutorial()
    {
        tutorial.SetActive(true);
        tutorialBar.SetActive(true);
        //tutorialBar.GetComponent<MeshRenderer>().enabled = true;
        tutorialBar.GetComponent<MeshRenderer>().material = okMaterial;
    }

    public void HideTutorial()
    {
        tutorialBar.GetComponent<MeshRenderer>().material = okMaterial; // in case it's in its error state
        //tutorialBar.GetComponent<MeshRenderer>().enabled = false;
        tutorial.SetActive(false);
        tutorialBar.SetActive(false);
    }

    // kind of a utility class. Should probably put this somewhere with utils
    public bool Debounce(float debounceTime)
    {

        // if the current time is greater than the last time we played a sound + the defined minimum sound interval, then...
        if (Time.time > debounceLastTime + debounceTime)
        {
            debounceLastTime = Time.time;
            return true;
        }
        return false;
    }

    public void OnTriggerEnter(Collider other)
    {
        GameObject hitByParent = other.GetComponent<Transform>().parent.gameObject;  // :-O
        //Debug.Log("HandSummon::onTriggerEnter:" + hitByParent.name);
        if (hitByParent.name == l_hand.gameObject.name)
        {
            // left hand triggered by entering the summon area
            if (HoldingBothGrips()) { 
                leftHandBegin = true;
            }
        }
        if (hitByParent.name == r_hand.gameObject.name)
        {
            // right hand triggered by entering the summon area
            if (HoldingBothGrips())
            {
                rightHandBegin = true;
            }
            // TODO: Check rotation of hands as well. Left hand would be Vector3(339.273499, 277.711548, 98.3878937)
        }
    }
 

    public bool HandsInsideBox()
    {
        var handsInBox = 0;
        Renderer rend = GetComponent<Renderer>();
        if (rend.bounds.Contains(l_hand.transform.position))
        {
            //Debug.Log("HandsInsideBox | Lefthand is in the box");
            handsInBox++;
        }
        if (rend.bounds.Contains(r_hand.transform.position))
        {
            //Debug.Log("HandsInsideBox | Right hand is in the box");
            handsInBox++;
        }

        return handsInBox > 0;
    }

    public bool HoldingBothGrips()
    {
        var gripsBeingHeld = 0;
        foreach (XRController controller in xrControllers)
        {
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float grip))
            {
                if (grip > 0.1)
                {
                    gripsBeingHeld++;
                }
            }
        }
        
        return gripsBeingHeld == 2;
    }
     

}
