using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ShowMenu : MonoBehaviour {

    public RectTransform menuPanel;
    public Button menuButton;
    public GameObject settingsPanel;
    public Vector3 menuButtonEndPosition, menuPanelEndPosition;
    public bool debugMode;
    public int menuSpeed;


    private bool menuActive = false;
    private bool settingsMenuActive = false;

    private Vector3 menuButtonInitPos, menuPanelInitPos;

    // Use this for initialization
    void Start ()
    {
        menuButtonInitPos = menuButton.GetComponent<RectTransform>().localPosition;
        menuPanelInitPos = menuPanel.GetComponent<RectTransform>().localPosition;

        settingsPanel.SetActive(false);

        if(debugMode)
        {
            Debug.Log("Menu Button:" + menuButtonInitPos);
            Debug.Log("Menu Panel:" + menuPanelInitPos);
        }

    }

    void Update()
    {

        if(menuActive && !settingsMenuActive)
        {
            //Get click outside of UI panel
            foreach (Touch touch in Input.touches)
            {
                int pointerID = touch.fingerId;
                if (EventSystem.current.IsPointerOverGameObject(pointerID))
                {
                    // at least one touch is over a canvas UI
                    return;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    // here we don't know if the touch was over an canvas UI
                    return;
                }

                //outside menu clicked
                OnMenuButtonClick();

            }
        }

#if UNITY_ANDROID
        if (menuActive)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (!settingsMenuActive)
                {
                    OnMenuButtonClick();
                }
                else
                {
                    OnSettingsMenuClick();
                }
            }

        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
#endif
    }

    public void OnMenuButtonClick()
    {

        if (!menuActive)
        {
            menuActive = true;
            iTween.MoveTo(menuButton.gameObject, iTween.Hash("islocal", true, "position", menuButtonEndPosition, "speed", menuSpeed));
            iTween.MoveTo(menuPanel.gameObject, iTween.Hash("islocal", true, "position", menuPanelEndPosition, "speed", menuSpeed));

        }
        else
        {
            menuActive = false;
            iTween.MoveTo(menuButton.gameObject, iTween.Hash("islocal", true, "position", menuButtonInitPos, "speed", menuSpeed));
            iTween.MoveTo(menuPanel.gameObject, iTween.Hash("islocal", true, "position", menuPanelInitPos, "speed", menuSpeed));
        }
    }

    public void OnSettingsMenuClick()
    {
        if(!settingsMenuActive)
        {
            settingsPanel.SetActive(true);
            settingsMenuActive = true;
        }
        else
        {
            settingsPanel.SetActive(false);
            settingsMenuActive = false;
        }
    }

}
