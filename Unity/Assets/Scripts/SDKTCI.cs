using System;
using UnityEngine;
using System.Collections;
using System.IO;






public class SDKTCI
{
    private static SDKTCI _instance;

    public static SDKTCI Instance
    {
        get
        {
            if (_instance == null)
            {
#if UNITY_EDITOR 
                _instance = new SDKDefault();
#elif UNITY_ANDROID
                _instance = new SDKAndroid();
#elif UNITY_IOS
                _instance = new SDKIOS();
#endif
            }

            return _instance;
        }
    }
    public Texture2D curTexture = null;

    public string curTextureMd5 = null;

    public virtual void Open() {
        Debug.Log("open");
    }

    public virtual void SaveTexture()
    { 
        
    }

    

}
