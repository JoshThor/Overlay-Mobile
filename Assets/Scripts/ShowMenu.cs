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

    private Vector3 menuButtonInitPos, menuPanelInitPos;

    // Use this for initialization
    void Start ()
    {
        menuButtonInitPos = menuButton.GetComponent<RectTransform>().localPosition;
        menuPanelInitPos = menuPanel.GetComponent<RectTransform>().localPosition;

        if(debugMode)
        {
            Debug.Log("Menu Button:" + menuButtonInitPos);
            Debug.Log("Menu Panel:" + menuPanelInitPos);
        }

    }

    void Update()
    {

#if UNITY_ANDROID
        if (menuActive)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                OnButtonClick();
            }
                    
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
#endif
        /*
        float menuX = menuPanel.GetComponent<RectTransform>().localPosition.x;
        if (menuX > menuPanelEndPosition.x)
        {
            menuPanel.GetComponent<RectTransform>().localPosition = menuPanelEndPosition;
        }

        if (menuX < menuPanelInitPos.x)
        {
            menuPanel.GetComponent<RectTransform>().localPosition = menuPanelInitPos;
        }

        Vector2 initMousePos = Input.mousePosition;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;

           // touchDelta.x = Mathf.Clamp(menuPanel.transform.localPosition.x, menuPanelInitPos.x, menuPanelEndPosition.x);

            menuPanel.transform.localPosition  = new Vector3(+touchDelta.x, 0, 0);
           
        }

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;

            
            if(touchDelta.x > 10)
            {
                if (menuX / (menuPanelEndPosition.x - menuPanelInitPos.x) > .5)
                {
                    menuActive = true;
                    iTween.MoveTo(menuPanel.gameObject, iTween.Hash("islocal", true, "position", menuPanelEndPosition, "speed", menuSpeed));
                }
               
            } else if (touchDelta.x < -10)
            {
                if (menuX / (menuPanelEndPosition.x - menuPanelInitPos.x) < .5)
                {
                    menuActive = false;
                    iTween.MoveTo(menuButton.gameObject, iTween.Hash("islocal", true, "position", menuButtonInitPos, "speed", menuSpeed));
                }
                
            }
           
        }*/
    }

    public void OnButtonClick()
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
}
