using System;
using UnityEngine;
using Unity.XR.Oculus;
using UnityEngine.UI;
//using Normal.Realtime;

#if LIH_PRESENT
using UnityEngine.Experimental.XR.Interaction;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// XRController MonoBehaviour that handles hand movement/animation
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("NSTOL Hand Animator (April)")]
    public class NSTOL_HandAnimate : MonoBehaviour
    {

        string debugOutput = ""; // debug output, although mostly we're doign this in the NSTOLDebug GameObjects' debug script, DebugHelpers.

        [Tooltip("This should be the left hand controller of your rig or left hand prefab. We use get input from this game object. Auto-set to this GameObject's XRController, if not set manually. This should work if you have your object hierarchy like mine.")]
        public XRController controller;

        private Animator anim;  // the animator that we're triggering changes on. It's auto-populated in Start().

        private XRRayInteractor interactor = null;

        // the interactor has this listener support thing used below. With interactor.onSelectEnter and interactor.onSelectExit. We want to use those. When using those, we dont want the animation stuff we've written here to override it. Hence this stateful bool.
        private bool takeAnimationControl = true;

        void Start()
        {
            if (!controller)
            {
                controller = GetComponent<XRController>();
            }

            // animators... ASSEMBLE!
            anim = GetComponentInChildren<Animator>();
            interactor = controller.GetComponent<XRRayInteractor>();
            interactor.onSelectEntered.AddListener(handIsHoldingBall);
            interactor.onSelectExited.AddListener(handDropBall);



        }

        private void handIsHoldingBall(XRBaseInteractable arg0)
        {
            //DebugHelpers.Log("handisHoldingBall called");
            /*
            if (arg0)
            { 
                if (arg0.GetComponent<RealtimeTransform>())
                {
                    arg0.GetComponent<RealtimeTransform>().RequestOwnership();
                }
            }
            */
            takeAnimationControl = false;
            anim.SetBool("holdBall", true);
        }

        private void handDropBall(XRBaseInteractable arg0)
        {
            //DebugHelpers.Log("handDropBall called");
            anim.SetBool("holdBall", false);
            takeAnimationControl = true;
            //arg0.GetComponent<RealtimeTransform>().ClearOwnership();  <-- not necessary because it's done automatically by Normal.
        }

        void Update()
        {
            if (takeAnimationControl)
                UpdateControllerStatus();
        }

        private void UpdateControllerStatus()
        {
            debugOutput = "";
            NSTOLWholeHandPosition handReducer = new NSTOLWholeHandPosition();

            // A Button Capacitive touch
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouch))
            {
                if (primaryTouch)
                {
                    handReducer.thumb = 3;
                    debugOutput += "Primary A Button Capacitive touch: " + primaryTouch + "\n";
                }
            }

            // A Button press
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primary))
            {
                if (primary)
                {
                    handReducer.thumb = 4;
                    debugOutput += "A Pressed: " + primary + "\n";
                }
            }


            // Grip press
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float grip))
            {
                grip = Mathf.Round(grip * 100f) / 100f;
                if (grip > 0.1)
                {
                    handReducer.finger2 = grip;
                    debugOutput += "Grip Pressed: " + grip + "\n";
                } else
                {
                    handReducer.finger2 = 0;
                }
            }

            // Joystick Capacitive touch
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool touch))
            {
                if (touch)
                {
                    handReducer.thumb = 1;
                    debugOutput += "Primary thumbstick capacitive touch: " + touch + "\n";
                }
            }

            // Joystick click 
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool primary2DAxisClick))
            {
                if (primary2DAxisClick)
                {
                    handReducer.thumb = 2;
                    debugOutput += "Thumbrest click: " + primary2DAxisClick + "\n";
                }
            }

            // Index (Pointer) Capacitive touch
            if (controller.inputDevice.TryGetFeatureValue(OculusUsages.indexTouch, out bool indexTouch))  
            {
                if (indexTouch)
                {
                    handReducer.finger1 = 1;
                    debugOutput += "Index (Pointer) Capacitive touch: " + indexTouch + "\n";
                } else
                {
                    handReducer.finger1 = 0;
                }
            }

            // Index (Pointer/Trigger) depressed 
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float trigger))
            {
                trigger = Mathf.Round(trigger * 100f) / 100f;
                if (trigger > 0.5)  // TODO: Change hard-coded trigger threshold to a global config value
                {
                    handReducer.finger1 = trigger;
                    debugOutput += "Index (Pointer/Trigger) depressed: " + trigger + "\n";
                }
                else
                {
                    handReducer.finger1 = 0;
                }
            }

            String handSign = handReducer.Reduce();



            if (debugOutput.Length > 0)
            {
                // used internally so we can reuse this object.
                // NSTOLLog.debugTextObject = debugTextObject;
                //DebugHelpers.Log(handSign + "\nThumbAct: " + handReducer.thumb + "\nGrip: " + grip + "\nTrigger: " + trigger);

#if UNITY_EDITOR
                // Debug.Log(controller.tag.ToString() + debugOutput);
#endif
            }

            // Update the animator to reflect the new hand pose.
            anim.SetInteger("ThumbAct", handReducer.thumb);
            anim.SetFloat("Grip", grip);
            anim.SetFloat("TriggerAct", trigger);

        }

    }


    /// <summary>
    /// internally used class for reducing a bunch of input values into a hand position
    /// </summary>

    public class NSTOLWholeHandPosition 
    {
        //fingers - they call 'em fingers, but you never see 'em fing.
        public float finger1;
        public float finger2;
        public int finger3;
        public int finger4;
        public int thumb;

        // TODO: Change to Enum.
        /* 
         * 
        <summary>
        Output: A string (TODO CHANGE TO ENUM) that is one of: open, relaxed, fist, fingerguns, thumbsup.
        fingers are mapped as:
         * 0: off
         * 1: touch
         * 2: on
            
         Thumb positions are: 
            0: off
            1: joystick touch
            2: joystick depressed
            3: a button touch
            4: a button depressed
            5: b button touch
            6: b button depressed 
        </summary>
        */

        public String Reduce() 
        {

            //open 
            if (thumb.Equals(2) && finger1.Equals(2) && finger2 <= 0.5)
            {
                return "open";
            }

            //relaxed
            if (thumb.Equals(1) && finger1.Equals(1) && finger2 < 0.5) // finger2 is allowed to be either touch or open, because we can't tell the difference right now, but it SHOULD be just on touch.
            {
                return "relaxed";
            }

            //FIST 
            if (thumb.Equals(2) && finger1.Equals(2) && finger2 >= 0.5)  // maybe allow thumb to be at state 4 or 6 as well.
            {
                return "fist";
            }

            //fingerguns!
            if (thumb.Equals(0) && finger1.Equals(0) && finger2 >= 0.5)
            {
                return "fingerguns";
            }

            // thumbs up
            if (thumb.Equals(0) && finger1.Equals(2) && finger2 >= 0.5)
            {
                return "thumbsup";
            }

            return "open";
        }

        public void Start()
        {
            thumb = 0;
            finger1 = 0;
            finger2 = 0;
            finger3 = 0;
            finger4 = 0;
        }
    }
}