using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowMenu : MonoBehaviour {

    public RectTransform menuPanel;
    public Button menuButton;
    public Vector3 menuButtonEndPosition, menuPanelEndPosition;
    public bool debugMode;
    public int menuSpeed;

    private bool menuActive = false;

    private GameObject cloudRecoGameObject;

    private Vector3 menuButtonInitPos, menuPanelInitPos;

    // Use this for initialization
    void Start ()
    {
        menuButtonInitPos = menuButton.GetComponent<RectTransform>().localPosition;
        menuPanelInitPos = menuPanel.GetComponent<RectTransform>().localPosition;

        cloudRecoGameObject = GameObject.Find("CloudRecgonition");

        if(debugMode)
        {
            Debug.Log("Menu Button:" + menuButtonInitPos);
            Debug.Log("Menu Panel:" + menuPanelInitPos);
        }

    }

    public void OnButtonClick()
    {
        
        if(!menuActive)
        {
            if(cloudRecoGameObject != null)
            {
                cloudRecoGameObject.GetComponent<CloudRecoEventHandler>().OnActive(false);
                Debug.Log("Paused");
            }
            //maybe use exact positions
            menuActive = true;
            iTween.MoveTo(menuButton.gameObject, iTween.Hash("islocal", true, "position", menuButtonEndPosition, "speed", menuSpeed));
            iTween.MoveTo(menuPanel.gameObject, iTween.Hash("islocal", true, "position", menuPanelEndPosition, "speed", menuSpeed));

        }
        else
        {
            if (cloudRecoGameObject != null)
            {
                cloudRecoGameObject.GetComponent<CloudRecoEventHandler>().OnActive(true);
                Debug.Log("Active");
            }
            menuActive = false;
            iTween.MoveTo(menuButton.gameObject, iTween.Hash("islocal", true, "position", menuButtonInitPos, "speed", menuSpeed));
            iTween.MoveTo(menuPanel.gameObject, iTween.Hash("islocal", true, "position",  menuPanelInitPos, "speed", menuSpeed));
        }
        
        /*
        if(!menuActive)
        {
            menuActive = true;
            menuButton.GetComponent<RectTransform>().localPosition = menuButtonEndPosition;
            menuPanel.GetComponent<RectTransform>().localPosition = menuPanelEndPosition;
        }
        else
        {
            menuActive = false;
            menuButton.GetComponent<RectTransform>().localPosition = menuButtonInitPos;
            menuPanel.GetComponent<RectTransform>().localPosition = menuPanelInitPos;
        }
        */
    }
	

}
