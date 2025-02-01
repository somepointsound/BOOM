using System;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEngine.XR.Interaction.Toolkit {
    /// <summary>
    /// The NSTOLAvatarMovement is a locomotion provider that allows the user to move their rig using a specified 2d axis input.
    /// the provider can take input from two different devices (eg: L & R hands). It is a ripoff of the SnapTurnProvider.cs XRToolkit provider.
    /// </summary>
    public class NSTOL_AvatarMovement : LocomotionProvider
    {

        [Tooltip("The axis on the controller you're gonna use for moving around. Look at the ProjectSettings panel's Input for this")]
        public Input inputDevice;
        public Transform avatar;
        public float rotationSpeed = 100.0f;
        [Tooltip("Valid options are left and right. Case sensitive.")]
        public string hand;
        private Transform forwardDirection;

        /// <summary>
        /// This is the list of possible valid "InputAxes" that we allow users to read from.
        /// </summary>
        public enum InputAxes
        {
            Primary2DAxis = 0,
            Secondary2DAxis = 1,
        };

        // Mapping of the above InputAxes to actual common usage values
        static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[] {
            CommonUsages.primary2DAxis,
            CommonUsages.secondary2DAxis,
        };

        [SerializeField]
        [Tooltip("The 2D Input Axis on the primary devices that will be used to trigger smooth movement.")]
        InputAxes m_MoveUsage = InputAxes.Primary2DAxis;
        /// <summary>
        /// The 2D Input Axis on the primary device that will be used to trigger a snap turn.
        /// </summary>
        public InputAxes turnUsage { get { return m_MoveUsage; } set { m_MoveUsage = value; } }

        [SerializeField]
        [Tooltip("A list of controllers that allow smooth movement.  If an XRController is not enabled, or does not have input actions enabled smooth movement will not work.")]
        List<XRController> m_Controllers = new List<XRController>();
        /// <summary>
        /// The XRControllers that allow smooth movement.  An XRController must be enabled in order to smooth move.
        /// </summary>
        public List<XRController> controllers { get { return m_Controllers; } set { m_Controllers = value; } }

        [SerializeField]
        [Tooltip("The speed at which movement will happen.")]
        float m_Speed = 10.0f;
        /// <summary>
        /// The speed at which movement will happen.
        /// </summary>
        public float moveSpeed { get { return m_Speed; } set { m_Speed = value; } }

        [SerializeField]
        [Tooltip("The amount of time that the system will wait before starting another snap turn.")]
        float m_DebounceTime = 0.25f;
        /// <summary>
        /// The amount of time that the system will wait before starting another snap turn.
        /// </summary>
        public float debounceTime { get { return m_DebounceTime; } set { m_DebounceTime = value; } }

        [SerializeField]
        [Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
        float m_DeadZone = 0.5f;
        /// <summary>
        /// The deadzone that the controller movement will have to be above to trigger a snap turn.
        /// </summary>
        public float deadZone { get { return m_DeadZone; } set { m_DeadZone = value; } }

        void EnsureControllerDataListSize()
        {
            if (m_Controllers.Count != m_ControllersWereActive.Count)
            {
                while (m_ControllersWereActive.Count < m_Controllers.Count)
                {
                    m_ControllersWereActive.Add(false);
                }

                while (m_ControllersWereActive.Count < m_Controllers.Count)
                {
                    m_ControllersWereActive.RemoveAt(m_ControllersWereActive.Count - 1);
                }
            }
        }

        // state data
        Vector3 m_CurrentMovementAmount = new Vector3();
        float m_TimeStarted = 0.0f;

        List<bool> m_ControllersWereActive = new List<bool>();

        private void Update()
        {
            // wait for a certain amount of time before allowing another turn.
            if (m_TimeStarted > 0.0f && (m_TimeStarted + m_DebounceTime < Time.time))
            {
                m_TimeStarted = 0.0f;
                return;
            }

            if (m_Controllers.Count > 0)
            {
                EnsureControllerDataListSize();

                InputFeatureUsage<Vector2> feature = m_Vec2UsageList[(int)m_MoveUsage];
                for (int i = 0; i < m_Controllers.Count; i++)
                {
                    XRController controller = m_Controllers[i];
                    if (controller != null)
                    {
                        if (controller.enableInputActions && m_ControllersWereActive[i])
                        {
                            InputDevice device = controller.inputDevice;

                            Vector2 currentState;
                            if (device.TryGetFeatureValue(feature, out currentState))
                            {
                                //if (currentState.y > deadZone)
                                //{
                                    StartMove(currentState);
                                //}
                                //else if (currentState.y < -deadZone)
                                //{
                                //    StartMove(-currentState.y);
                                //}
                            }
                        }
                        else //This adds a 1 frame delay when enabling input actions, so that the frame it's enabled doesn't trigger a snap turn.
                        {
                            m_ControllersWereActive[i] = controller.enableInputActions;
                        }
                    }
                }
            }

            if (Math.Abs(m_CurrentMovementAmount.x) > 0.0f || Math.Abs(m_CurrentMovementAmount.y) > 0.0f)
            {

                if (BeginLocomotion())
                {
                    // the below code works for smooth movement, but doesn't seem to update the avatar position properly.
                    var xrRig = system.xrRig;
                    var camera = xrRig.transform.Find("Camera Offset").transform.Find("Main Camera");
                    Quaternion headRotationFlat = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0);
                    var move = (headRotationFlat * (new Vector3(m_CurrentMovementAmount.x, 0f, m_CurrentMovementAmount.y)) * m_Speed * Time.deltaTime);
                    xrRig.transform.Translate(move, Space.World);

                    m_CurrentMovementAmount = new Vector2();
                    EndLocomotion();
                }
            }

        }


        private void StartMove(Vector2 currentState)
        {
            //if (m_TimeStarted != 0.0f)
            //    return;

            if (!CanBeginLocomotion())
                return;

            m_TimeStarted = Time.time;
            m_CurrentMovementAmount = currentState;
        }        
     
    }
    }