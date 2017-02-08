/*
(C) 2015 AARO4130
PARTS OF TGA LOADING CODE (C) 2013 mikezila
DO NOT USE PARTS OF, OR THE ENTIRE SCRIPT, AND CLAIM AS YOUR OWN WORK
*/

using System;
using UnityEngine;
using System.Collections;
using System.IO;

public class TextureLoader : MonoBehaviour {

    public static string url;

    static Texture2D t2d = new Texture2D(1, 1);

    public static void SetNormalMap(ref Texture2D tex)
    {
        Color[] pixels = tex.GetPixels();
        for(int i=0; i < pixels.Length; i++)
        {
            Color temp = pixels[i];
            temp.r = pixels[i].g;
            temp.a = pixels[i].r;
            pixels[i] = temp;
        }
        tex.SetPixels(pixels);
    }
    public static Texture2D LoadTexture(string imageUrl,bool normalMap = false)
    {
       
        url = imageUrl;

        if (normalMap)
            SetNormalMap(ref t2d);
        return t2d;

    }

    IEnumerator DownloadImages()
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (www.error != null)
                throw new UnityException("WWW download had an error:" + www.error);
            www.LoadImageIntoTexture(t2d);
        }
    }

}
