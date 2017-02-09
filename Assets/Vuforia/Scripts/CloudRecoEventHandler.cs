﻿/*==============================================================================
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

    public GameObject restartButton;

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

    public GameObject OBJLoader;

    // Use this for initialization
    void Start()
    {
        restartButton.SetActive(false);

        // register this event handler at the cloud reco behaviour
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterEventHandler(this);
        }
        OBJLoader = GameObject.Find("OBJLoader");
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

                //OBJLoader.GetComponent<OBJ>().SetLoaded(false);
                Destroy(OBJLoader.GetComponent<OBJ>());
                OBJLoader.AddComponent<OBJ>();

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


        StartCoroutine(OBJLoader.GetComponent<OBJ>().Load(modelURL));
        

        while(!OBJLoader.GetComponent<OBJ>().IsLoaded())
            yield return new WaitForSeconds(0.1f);

        GameObject[] spawnedObjects;
        spawnedObjects = GameObject.FindGameObjectsWithTag("ARO");

        if(spawnedObjects.Length == 0)
        {
            Debug.Log("No game object found");
        }
        

        foreach (GameObject g in spawnedObjects)
        {
            g.transform.parent = ImageTargetTemplate.transform;
            g.transform.localPosition = Vector3.zero;
            g.transform.localScale = Vector3.one / 2;
            g.transform.localRotation = Quaternion.identity;
        }
        if(spawnedObjects.Length == 1)
        {
            ARO = spawnedObjects[0];
        }
    }
/*
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
*/
    public void OnActive(Boolean paused)
    {
        if(paused)
        {
            VuforiaBehaviour.Instance.enabled = false;
        }
        else
        {
            VuforiaBehaviour.Instance.enabled = true;
        }
    }

    void OnGUI()
    {
        // If not scanning, show button 
        // so that user can restart scanning
        if (!mIsScanning)
        {
            restartButton.SetActive(true);
            /*
            if (GUI.Button(new Rect(125, 275, 50, 50), ">"))
            {
                ARO.transform.Translate(Vector3.right * 2);
            }
            if (GUI.Button(new Rect(75, 275, 50, 50), "<"))
            {
                ARO.transform.Translate(Vector3.left * 2);
            }
            */
            if (GUI.Button(new Rect(100, 325, 50, 50), "-"))
            {
                ARO.transform.localScale /= 2;
            }
            if (GUI.Button(new Rect(100, 375, 50, 50), "+"))
            {
                ARO.transform.localScale *= 2;
            }
            
        }
        else
        {
            restartButton.SetActive(false);
        }
    }

    public void OnRestartButton()
    {
        // Restart TargetFinder
        mCloudRecoBehaviour.CloudRecoEnabled = true;
    }
}