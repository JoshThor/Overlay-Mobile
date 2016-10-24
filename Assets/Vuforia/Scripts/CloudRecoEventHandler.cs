/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEngine;
using System.Collections;
using Vuforia;

public class CloudRecoEventHandler : MonoBehaviour, ICloudRecoEventHandler
{

    private CloudRecoBehaviour mCloudRecoBehaviour;
    private bool mIsScanning = false;

    public ImageTargetBehaviour ImageTargetTemplate;

    // Use this for initialization
    void Start()
    {
        // register this event handler at the cloud reco behaviour
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterEventHandler(this);
        }
    }

    public void OnInitialized()
    {
        Debug.Log("Cloud Reco initialized");
    }

    public void OnInitError(TargetFinder.InitState initError)
    {
        // handle error
    }

    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        // handle error
    }

    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;

        if (scanning)
        {
            // clear all known trackables
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.TargetFinder.ClearTrackables(false);

            // Remove the augmentation (cube object)
            if (ImageTargetTemplate)
            {
                Transform cube = ImageTargetTemplate.transform.FindChild("MyAugmentation");
                if (cube)
                {
                    GameObject.Destroy(cube.gameObject);
                }
            }
        }
    }

    // Here we handle a target reco event
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        // stop the target finder
        mCloudRecoBehaviour.CloudRecoEnabled = false;

        // Build augmentation based on target
        if (ImageTargetTemplate)
        {
            // enable the new result with the same ImageTargetBehaviour:
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            ImageTargetBehaviour imageTargetBehaviour = tracker.TargetFinder.EnableTracking(targetSearchResult, ImageTargetTemplate.gameObject) as ImageTargetBehaviour;

            // Create a simple cube
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "MyAugmentation";

            // Attach cube as child of the cloud image target
            cube.transform.parent = imageTargetBehaviour.gameObject.transform;

            // Setup local transformation
            cube.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            cube.transform.localRotation = Quaternion.identity;

            // Enable cube
            cube.SetActive(true);
        }
    }

    void OnGUI()
    {
        // If not scanning, show button 
        // so that user can restart scanning
        if (!mIsScanning)
        {
            if (GUI.Button(new Rect(100, 100, 200, 50), "Restart Scanning"))
            {
                // Restart TargetFinder
                mCloudRecoBehaviour.CloudRecoEnabled = true;
            }
        }
    }
}