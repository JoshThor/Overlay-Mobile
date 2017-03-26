using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Facebook.Unity;
using UnityEngine.UI;

public class FBScript : MonoBehaviour {

    public GameObject dialogueLoggedIn;
    public GameObject dialogueLoggedOut;
    public GameObject dialogueDisplayName;
    public GameObject profilePicture;
    public GameObject loggoutButton;

    // Use this for initialization
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(SetInit, OnHideUnity);

        }
        else
        {
            FB.ActivateApp();
        }
    }
    
    void SetInit()
    {
        if(FB.IsLoggedIn)
        {
            Debug.Log("FB is logged in");
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("FB is not logged in");
        }

        DealWithFBMenus(FB.IsLoggedIn);
    }

    void OnHideUnity(bool isGameShown)
    {
        if(!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void FBLogin()
    {
        List<string> permissions = new List<string>();

        permissions.Add("public_profile");

        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }

    public void FBLogout()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
            StartCoroutine(CheckForSuccussfulLogout());
        }
    }

    IEnumerator CheckForSuccussfulLogout()
    {
        if (FB.IsLoggedIn)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine("CheckForSuccussfulLogout");
        }
        else
        {
            // Here you have successfully logged out.
            DealWithFBMenus(FB.IsLoggedIn);
        }
    }

    void AuthCallBack(IResult result)
    {
        if(result.Error != null)
        {
            Debug.Log(result.Error);
        }
        else
        {
            if (FB.IsLoggedIn)
            {
                Debug.Log("FB is logged in");
            }
            else
            {
                Debug.Log("FB is not logged in");
            }
            DealWithFBMenus(FB.IsLoggedIn);
        }
    }


    void DealWithFBMenus(bool isLoggedIn)
    {
        if(isLoggedIn)
        {
            dialogueLoggedIn.SetActive(true);
            dialogueLoggedOut.SetActive(false);
            loggoutButton.SetActive(true);

            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayName);
            FB.API("/me/picture?type=square&height=256&width=256", HttpMethod.GET, DisplayprofilePicture);
        }
        else
        {
            dialogueLoggedIn.SetActive(false);
            dialogueLoggedOut.SetActive(true);
            loggoutButton.SetActive(false);
        }
    }

    void DisplayName(IResult result)
    {
        Text DisplayName = dialogueDisplayName.GetComponent<Text>();
        if(result.Error == null)
        {
            DisplayName.text = "Welcome, " + result.ResultDictionary["first_name"];
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    void DisplayprofilePicture(IGraphResult result)
    {
        
        if(result.Texture != null)
        {
            Image profileImage = profilePicture.GetComponent<Image>();
            profileImage.sprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2());
        }
        else
        {
            
        }
    }
}
