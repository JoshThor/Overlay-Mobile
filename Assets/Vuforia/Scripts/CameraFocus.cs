using UnityEngine;
using System.Collections;

using Vuforia;
using System.Collections.Generic;
using System;
using System.IO;

public class CameraFocus : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        VuforiaBehaviour.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaBehaviour.Instance.RegisterOnPauseCallback(OnPaused);    
	}

    private void OnPaused(bool paused)
    {
        if(!paused)
        {
            bool focusModeSet = CameraDevice.Instance.SetFocusMode(
            CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

            if (!focusModeSet)
            {
                CameraDevice.Instance.SetFocusMode(
                    CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
                Debug.Log("Failed to set camera mode to Continuous Auto focus");
            }
        }
    }

    private void OnVuforiaStarted()
    {
        bool focusModeSet = CameraDevice.Instance.SetFocusMode(
            CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

        if(!focusModeSet)
        {
            CameraDevice.Instance.SetFocusMode(
                CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
            Debug.Log("Failed to set camera mode to Continuous Auto focus");
        }
    }
}
