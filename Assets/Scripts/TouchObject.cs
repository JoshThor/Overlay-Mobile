using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchObject : MonoBehaviour {

    string URL = "";

	
	// Update is called once per frame
	void Update () {

        int i = 0;
        while (i < Input.touchCount)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                //Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position), -Vector2.up);

                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                Debug.Log("TAP !");
                ShootRay(ray);
            }
            ++i;
        }

#if (UNITY_EDITOR)
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("TAP !");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            ShootRay(ray);
        }
#endif

    }


    void ShootRay(Ray ray)
    {

        RaycastHit rhit;

        if (Physics.Raycast(ray, out rhit, 1000.0f))
        {
            Debug.Log("Object clicked...");
            Application.OpenURL(URL);
        }

    }


    public void SetURL(string URL)
    {
        this.URL = URL;
    }
}
