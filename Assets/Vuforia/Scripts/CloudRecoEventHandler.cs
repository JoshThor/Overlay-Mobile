/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
==============================================================================*/

using UnityEngine;
using System.Collections;
using Vuforia;
using System.Collections.Generic;
using System;
using System.IO;

public class CloudRecoEventHandler : MonoBehaviour, ICloudRecoEventHandler
{

    private CloudRecoBehaviour mCloudRecoBehaviour;
    private bool mIsScanning = false;

    //Bundle URL;
    private string modelURL = "http://people.sc.fsu.edu/~jburkardt/data/obj/cube.obj";

    /* Models that I know work:
    Cube: http://people.sc.fsu.edu/~jburkardt/data/obj/cube.obj
    Decahedron: http://people.sc.fsu.edu/~jburkardt/data/obj/dodecahedron.obj
    Pyramid: http://people.sc.fsu.edu/~jburkardt/data/obj/pyramid.obj //Note: you have to modify ObjImporter line 45 to: newVerts[i] = newMesh.vertices[(int)v.x - 1 -1];
    Tetrahedron: http://people.sc.fsu.edu/~jburkardt/data/obj/tetrahedron.obj
    Humanoid *Works but is  a bit large*: http://people.sc.fsu.edu/~jburkardt/data/obj/humanoid_quad.obj
    Gourd: http://people.sc.fsu.edu/~jburkardt/data/obj/gourd.obj
    Shuttle: http://people.sc.fsu.edu/~jburkardt/data/obj/shuttle.obj
     */

    public ImageTargetBehaviour ImageTargetTemplate;

    public GameObject emptyPrefabWithMeshRenderer;

    public Material renderMaterial;

    private GameObject ARO;

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

        // Remove the augmentation
        if (ImageTargetTemplate)
        {
            //Destroys any active 3D objects
            if (ImageTargetTemplate.transform.childCount > 0)
                Destroy(ImageTargetTemplate.transform.GetChild(0).gameObject);
        }
    }
    }

    // Here we handle a target reco event
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        // stop the target finder
        mCloudRecoBehaviour.CloudRecoEnabled = false;

        if(targetSearchResult.MetaData == null)
        {
            return;
        }

        //Fetch the Bundle URL from the metadata
        modelURL = targetSearchResult.MetaData;

        // Build augmentation based on target
        if (ImageTargetTemplate)
        {
            // enable the new result with the same ImageTargetBehaviour:
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            ImageTargetBehaviour imageTargetBehaviour = tracker.TargetFinder.EnableTracking(targetSearchResult, ImageTargetTemplate.gameObject) as ImageTargetBehaviour;
            Debug.Log("Image Recognized");

            StartCoroutine("DownloadObject");
        }
    }

    IEnumerator DownloadObject()
    {
        //Destroys any active 3D objects
        if (ImageTargetTemplate.transform.childCount > 0)
            Destroy(ImageTargetTemplate.transform.GetChild(0).gameObject);

        // Load the 3D Object file from the specified URL
        using (WWW www = new WWW(modelURL))
        {
            yield return www;
            if (www.error != null)
                throw new UnityException("WWW download had an error:" + www.error);

            string fileText = www.text;


            GameObject spawnedPrefab;
            Mesh importedMesh = ObjImporter.ImportFile(fileText, "ARO");
            spawnedPrefab = Instantiate(emptyPrefabWithMeshRenderer, transform.position, transform.rotation) as GameObject;
            spawnedPrefab.GetComponent<MeshFilter>().mesh = importedMesh;
            spawnedPrefab.GetComponent<Renderer>().material = renderMaterial;
            spawnedPrefab.transform.parent = ImageTargetTemplate.transform;
            spawnedPrefab.transform.localPosition = Vector3.zero;
            spawnedPrefab.transform.localScale = Vector3.one / 2;
            spawnedPrefab.transform.localRotation = Quaternion.identity;

            ARO = spawnedPrefab;

            Debug.Log("Model Loaded");
        }
    }

    void OnGUI()
    {
        // If not scanning, show button 
        // so that user can restart scanning
        if (!mIsScanning)
        {

            if (GUI.Button(new Rect(125, 275, 50, 50), ">"))
            {
                ARO.transform.Translate(Vector3.right * 2);
            }
            if (GUI.Button(new Rect(75, 275, 50, 50), "<"))
            {
                ARO.transform.Translate(Vector3.left * 2);
            }
            if (GUI.Button(new Rect(100, 225, 50, 50), "-"))
            {
                ARO.transform.localScale /= 2;
            }
            if (GUI.Button(new Rect(100, 175, 50, 50), "+"))
            {
                ARO.transform.localScale *= 2;
            }
            if (GUI.Button(new Rect(100, 100, 200, 50), "Restart Scanning"))
            {
                // Restart TargetFinder
                mCloudRecoBehaviour.CloudRecoEnabled = true;
            }
        }
    }
}