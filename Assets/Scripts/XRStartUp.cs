using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class XRStartUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartXR();

    }
    private void OnDisable()
    {
        StopXR();
    }
    public static void StartXR()
    {
#if UNITY_EDITOR
        //Invalidate XR if previous instance failed to shutdown XR properly
        StopXR();
#endif

        if (XRGeneralSettings.Instance.Manager.isInitializationComplete) return;

        XRGeneralSettings.Instance.Manager.InitializeLoaderSync();

        if (!XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            Debug.LogError("Failed to initialize XR!");
            return;
        }

        XRGeneralSettings.Instance.Manager.StartSubsystems();

        Debug.Log("Starting XR");
    }

    public static void StopXR()
    {
        if (!XRGeneralSettings.Instance.Manager.isInitializationComplete) return;

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();

        Debug.Log("Stopping XR");
    }
}