using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;

public class FBShareScript : MonoBehaviour {
    
    public void Screenshot()
    {
        StartCoroutine("TakeScreenshot");
    }

    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        tex.Apply();
        byte[] screenshot = tex.EncodeToPNG();

        var wwwForm = new WWWForm();

        string picName = "Idioman_" + Time.time + ".png";
        wwwForm.AddBinaryData("image", screenshot, picName);

        Debug.Log("trying to post screenshot");
        try
        {
            FB.API("me/photos", HttpMethod.POST, PostPicCallback, wwwForm);
        } catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    private void PostPicCallback(IGraphResult result)
    {
        if (result.Error != null)
        {
            Debug.Log(result.Error);
        }
        else
        {
            Debug.Log("Posted Screenshot");
        }
    }
}