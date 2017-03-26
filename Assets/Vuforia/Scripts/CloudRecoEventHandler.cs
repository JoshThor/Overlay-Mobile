
using UnityEngine;
using System.Collections;
using Vuforia;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.UI;

public class CloudRecoEventHandler : MonoBehaviour, ICloudRecoEventHandler
{

    private CloudRecoBehaviour mCloudRecoBehaviour;


    public ImageTargetBehaviour ImageTargetTemplate;

    public GameObject emptyPrefabWithMeshRenderer;
    
    public Material renderMaterial;

    private GameObject ARO;

    public GameObject OBJLoader;

    public GameObject restartButton;
    public GameObject scanButton;

    private bool pinchToZoom;
    private bool targetFound = false;
    private bool tapToScan = false;
    private bool mIsScanning = false;
    private bool menuActive = false;



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

    // Use this for initialization
    void Start()
    {
        // register this event handler at the cloud reco behaviour
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();

        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterEventHandler(this);
        }

        restartButton.SetActive(false);

        SetPinchToZoom(true);

        SetTapToScan(false);

        OBJLoader = GameObject.Find("OBJLoader");
    }

    void Update()
    {
        if (!mIsScanning && !menuActive)
        {
            if(tapToScan && !targetFound)
            {
                restartButton.SetActive(false);
                scanButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                scanButton.GetComponent<Button>().interactable = false;
                restartButton.SetActive(true);
            }
        }
        else
        {

            restartButton.SetActive(false);
  
        }
    }

    public void OnInitialized()
    {
        Debug.Log("Cloud Reco initialized");
    }

    public void OnInitError(TargetFinder.InitState initError)
    {
        //Debug.Log("Cloud Reco initialization Error: "+initError.ToString());
    }

    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        //Debug.Log("Cloud Reco update Error: " + updateError.ToString());
    }

    public void SetTapToScan(bool active)
    {
        tapToScan = active;

        scanButton.SetActive(active);

        if(active)
        {
            Debug.Log("Scanning disabled...");
            mCloudRecoBehaviour.CloudRecoEnabled = false;
        }
        else
        {
            Debug.Log("Scanning Enabled...");
            mCloudRecoBehaviour.CloudRecoEnabled = true;
        }

        //if true set tap to scan button to active
    }

    public void SetPinchToZoom(bool active)
    {
        pinchToZoom = active;
        //Debug.Log("Toggled: "+active);
    }

    public void OnMenuButton()
    {

        if(!menuActive)
        {
            menuActive = true;

            mCloudRecoBehaviour.CloudRecoEnabled = false;

            TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();

        }
        else
        {
            menuActive = false;

            //If tap to scan is turned on we dont want it to activate the scanning again
            if(!tapToScan)
                mCloudRecoBehaviour.CloudRecoEnabled = true;

            TrackerManager.Instance.GetTracker<ObjectTracker>().Start();

        }

    }

    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;

        if (scanning && targetFound && tapToScan)
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

                Destroy(OBJLoader.GetComponent<OBJ>());
                OBJLoader.AddComponent<OBJ>();

                //re-enable tap to scan
                targetFound = false;

                OnStateChanged(false);
            }
        }
        else if (!tapToScan && scanning)
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
                DestroyImmediate(OBJLoader.GetComponent<OBJ>(), true);
                OBJLoader.AddComponent<OBJ>();


                //re-enable tap to scan
            }

        }
    }

    // Here we handle a target reco event
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {

        //Target found
        targetFound = true;

        // stop the target finder
        mCloudRecoBehaviour.CloudRecoEnabled = false;

        //disable tap to scan button

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

           // Vector3 loadedPosition = g.GetComponent<Renderer>().bounds.center;
            // Debug.Log(loadedPosition);
            //Debug.Log(g.transform.localPosition);
            g.transform.position = Vector3.zero;
            g.AddComponent<SetMeshBounds>();
            //g.transform.localPosition = Vector3.zero;
            g.transform.localScale = Vector3.one / 4;
            g.transform.localRotation = Quaternion.identity;
        }
        if (spawnedObjects.Length == 1)
        {
            if (pinchToZoom)
            { 
                spawnedObjects[0].AddComponent<PinchZoom>();
            }

            ARO = spawnedObjects[0];
            ARO.transform.localPosition = Vector3.zero;
            
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

    public void OnRestartButton()
    {
        // Restart TargetFinder
        if (!tapToScan)
        {
            mCloudRecoBehaviour.CloudRecoEnabled = true;
        }
        else
        {
            OnStateChanged(true);
        }
    }


    public void OnTapToScan()
    {
        if (!mIsScanning && tapToScan)
        {   
            StartCoroutine("ScanImage");
        }
        else
        {
            Debug.Log("Already scanning or tap to scan not enabled");
        }
    }

    IEnumerator ScanImage()
    {
        mCloudRecoBehaviour.CloudRecoEnabled = true;
        //Disable button
        scanButton.GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(5);

        mCloudRecoBehaviour.CloudRecoEnabled = false;
        //re-enable button
        scanButton.GetComponent<Button>().interactable = true;
    }
}