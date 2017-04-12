using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchObject : MonoBehaviour {

    string URL = "";

    float touchDuration;
    Touch touch;

    // Update is called once per frame
    void Update () {
        /*
        int i = 0;
        while (i < Input.touchCount)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                Debug.Log("TAP !");
                ShootRay(ray);
            }
            ++i;
        }
        */
#if (UNITY_EDITOR)
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("TAP !");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            ShootRay(ray);
        }
#endif

        if (Input.touchCount > 0)
        { //if there is any touch
            touchDuration += Time.deltaTime;
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended && touchDuration < 0.2f) //making sure it only check the touch once && it was a short touch/tap and not a dragging.
                StartCoroutine("singleOrDouble");
        }
        else
            touchDuration = 0.0f;

    }

    IEnumerator singleOrDouble()
    {
        yield return new WaitForSeconds(0.3f);
        if (touch.tapCount == 1)
            Debug.Log("Single");
        else if (touch.tapCount == 2)
        {
            //this coroutine has been called twice. We should stop the next one here otherwise we get two double tap
            StopCoroutine("singleOrDouble");
            Debug.Log("Double");
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            ShootRay(ray);
        }
    }


    void ShootRay(Ray ray)
    {

        RaycastHit rhit;

        if (Physics.Raycast(ray, out rhit, 1000.0f))
        {
            Debug.Log("Object clicked...");

            if(URL != "")
                Application.OpenURL(URL);
        }

    }


    public void SetURL(string URL)
    {
        this.URL = URL;
    }
}
